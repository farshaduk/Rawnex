using AspNetCoreRateLimit;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Rawnex.Application;
using Rawnex.Domain.Entities;
using Rawnex.Infrastructure;
using Rawnex.Infrastructure.Hubs;
using Rawnex.Infrastructure.Jobs;
using Rawnex.Persistence;
using Rawnex.Persistence.Context;
using Rawnex.Persistence.Seed;
using Rawnex.WebApi.GraphQL;
using Rawnex.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---- Serilog ----
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ---- Layer DI Registration ----
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// ---- Rate Limiting ----
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimit"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// ---- CORS ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"])
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for httpOnly cookies
    });
});

// ---- Controllers + Swagger ----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Hangfire server
builder.Services.AddHangfireServer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Rawnex API", Version = "v1" });

    // JWT in Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter your JWT access token",
    });

    options.AddSecurityRequirement(doc => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

// ---- Health Checks ----
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);

// ---- GraphQL ----
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

// ---- Middleware Pipeline ----
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
    await next();
});

app.UseIpRateLimiting();
app.UseMiddleware<VelocityCheckMiddleware>();

app.UseStaticFiles(); // Serve wwwroot/uploads

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapGraphQL("/graphql");

// ---- SignalR Hubs ----
app.MapHub<TradingHub>("/hubs/trading");
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<ShipmentTrackingHub>("/hubs/shipment-tracking");
app.MapHub<ChatHub>("/hubs/chat");

// ---- Hangfire Dashboard (dev only) ----
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}

// ---- Seed Database ----
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        // Ensure Hangfire schema exists before it tries to create tables
        await context.Database.ExecuteSqlRawAsync(
            "IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'hangfire') EXEC('CREATE SCHEMA [hangfire]')");

        // Force Hangfire to create its tables now that the database exists
        // (PrepareSchemaIfNecessary ran during DI before the DB was created, so it failed silently)
        _ = new SqlServerStorage(
            context.Database.GetConnectionString(),
            new SqlServerStorageOptions { PrepareSchemaIfNecessary = true, SchemaName = "hangfire" });

        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Core identity
        await ApplicationDbContextSeed.SeedDefaultRolesAsync(roleManager, logger);
        await ApplicationDbContextSeed.SeedPermissionsAsync(context, logger);
        await ApplicationDbContextSeed.SeedAdminPermissionsAsync(context, roleManager, logger);

        // Tenant & organization
        await ApplicationDbContextSeed.SeedTenantAsync(context, logger);
        await ApplicationDbContextSeed.SeedCompaniesAsync(context, logger);
        await ApplicationDbContextSeed.SeedDefaultAdminAsync(userManager, context, logger);
        await ApplicationDbContextSeed.SeedDemoUsersAsync(userManager, context, logger);
        await ApplicationDbContextSeed.SeedDepartmentsAsync(context, logger);

        // Catalog & inventory
        await ApplicationDbContextSeed.SeedProductCategoriesAsync(context, logger);
        await ApplicationDbContextSeed.SeedProductsAsync(context, logger);
        await ApplicationDbContextSeed.SeedListingsAsync(context, logger);
        await ApplicationDbContextSeed.SeedWarehousesAsync(context, logger);

        // Platform config
        await ApplicationDbContextSeed.SeedCommissionRulesAsync(context, logger);
        await ApplicationDbContextSeed.SeedFeatureFlagsAsync(context, logger);

        // Verification & notifications
        await ApplicationDbContextSeed.SeedKycKybAsync(context, logger);
        await ApplicationDbContextSeed.SeedNotificationsAsync(context, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// ---- Register Recurring Jobs ----
using (var scope = app.Services.CreateScope())
{
    try
    {
        var jobService = scope.ServiceProvider.GetRequiredService<Rawnex.Application.Common.Interfaces.IBackgroundJobService>();
        jobService.AddOrUpdateRecurring<RecurringJobs>("expire-listings", j => j.ExpireListingsAsync(), Cron.Hourly());
        jobService.AddOrUpdateRecurring<RecurringJobs>("expire-rfqs", j => j.ExpireRfqsAsync(), Cron.Hourly());
        jobService.AddOrUpdateRecurring<RecurringJobs>("end-auctions", j => j.EndAuctionsAsync(), Cron.Minutely());
        jobService.AddOrUpdateRecurring<RecurringJobs>("start-scheduled-auctions", j => j.StartScheduledAuctionsAsync(), Cron.Minutely());
        jobService.AddOrUpdateRecurring<RecurringJobs>("overdue-invoices", j => j.SendOverdueInvoiceRemindersAsync(), Cron.Daily());
        jobService.AddOrUpdateRecurring<RecurringJobs>("cleanup-tokens", j => j.CleanupExpiredTokensAsync(), Cron.Daily());
        jobService.AddOrUpdateRecurring<RecurringJobs>("expire-negotiations", j => j.ExpireNegotiationsAsync(), Cron.Hourly());
        jobService.AddOrUpdateRecurring<RecurringJobs>("seller-re-audit", j => j.SellerReAuditAsync(), Cron.Weekly());
        jobService.AddOrUpdateRecurring<RecurringJobs>("detect-off-platform-payments", j => j.DetectOffPlatformPaymentsAsync(), Cron.Daily());
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Failed to register recurring jobs. They will be registered on next restart.");
    }
}

app.Run();
