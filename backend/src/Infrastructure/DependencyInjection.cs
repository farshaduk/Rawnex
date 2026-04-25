using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Infrastructure.Authorization;
using Rawnex.Infrastructure.Identity;
using Rawnex.Infrastructure.Jobs;
using Rawnex.Infrastructure.Services;

namespace Rawnex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT Settings
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // JWT Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // No tolerance — token expires exactly when it says
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers["X-Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                }
            };
        });

        // Dynamic permission-based authorization
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddAuthorization();

        // Services
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IPushNotificationService, PushNotificationService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IRealTimeNotifier, RealTimeNotifier>();
        services.AddSingleton<IBackgroundJobService, BackgroundJobService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IFraudScoringService, FraudScoringService>();
        services.AddScoped<IWebhookDispatcher, WebhookDispatcher>();
        services.AddScoped<IPaymentGatewayService, StripePaymentService>();
        services.AddScoped<IErpConnectorService, ErpConnectorService>();
        services.AddScoped<ICommodityPriceFeedService, CommodityPriceFeedService>();
        services.AddScoped<ISanctionScreeningService, SanctionScreeningService>();
        services.AddScoped<IBiometricVerificationService, BiometricVerificationService>();
        services.AddScoped<IOcrDocumentService, OcrDocumentService>();
        services.AddScoped<IBillOfLadingService, BillOfLadingService>();
        services.AddScoped<ICarrierApiService, CarrierApiService>();
        services.AddScoped<RecurringJobs>();
        services.AddHttpClient("Webhook");
        services.AddHttpClient("Sms");
        services.AddHttpClient("Firebase");
        services.AddHttpClient("Stripe");
        services.AddHttpClient("Erp");
        services.AddHttpClient("CommodityPriceFeed");
        services.AddHttpClient("SanctionScreening");
        services.AddHttpClient("Biometric");
        services.AddHttpClient("Ocr");
        services.AddHttpClient("Carrier");

        // SignalR
        var redisConnection = configuration.GetConnectionString("Redis");
        var signalRBuilder = services.AddSignalR();
        if (!string.IsNullOrEmpty(redisConnection))
        {
            signalRBuilder.AddStackExchangeRedis(redisConnection, options =>
            {
                options.Configuration.ChannelPrefix = StackExchange.Redis.RedisChannel.Literal("Rawnex");
            });
        }

        // Distributed Cache (Redis or in-memory fallback)
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "Rawnex:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        // Hangfire
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true,
                PrepareSchemaIfNecessary = true,
                SchemaName = "hangfire"
            }));

        services.AddHttpContextAccessor();

        return services;
    }
}
