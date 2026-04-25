using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Persistence.Context;

namespace Rawnex.Persistence.Seed;

public static class ApplicationDbContextSeed
{
    /// <summary>
    /// All system resources. Add new resources here as the system grows.
    /// </summary>
    private static readonly string[] SystemResources =
    [
        "users", "roles", "permissions", "branches", "departments",
        "products", "orders", "categories", "dashboard", "reports",
        "settings", "audit-logs", "sessions", "companies", "listings",
        "rfqs", "auctions", "contracts", "payments", "shipments",
        "disputes", "commissions", "feature-flags", "kyc", "analytics",
        "warehouses", "inventory", "notifications", "chat"
    ];

    // ─── Stable GUIDs for cross-referencing ───
    private static readonly Guid TenantId = new("A0000000-0000-0000-0000-000000000001");
    private static readonly Guid AdminUserId = new("B0000000-0000-0000-0000-000000000001");
    private static readonly Guid BuyerUserId = new("B0000000-0000-0000-0000-000000000002");
    private static readonly Guid SellerUserId = new("B0000000-0000-0000-0000-000000000003");
    private static readonly Guid BuyerManagerId = new("B0000000-0000-0000-0000-000000000004");
    private static readonly Guid SellerManagerId = new("B0000000-0000-0000-0000-000000000005");

    private static readonly Guid BuyerCompanyId = new("C0000000-0000-0000-0000-000000000001");
    private static readonly Guid SellerCompanyId = new("C0000000-0000-0000-0000-000000000002");
    private static readonly Guid TraderCompanyId = new("C0000000-0000-0000-0000-000000000003");

    // Categories
    private static readonly Guid CatMetalsId = new("D0000000-0000-0000-0000-000000000001");
    private static readonly Guid CatChemicalsId = new("D0000000-0000-0000-0000-000000000002");
    private static readonly Guid CatMineralsId = new("D0000000-0000-0000-0000-000000000003");
    private static readonly Guid CatAgriId = new("D0000000-0000-0000-0000-000000000004");
    private static readonly Guid CatEnergyId = new("D0000000-0000-0000-0000-000000000005");
    private static readonly Guid CatPolymersId = new("D0000000-0000-0000-0000-000000000006");
    // Sub-categories
    private static readonly Guid CatFerrousId = new("D0000000-0000-0000-0000-000000000011");
    private static readonly Guid CatNonFerrousId = new("D0000000-0000-0000-0000-000000000012");
    private static readonly Guid CatPreciousId = new("D0000000-0000-0000-0000-000000000013");
    private static readonly Guid CatPetroId = new("D0000000-0000-0000-0000-000000000021");
    private static readonly Guid CatIndustrialChemId = new("D0000000-0000-0000-0000-000000000022");

    // Products
    private static readonly Guid ProdCopperId = new("E0000000-0000-0000-0000-000000000001");
    private static readonly Guid ProdIronId = new("E0000000-0000-0000-0000-000000000002");
    private static readonly Guid ProdSodaAshId = new("E0000000-0000-0000-0000-000000000003");
    private static readonly Guid ProdBitumenId = new("E0000000-0000-0000-0000-000000000004");
    private static readonly Guid ProdChromiteId = new("E0000000-0000-0000-0000-000000000005");
    private static readonly Guid ProdSaffronId = new("E0000000-0000-0000-0000-000000000006");
    private static readonly Guid ProdPistachioId = new("E0000000-0000-0000-0000-000000000007");
    private static readonly Guid ProdUreaId = new("E0000000-0000-0000-0000-000000000008");
    private static readonly Guid ProdPPId = new("E0000000-0000-0000-0000-000000000009");
    private static readonly Guid ProdZincId = new("E0000000-0000-0000-0000-000000000010");

    // Warehouses
    private static readonly Guid WarehouseTehranId = new("F0000000-0000-0000-0000-000000000001");
    private static readonly Guid WarehouseBandarId = new("F0000000-0000-0000-0000-000000000002");
    private static readonly Guid WarehouseIsfahanId = new("F0000000-0000-0000-0000-000000000003");

    private static readonly DateTime Now = new(2026, 4, 6, 0, 0, 0, DateTimeKind.Utc);

    // ═══════════════════════════════════════════════════════════
    //  1. ROLES
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedDefaultRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {
        var roles = new[]
        {
            new ApplicationRole { Name = ApplicationRole.Admin, Description = "Full system access", IsSystemRole = true },
            new ApplicationRole { Name = ApplicationRole.BranchAdmin, Description = "Branch-level management", IsSystemRole = true },
            new ApplicationRole { Name = ApplicationRole.DepartmentManager, Description = "Department-level management", IsSystemRole = true },
            new ApplicationRole { Name = ApplicationRole.User, Description = "Standard user access", IsSystemRole = true },
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
                logger.LogInformation("Seeded role: {Role}", role.Name);
            }
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  2. PERMISSIONS
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedPermissionsAsync(ApplicationDbContext context, ILogger logger)
    {
        var existingKeys = await context.Permissions
            .Select(p => new { p.Resource, p.Action })
            .ToListAsync();

        var existingSet = existingKeys
            .Select(e => $"{e.Resource}.{e.Action}")
            .ToHashSet();

        var newPermissions = new List<Permission>();

        foreach (var resource in SystemResources)
        {
            foreach (var action in Enum.GetValues<PermissionAction>())
            {
                var key = $"{resource}.{action}";
                if (!existingSet.Contains(key))
                {
                    newPermissions.Add(new Permission
                    {
                        Id = Guid.NewGuid(),
                        Resource = resource,
                        Action = action,
                        Description = $"{action} access for {resource}",
                        IsSystem = true
                    });
                }
            }
        }

        if (newPermissions.Count > 0)
        {
            context.Permissions.AddRange(newPermissions);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} new permissions", newPermissions.Count);
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  3. ADMIN ROLE → ALL PERMISSIONS
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedAdminPermissionsAsync(
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
        ILogger logger)
    {
        var adminRole = await roleManager.FindByNameAsync(ApplicationRole.Admin);
        if (adminRole is null) return;

        var allPermissionIds = await context.Permissions.Select(p => p.Id).ToListAsync();
        var existingRolePermissionIds = await context.RolePermissions
            .Where(rp => rp.RoleId == adminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToHashSetAsync();

        var newRolePermissions = allPermissionIds
            .Where(id => !existingRolePermissionIds.Contains(id))
            .Select(id => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = id,
                GrantedBy = "System"
            })
            .ToList();

        if (newRolePermissions.Count > 0)
        {
            context.RolePermissions.AddRange(newRolePermissions);
            await context.SaveChangesAsync();
            logger.LogInformation("Assigned {Count} permissions to Admin role", newRolePermissions.Count);
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  4. DEFAULT ADMIN USER
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedDefaultAdminAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger logger)
    {
        // Ensure tenant exists first
        if (!await context.Tenants.AnyAsync(t => t.Id == TenantId))
            return; // Tenant seed must run before this

        var adminEmail = "admin@rawnex.com";
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

        if (existingAdmin is null)
        {
            var admin = new ApplicationUser
            {
                Id = AdminUserId,
                TenantId = TenantId,
                Email = adminEmail,
                UserName = adminEmail,
                FirstName = "مدیر",
                LastName = "سیستم",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = Now
            };

            var result = await userManager.CreateAsync(admin, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(admin, [ApplicationRole.Admin, ApplicationRole.User]);
                logger.LogInformation("Seeded default admin user: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Failed to seed admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    // ═══════════════════════════════════════════════════════════
    //  5. TENANT & TENANT SETTINGS
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedTenantAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Tenants.AnyAsync(t => t.Id == TenantId))
            return;

        var tenant = new Tenant
        {
            Id = TenantId,
            Name = "راونکس ایران",
            Subdomain = "iran",
            Status = TenantStatus.Active,
            Plan = TenantPlan.Enterprise,
            ContactEmail = "info@rawnex.com",
            ContactPhone = "+98-21-88776655",
            SubscriptionExpiresAt = Now.AddYears(1),
            CreatedAt = Now,
            CreatedBy = "System"
        };

        context.Tenants.Add(tenant);

        context.TenantSettings.Add(new TenantSettings
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            DefaultCurrency = "IRR",
            DefaultLanguage = "fa",
            TimeZone = "Asia/Tehran",
            RequireMfa = false,
            RequireKyc = true,
            MaxUsersAllowed = 200
        });

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded default tenant: {Name}", tenant.Name);
    }

    // ═══════════════════════════════════════════════════════════
    //  6. COMPANIES
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedCompaniesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Companies.AnyAsync(c => c.TenantId == TenantId))
            return;

        var companies = new List<Company>
        {
            new()
            {
                Id = BuyerCompanyId,
                TenantId = TenantId,
                LegalName = "شرکت فولاد پارسیان",
                TradeName = "Parsian Steel Co.",
                RegistrationNumber = "10320654321",
                TaxId = "411234567890",
                Type = CompanyType.Buyer,
                Status = CompanyStatus.Active,
                Website = "https://parsiansteel.example.com",
                Description = "شرکت خریدار مواد خام فلزات و مواد شیمیایی صنعتی",
                AddressLine1 = "خیابان ولیعصر، پلاک ۱۲۵",
                City = "تهران",
                State = "تهران",
                PostalCode = "1516713411",
                Country = "IR",
                Phone = "+98-21-22334455",
                Email = "info@parsiansteel.example.com",
                CreditLimit = 5_000_000_000m,
                DefaultCurrency = Currency.IRR,
                BankName = "بانک صادرات ایران",
                BankAccountNumber = "0102345678901",
                BankIban = "IR820170000000102345678901",
                BankSwiftCode = "BSIRIRTH",
                VerificationStatus = VerificationStatus.Approved,
                VerifiedAt = Now.AddDays(-30),
                VerifiedBy = "System",
                TrustScore = 87m,
                CreatedAt = Now.AddMonths(-6),
                CreatedBy = "System"
            },
            new()
            {
                Id = SellerCompanyId,
                TenantId = TenantId,
                LegalName = "شرکت معدنی زرین سنگ",
                TradeName = "ZarrinSang Mining Corp.",
                RegistrationNumber = "10260987654",
                TaxId = "412987654321",
                Type = CompanyType.Seller,
                Status = CompanyStatus.Active,
                Website = "https://zarrinsang.example.com",
                Description = "تولیدکننده و صادرکننده مواد معدنی و مواد اولیه صنعتی",
                AddressLine1 = "بلوار صنعت، کیلومتر ۱۵",
                City = "اصفهان",
                State = "اصفهان",
                PostalCode = "8156713211",
                Country = "IR",
                Phone = "+98-31-36778899",
                Email = "info@zarrinsang.example.com",
                CreditLimit = 10_000_000_000m,
                DefaultCurrency = Currency.IRR,
                BankName = "بانک ملت",
                BankAccountNumber = "6102345678902",
                BankIban = "IR620120000006102345678902",
                BankSwiftCode = "BKMTIRTH",
                VerificationStatus = VerificationStatus.Approved,
                VerifiedAt = Now.AddDays(-60),
                VerifiedBy = "System",
                EsgScore = 72m,
                TrustScore = 92m,
                CreatedAt = Now.AddMonths(-8),
                CreatedBy = "System"
            },
            new()
            {
                Id = TraderCompanyId,
                TenantId = TenantId,
                LegalName = "شرکت بازرگانی آرمان تجارت",
                TradeName = "Arman Trading LLC",
                RegistrationNumber = "10101234567",
                TaxId = "413112233445",
                Type = CompanyType.Both,
                Status = CompanyStatus.Active,
                Description = "بازرگانی و واسطه‌گری مواد اولیه بین‌المللی",
                AddressLine1 = "منطقه آزاد کیش، بلوک ۷",
                City = "کیش",
                State = "هرمزگان",
                PostalCode = "7941700000",
                Country = "IR",
                Phone = "+98-76-44556677",
                Email = "info@armantrading.example.com",
                CreditLimit = 8_000_000_000m,
                DefaultCurrency = Currency.USD,
                BankName = "بانک پاسارگاد",
                BankAccountNumber = "3001234567890",
                BankIban = "IR570570000003001234567890",
                BankSwiftCode = "BKPAIRTH",
                VerificationStatus = VerificationStatus.Approved,
                VerifiedAt = Now.AddDays(-15),
                VerifiedBy = "System",
                TrustScore = 78m,
                CreatedAt = Now.AddMonths(-3),
                CreatedBy = "System"
            }
        };

        context.Companies.AddRange(companies);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} companies", companies.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  7. DEMO USERS (Buyer, Seller, Managers)
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedDemoUsersAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        ILogger logger)
    {
        var demoUsers = new (Guid Id, string Email, string First, string Last, string[] Roles, Guid CompanyId, bool IsCompanyAdmin, string JobTitle)[]
        {
            (BuyerUserId,   "buyer@rawnex.com",          "علی",     "محمدی",     [ApplicationRole.User],                                BuyerCompanyId,  false, "کارشناس خرید"),
            (SellerUserId,  "seller@rawnex.com",         "فاطمه",   "رضایی",     [ApplicationRole.User],                                SellerCompanyId, false, "کارشناس فروش"),
            (BuyerManagerId,"buyer-manager@rawnex.com",  "حسین",    "احمدی",     [ApplicationRole.DepartmentManager, ApplicationRole.User], BuyerCompanyId,  true,  "مدیر خرید"),
            (SellerManagerId,"seller-manager@rawnex.com","مریم",    "کریمی",     [ApplicationRole.DepartmentManager, ApplicationRole.User], SellerCompanyId, true,  "مدیر فروش"),
        };

        foreach (var (id, email, first, last, roles, companyId, isAdmin, jobTitle) in demoUsers)
        {
            if (await userManager.FindByEmailAsync(email) is not null)
                continue;

            var user = new ApplicationUser
            {
                Id = id,
                TenantId = TenantId,
                Email = email,
                UserName = email,
                FirstName = first,
                LastName = last,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = Now.AddMonths(-2)
            };

            var result = await userManager.CreateAsync(user, "Demo@123456");
            if (!result.Succeeded)
            {
                logger.LogError("Failed to seed user {Email}: {Errors}", email,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                continue;
            }

            await userManager.AddToRolesAsync(user, roles);

            // Link to company
            context.CompanyMembers.Add(new CompanyMember
            {
                Id = Guid.NewGuid(),
                TenantId = TenantId,
                CompanyId = companyId,
                UserId = id,
                JobTitle = jobTitle,
                IsCompanyAdmin = isAdmin,
                IsActive = true,
                CreatedAt = Now.AddMonths(-2),
                CreatedBy = "System"
            });

            logger.LogInformation("Seeded demo user: {Email}", email);
        }

        // Also link admin to trader company
        if (!await context.CompanyMembers.AnyAsync(cm => cm.UserId == AdminUserId))
        {
            context.CompanyMembers.Add(new CompanyMember
            {
                Id = Guid.NewGuid(),
                TenantId = TenantId,
                CompanyId = TraderCompanyId,
                UserId = AdminUserId,
                JobTitle = "مدیرعامل",
                IsCompanyAdmin = true,
                IsActive = true,
                CreatedAt = Now,
                CreatedBy = "System"
            });
        }

        await context.SaveChangesAsync();
    }

    // ═══════════════════════════════════════════════════════════
    //  8. DEPARTMENTS
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedDepartmentsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Departments.AnyAsync(d => d.TenantId == TenantId))
            return;

        var departments = new List<Department>();

        var deptData = new (Guid CompanyId, string Name, DepartmentType Type, Guid? ManagerId)[]
        {
            (BuyerCompanyId,  "واحد خرید",         DepartmentType.Procurement,    BuyerManagerId),
            (BuyerCompanyId,  "واحد مالی",          DepartmentType.Finance,        null),
            (BuyerCompanyId,  "واحد انبار و لجستیک", DepartmentType.Logistics,      null),
            (SellerCompanyId, "واحد فروش",         DepartmentType.Sales,          SellerManagerId),
            (SellerCompanyId, "واحد کنترل کیفیت",   DepartmentType.QualityControl, null),
            (SellerCompanyId, "واحد مالی",          DepartmentType.Finance,        null),
            (TraderCompanyId, "واحد بازرگانی",      DepartmentType.Sales,          null),
            (TraderCompanyId, "واحد حقوقی",        DepartmentType.Legal,          null),
        };

        foreach (var (companyId, name, type, managerId) in deptData)
        {
            departments.Add(new Department
            {
                Id = Guid.NewGuid(),
                TenantId = TenantId,
                CompanyId = companyId,
                Name = name,
                Type = type,
                ManagerUserId = managerId,
                CreatedAt = Now.AddMonths(-5),
                CreatedBy = "System"
            });
        }

        context.Departments.AddRange(departments);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} departments", departments.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  9. PRODUCT CATEGORIES (hierarchical)
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedProductCategoriesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.ProductCategories.AnyAsync())
            return;

        var categories = new List<ProductCategory>
        {
            // ── Top-level ──
            new() { Id = CatMetalsId,     Name = "Metals & Alloys",     NameFa = "فلزات و آلیاژها",     Slug = "metals",     SortOrder = 1, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = CatChemicalsId,  Name = "Chemicals",           NameFa = "مواد شیمیایی",        Slug = "chemicals",  SortOrder = 2, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = CatMineralsId,   Name = "Minerals & Ores",     NameFa = "مواد معدنی و سنگ‌ها",  Slug = "minerals",   SortOrder = 3, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = CatAgriId,       Name = "Agricultural Products",NameFa = "محصولات کشاورزی",    Slug = "agriculture",SortOrder = 4, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = CatEnergyId,     Name = "Energy & Petroleum",  NameFa = "انرژی و نفت",         Slug = "energy",     SortOrder = 5, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = CatPolymersId,   Name = "Polymers & Plastics", NameFa = "پلیمر و پلاستیک",     Slug = "polymers",   SortOrder = 6, IsActive = true, CreatedAt = Now, CreatedBy = "System" },

            // ── Sub-categories ──
            new() { Id = CatFerrousId,       Name = "Ferrous Metals",      NameFa = "فلزات آهنی",         Slug = "ferrous-metals",      ParentCategoryId = CatMetalsId,    SortOrder = 1, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = CatNonFerrousId,    Name = "Non-Ferrous Metals",  NameFa = "فلزات غیرآهنی",      Slug = "non-ferrous-metals",  ParentCategoryId = CatMetalsId,    SortOrder = 2, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = CatPreciousId,      Name = "Precious Metals",     NameFa = "فلزات گرانبها",      Slug = "precious-metals",     ParentCategoryId = CatMetalsId,    SortOrder = 3, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = CatPetroId,         Name = "Petrochemicals",      NameFa = "پتروشیمی",           Slug = "petrochemicals",      ParentCategoryId = CatChemicalsId, SortOrder = 1, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = CatIndustrialChemId,Name = "Industrial Chemicals",NameFa = "مواد شیمیایی صنعتی", Slug = "industrial-chemicals",ParentCategoryId = CatChemicalsId, SortOrder = 2, IsActive = true, CreatedAt = Now, CreatedBy = "System" },
        };

        context.ProductCategories.AddRange(categories);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} product categories", categories.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  10. PRODUCTS (with attributes, variants, certifications)
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedProductsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Products.AnyAsync(p => p.TenantId == TenantId))
            return;

        var products = new List<Product>
        {
            BuildProduct(ProdCopperId, SellerCompanyId, CatNonFerrousId, "Copper Cathode", "کاتد مس", "copper-cathode",
                "High-purity copper cathode Grade A, 99.99% Cu content. Produced in Sarcheshmeh copper complex.",
                "کاتد مس با خلوص بالا گرید A، حاوی ۹۹.۹۹٪ مس. تولید مجتمع مس سرچشمه.",
                "CU-CATH-001", "7440-50-8", "99.99%", "Iran", PackagingType.Palletized, "MT", 25m, 500m,
                8950m, Currency.USD, "/MT", ProductStatus.Active),

            BuildProduct(ProdIronId, SellerCompanyId, CatFerrousId, "Iron Ore Concentrate", "کنسانتره سنگ آهن", "iron-ore-concentrate",
                "Iron ore concentrate 67% Fe, low silica. Suitable for direct reduction and blast furnace.",
                "کنسانتره سنگ آهن ۶۷٪ آهن، سیلیس پایین. مناسب احیای مستقیم و کوره بلند.",
                "FE-CONC-001", "1317-61-9", "67% Fe", "Iran", PackagingType.Bulk, "MT", 1000m, 50000m,
                120m, Currency.USD, "/MT", ProductStatus.Active),

            BuildProduct(ProdSodaAshId, SellerCompanyId, CatIndustrialChemId, "Soda Ash Dense", "سودا اش سنگین", "soda-ash-dense",
                "Dense soda ash (Na₂CO₃) 99.2% purity. For glass, detergent, and chemical industries.",
                "سودا اش سنگین (Na₂CO₃) با خلوص ۹۹.۲٪. برای صنایع شیشه، شوینده و شیمیایی.",
                "NA-SODA-001", "497-19-8", "99.2%", "Iran", PackagingType.Bagged, "MT", 20m, 5000m,
                280m, Currency.USD, "/MT", ProductStatus.Active),

            BuildProduct(ProdBitumenId, TraderCompanyId, CatPetroId, "Bitumen 60/70", "قیر ۶۰/۷۰", "bitumen-60-70",
                "Penetration grade bitumen 60/70 for road construction. Meets ASTM D946 standards.",
                "قیر نفوذی ۶۰/۷۰ برای راهسازی. مطابق استاندارد ASTM D946.",
                "BIT-6070-001", "8052-42-4", "60/70 Pen", "Iran", PackagingType.Drummed, "MT", 20m, 10000m,
                380m, Currency.USD, "/MT", ProductStatus.Active),

            BuildProduct(ProdChromiteId, SellerCompanyId, CatMineralsId, "Chromite Ore", "سنگ کرومیت", "chromite-ore",
                "Lumpy chromite ore 42% Cr₂O₃. High Cr/Fe ratio, suitable for ferrochrome production.",
                "سنگ کرومیت دانه‌بندی شده ۴۲٪ Cr₂O₃. نسبت بالای کروم به آهن، مناسب تولید فروکروم.",
                "CR-ORE-001", "1308-31-2", "42% Cr₂O₃", "Iran", PackagingType.Bulk, "MT", 100m, 20000m,
                290m, Currency.USD, "/MT", ProductStatus.Active),

            BuildProduct(ProdSaffronId, TraderCompanyId, CatAgriId, "Saffron (Super Negin)", "زعفران سوپر نگین", "saffron-super-negin",
                "Premium Iranian Super Negin saffron. Hand-picked, ISO 3632 Grade I. Crocin >260.",
                "زعفران سوپر نگین ممتاز ایرانی. دست‌چین، استاندارد ISO 3632 درجه ۱. کروسین بالای ۲۶۰.",
                "SAF-SN-001", null, "Super Negin", "Iran", PackagingType.Bottled, "KG", 0.5m, 100m,
                1450m, Currency.USD, "/KG", ProductStatus.Active),

            BuildProduct(ProdPistachioId, TraderCompanyId, CatAgriId, "Pistachio (Ahmad Aghaei)", "پسته احمد آقایی", "pistachio-ahmad-aghaei",
                "Long pistachio Ahmad Aghaei grade. Naturally opened. Size: 26-28 count per ounce.",
                "پسته احمد آقایی بلند. خندان طبیعی. سایز ۲۶-۲۸ دانه در هر اونس.",
                "PIS-AA-001", null, "26-28 oz", "Iran", PackagingType.Bagged, "MT", 1m, 200m,
                8500m, Currency.USD, "/MT", ProductStatus.Active),

            BuildProduct(ProdUreaId, SellerCompanyId, CatPetroId, "Urea Granular 46%", "اوره گرانوله ۴۶٪", "urea-granular-46",
                "Prilled/granular urea 46% nitrogen content. For agricultural and industrial use.",
                "اوره گرانوله با ۴۶٪ نیتروژن. برای مصارف کشاورزی و صنعتی.",
                "UR-GR-001", "57-13-6", "46% N", "Iran", PackagingType.Bagged, "MT", 100m, 30000m,
                320m, Currency.USD, "/MT", ProductStatus.Active),

            BuildProduct(ProdPPId, TraderCompanyId, CatPolymersId, "Polypropylene (PP) Homopolymer", "پلی‌پروپیلن هموپلیمر", "polypropylene-homopolymer",
                "Polypropylene homopolymer injection grade. MFI 10-12 g/10min. For packaging and automotive.",
                "پلی‌پروپیلن هموپلیمر گرید تزریقی. MFI ۱۰-۱۲. برای بسته‌بندی و خودرو.",
                "PP-HP-001", "9003-07-0", "Injection Grade", "Iran", PackagingType.Bagged, "MT", 20m, 5000m,
                1200m, Currency.USD, "/MT", ProductStatus.Active),

            BuildProduct(ProdZincId, SellerCompanyId, CatNonFerrousId, "Zinc Ingot 99.995%", "شمش روی ۹۹.۹۹۵٪", "zinc-ingot",
                "High-grade zinc ingot SHG 99.995%. LME registered brand quality.",
                "شمش روی با خلوص ۹۹.۹۹۵٪ با کیفیت ثبت‌شده LME.",
                "ZN-ING-001", "7440-66-6", "99.995%", "Iran", PackagingType.Palletized, "MT", 10m, 1000m,
                2650m, Currency.USD, "/MT", ProductStatus.Active),
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        // ── Attributes ──
        var attributes = new List<ProductAttribute>
        {
            Attr(ProdCopperId, "Copper Content",  "99.99",  "%",   1),
            Attr(ProdCopperId, "Weight per Unit",  "125",   "kg",  2),
            Attr(ProdCopperId, "Dimensions",       "914×914","mm", 3),

            Attr(ProdIronId, "Fe Content",   "67",    "%", 1),
            Attr(ProdIronId, "SiO₂",         "3.5",   "%", 2),
            Attr(ProdIronId, "Moisture",      "8",     "%", 3),

            Attr(ProdSodaAshId, "Na₂CO₃ Content","99.2","%", 1),
            Attr(ProdSodaAshId, "Bulk Density",  "1.05","g/cm³",2),
            Attr(ProdSodaAshId, "Bag Size",      "50",  "kg",  3),

            Attr(ProdBitumenId, "Penetration",   "60-70", "dmm", 1),
            Attr(ProdBitumenId, "Softening Point","49-56","°C",  2),
            Attr(ProdBitumenId, "Flash Point",   "250+", "°C",  3),

            Attr(ProdChromiteId, "Cr₂O₃ Content","42",    "%", 1),
            Attr(ProdChromiteId, "Cr/Fe Ratio",   "2.5:1","",  2),
            Attr(ProdChromiteId, "Size",          "10-80", "mm",3),

            Attr(ProdUreaId, "Nitrogen",        "46",  "%",   1),
            Attr(ProdUreaId, "Biuret",          "<1",  "%",   2),
            Attr(ProdUreaId, "Granule Size",    "2-4", "mm",  3),

            Attr(ProdPPId, "MFI",           "10-12","g/10min", 1),
            Attr(ProdPPId, "Density",       "0.905","g/cm³",   2),
            Attr(ProdPPId, "Tensile Strength","35", "MPa",     3),

            Attr(ProdZincId, "Zn Purity",     "99.995","%",   1),
            Attr(ProdZincId, "Ingot Weight",  "25",    "kg",  2),
        };
        context.ProductAttributes.AddRange(attributes);

        // ── Variants ──
        var variants = new List<ProductVariant>
        {
            new() { Id = Guid.NewGuid(), ProductId = ProdCopperId, Name = "Grade A – Full Plate", Sku = "CU-CATH-FP", PurityGrade = "99.99%", Packaging = PackagingType.Palletized, Price = 8950m, PriceCurrency = Currency.USD, AvailableQuantity = 200m, UnitOfMeasure = "MT", IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdCopperId, Name = "Grade A – Cut Cathode", Sku = "CU-CATH-CC", PurityGrade = "99.99%", Packaging = PackagingType.Palletized, Price = 9050m, PriceCurrency = Currency.USD, AvailableQuantity = 80m, UnitOfMeasure = "MT", IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdIronId, Name = "Concentrate 67% Fe", Sku = "FE-CONC-67", PurityGrade = "67%", Packaging = PackagingType.Bulk, Price = 120m, PriceCurrency = Currency.USD, AvailableQuantity = 25000m, UnitOfMeasure = "MT", IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdIronId, Name = "Concentrate 65% Fe", Sku = "FE-CONC-65", PurityGrade = "65%", Packaging = PackagingType.Bulk, Price = 105m, PriceCurrency = Currency.USD, AvailableQuantity = 15000m, UnitOfMeasure = "MT", IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdBitumenId, Name = "Drummed – New Steel", Sku = "BIT-DR-NEW", PurityGrade = "60/70", Packaging = PackagingType.Drummed, Price = 380m, PriceCurrency = Currency.USD, AvailableQuantity = 5000m, UnitOfMeasure = "MT", IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdBitumenId, Name = "Bulk – Tanker", Sku = "BIT-BULK", PurityGrade = "60/70", Packaging = PackagingType.Tanker, Price = 350m, PriceCurrency = Currency.USD, AvailableQuantity = 10000m, UnitOfMeasure = "MT", IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdSaffronId, Name = "250g Gift Box", Sku = "SAF-250G", PurityGrade = "Super Negin", Packaging = PackagingType.Bottled, Price = 1500m, PriceCurrency = Currency.USD, AvailableQuantity = 50m, UnitOfMeasure = "KG", IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdPPId, Name = "HP500J – Injection", Sku = "PP-HP500J", PurityGrade = "Injection", Packaging = PackagingType.Bagged, Price = 1200m, PriceCurrency = Currency.USD, AvailableQuantity = 2000m, UnitOfMeasure = "MT", IsActive = true, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdPPId, Name = "HP510L – Film", Sku = "PP-HP510L", PurityGrade = "Film", Packaging = PackagingType.Bagged, Price = 1220m, PriceCurrency = Currency.USD, AvailableQuantity = 1500m, UnitOfMeasure = "MT", IsActive = true, CreatedAt = Now, CreatedBy = "System" },
        };
        context.ProductVariants.AddRange(variants);

        // ── Certifications ──
        var certs = new List<ProductCertification>
        {
            new() { Id = Guid.NewGuid(), ProductId = ProdCopperId,  CertificationType = "LME Grade A",   CertificationBody = "London Metal Exchange", IsVerified = true, IssuedAt = Now.AddMonths(-12), ExpiresAt = Now.AddMonths(12), CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdCopperId,  CertificationType = "ISO 9001:2015", CertificationBody = "SGS",                   IsVerified = true, IssuedAt = Now.AddMonths(-6), ExpiresAt = Now.AddMonths(30), CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdIronId,    CertificationType = "ISO 9001:2015", CertificationBody = "Bureau Veritas",         IsVerified = true, IssuedAt = Now.AddMonths(-8), ExpiresAt = Now.AddMonths(28), CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdSaffronId, CertificationType = "ISO 3632",      CertificationBody = "ISIRI",                  IsVerified = true, IssuedAt = Now.AddMonths(-3), ExpiresAt = Now.AddMonths(9),  CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdSaffronId, CertificationType = "Organic",       CertificationBody = "IFOAM / IMO",           IsVerified = true, IssuedAt = Now.AddMonths(-2), ExpiresAt = Now.AddMonths(10), CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdUreaId,    CertificationType = "REACH",         CertificationBody = "ECHA",                   IsVerified = true, IssuedAt = Now.AddMonths(-4), ExpiresAt = Now.AddMonths(20), CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), ProductId = ProdPPId,      CertificationType = "ISO 9001:2015", CertificationBody = "TÜV",                    IsVerified = true, IssuedAt = Now.AddMonths(-5), ExpiresAt = Now.AddMonths(31), CreatedAt = Now, CreatedBy = "System" },
        };
        context.ProductCertifications.AddRange(certs);

        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} products with attributes, variants & certifications", products.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  11. LISTINGS
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedListingsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Listings.AnyAsync(l => l.TenantId == TenantId))
            return;

        var listings = new List<Listing>
        {
            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = SellerCompanyId, ProductId = ProdCopperId,
                Type = ListingType.Spot, Status = ListingStatus.Active,
                Title = "Copper Cathode Grade A – Immediate Delivery",
                Description = "200 MT copper cathode available ex-warehouse Bandar Abbas",
                Quantity = 200m, UnitOfMeasure = "MT", Price = 8950m, PriceCurrency = Currency.USD, PriceUnit = "/MT",
                MinOrderQuantity = 25m, Incoterm = Incoterm.FOB, DeliveryLocation = "Bandar Abbas, Iran",
                LeadTimeDays = 7, ExpiresAt = Now.AddDays(30), CreatedAt = Now.AddDays(-2), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = SellerCompanyId, ProductId = ProdIronId,
                Type = ListingType.ForwardContract, Status = ListingStatus.Active,
                Title = "Iron Ore Concentrate 67% Fe – Q2 2026",
                Description = "25,000 MT iron ore concentrate for Q2 delivery. Gol-e-Gohar mine origin.",
                Quantity = 25000m, UnitOfMeasure = "MT", Price = 120m, PriceCurrency = Currency.USD, PriceUnit = "/MT",
                MinOrderQuantity = 5000m, Incoterm = Incoterm.CFR, DeliveryLocation = "Bandar Abbas, Iran",
                LeadTimeDays = 30, DeliveryStartDate = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                DeliveryEndDate = new DateTime(2026, 6, 30, 0, 0, 0, DateTimeKind.Utc),
                ExpiresAt = Now.AddDays(45), CreatedAt = Now.AddDays(-5), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = SellerCompanyId, ProductId = ProdSodaAshId,
                Type = ListingType.Spot, Status = ListingStatus.Active,
                Title = "Soda Ash Dense 99.2% – Ready Stock",
                Description = "3,000 MT soda ash dense in 50kg bags. Kaveh Soda Ash plant.",
                Quantity = 3000m, UnitOfMeasure = "MT", Price = 280m, PriceCurrency = Currency.USD, PriceUnit = "/MT",
                MinOrderQuantity = 20m, Incoterm = Incoterm.EXW, DeliveryLocation = "Maragheh, Iran",
                LeadTimeDays = 5, ExpiresAt = Now.AddDays(20), CreatedAt = Now.AddDays(-1), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = TraderCompanyId, ProductId = ProdBitumenId,
                Type = ListingType.Spot, Status = ListingStatus.Active,
                Title = "Bitumen 60/70 – New Steel Drums",
                Description = "5,000 MT penetration grade bitumen in new steel drums. Jey Oil refinery.",
                Quantity = 5000m, UnitOfMeasure = "MT", Price = 380m, PriceCurrency = Currency.USD, PriceUnit = "/MT",
                MinOrderQuantity = 100m, Incoterm = Incoterm.FOB, DeliveryLocation = "Bandar Abbas, Iran",
                LeadTimeDays = 10, ExpiresAt = Now.AddDays(25), CreatedAt = Now.AddDays(-3), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = TraderCompanyId, ProductId = ProdSaffronId,
                Type = ListingType.Spot, Status = ListingStatus.Active,
                Title = "Super Negin Saffron – Export Quality",
                Description = "50 KG premium Super Negin saffron. ISO 3632 certified. Qaen origin.",
                Quantity = 50m, UnitOfMeasure = "KG", Price = 1450m, PriceCurrency = Currency.USD, PriceUnit = "/KG",
                MinOrderQuantity = 1m, Incoterm = Incoterm.CIF, DeliveryLocation = "IKA Airport, Tehran",
                LeadTimeDays = 3, ExpiresAt = Now.AddDays(15), CreatedAt = Now, CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = SellerCompanyId, ProductId = ProdChromiteId,
                Type = ListingType.FrameworkContract, Status = ListingStatus.Active,
                Title = "Chromite Ore 42% – Annual Supply 2026",
                Description = "Up to 20,000 MT/year chromite ore. Monthly shipments from Sabzevar mine.",
                Quantity = 20000m, UnitOfMeasure = "MT", Price = 290m, PriceCurrency = Currency.USD, PriceUnit = "/MT",
                MinOrderQuantity = 500m, Incoterm = Incoterm.FOB, DeliveryLocation = "Bandar Abbas, Iran",
                LeadTimeDays = 21, DeliveryStartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                DeliveryEndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                ExpiresAt = Now.AddDays(60), CreatedAt = Now.AddDays(-7), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = SellerCompanyId, ProductId = ProdUreaId,
                Type = ListingType.Spot, Status = ListingStatus.Active,
                Title = "Urea Granular 46% – 10,000 MT Available",
                Description = "Prilled urea from Pardis Petrochemical. 50kg PP bags.",
                Quantity = 10000m, UnitOfMeasure = "MT", Price = 320m, PriceCurrency = Currency.USD, PriceUnit = "/MT",
                MinOrderQuantity = 500m, Incoterm = Incoterm.FOB, DeliveryLocation = "Imam Khomeini Port, Iran",
                LeadTimeDays = 14, ExpiresAt = Now.AddDays(35), CreatedAt = Now.AddDays(-4), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = TraderCompanyId, ProductId = ProdPPId,
                Type = ListingType.Spot, Status = ListingStatus.Active,
                Title = "Polypropylene HP500J – Jam Petrochemical",
                Description = "2,000 MT PP homopolymer injection grade. 25kg bags on pallets.",
                Quantity = 2000m, UnitOfMeasure = "MT", Price = 1200m, PriceCurrency = Currency.USD, PriceUnit = "/MT",
                MinOrderQuantity = 20m, Incoterm = Incoterm.CPT, DeliveryLocation = "Assaluyeh, Iran",
                LeadTimeDays = 7, ExpiresAt = Now.AddDays(20), CreatedAt = Now.AddDays(-1), CreatedBy = "System" },
        };

        context.Listings.AddRange(listings);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} listings", listings.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  12. WAREHOUSES & INVENTORY
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedWarehousesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Warehouses.AnyAsync(w => w.TenantId == TenantId))
            return;

        var warehouses = new List<Warehouse>
        {
            new() { Id = WarehouseTehranId, TenantId = TenantId, CompanyId = BuyerCompanyId,
                Name = "انبار مرکزی تهران", Code = "WH-THR-01",
                AddressLine1 = "جاده مخصوص کرج، کیلومتر ۲۰", City = "تهران", Country = "IR",
                ContactPhone = "+98-21-44001122", CapacityTons = 5000m, IsActive = true,
                CreatedAt = Now.AddMonths(-6), CreatedBy = "System" },

            new() { Id = WarehouseBandarId, TenantId = TenantId, CompanyId = SellerCompanyId,
                Name = "انبار بندرعباس", Code = "WH-BND-01",
                AddressLine1 = "منطقه ویژه اقتصادی بندرعباس", City = "بندرعباس", Country = "IR",
                ContactPhone = "+98-76-33445566", CapacityTons = 25000m, IsActive = true,
                CreatedAt = Now.AddMonths(-8), CreatedBy = "System" },

            new() { Id = WarehouseIsfahanId, TenantId = TenantId, CompanyId = SellerCompanyId,
                Name = "انبار اصفهان", Code = "WH-ISF-01",
                AddressLine1 = "شهرک صنعتی جی اصفهان", City = "اصفهان", Country = "IR",
                ContactPhone = "+98-31-36112233", CapacityTons = 10000m, IsActive = true,
                CreatedAt = Now.AddMonths(-8), CreatedBy = "System" },
        };

        context.Warehouses.AddRange(warehouses);
        await context.SaveChangesAsync();

        // ── Inventory ──
        var inventory = new List<InventoryItem>
        {
            new() { Id = Guid.NewGuid(), TenantId = TenantId, WarehouseId = WarehouseBandarId, ProductId = ProdCopperId,
                Quantity = 350m, UnitOfMeasure = "MT", ReservedQuantity = 150m,
                CreatedAt = Now.AddDays(-10), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, WarehouseId = WarehouseBandarId, ProductId = ProdIronId,
                Quantity = 30000m, UnitOfMeasure = "MT", ReservedQuantity = 5000m,
                CreatedAt = Now.AddDays(-15), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, WarehouseId = WarehouseIsfahanId, ProductId = ProdSodaAshId,
                Quantity = 4500m, UnitOfMeasure = "MT", ReservedQuantity = 1500m,
                CreatedAt = Now.AddDays(-5), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, WarehouseId = WarehouseBandarId, ProductId = ProdBitumenId,
                Quantity = 8000m, UnitOfMeasure = "MT", ReservedQuantity = 3000m,
                CreatedAt = Now.AddDays(-8), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, WarehouseId = WarehouseIsfahanId, ProductId = ProdChromiteId,
                Quantity = 12000m, UnitOfMeasure = "MT", ReservedQuantity = 0m,
                CreatedAt = Now.AddDays(-20), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, WarehouseId = WarehouseTehranId, ProductId = ProdSaffronId,
                Quantity = 80m, UnitOfMeasure = "KG", ReservedQuantity = 30m,
                CreatedAt = Now.AddDays(-3), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, WarehouseId = WarehouseBandarId, ProductId = ProdUreaId,
                Quantity = 15000m, UnitOfMeasure = "MT", ReservedQuantity = 5000m,
                CreatedAt = Now.AddDays(-12), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, WarehouseId = WarehouseBandarId, ProductId = ProdPPId,
                Quantity = 3500m, UnitOfMeasure = "MT", ReservedQuantity = 1500m,
                CreatedAt = Now.AddDays(-6), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, WarehouseId = WarehouseIsfahanId, ProductId = ProdZincId,
                Quantity = 800m, UnitOfMeasure = "MT", ReservedQuantity = 200m,
                CreatedAt = Now.AddDays(-9), CreatedBy = "System" },
        };

        context.InventoryItems.AddRange(inventory);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} warehouses with {InvCount} inventory items", warehouses.Count, inventory.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  13. COMMISSION RULES
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedCommissionRulesAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.CommissionRules.AnyAsync())
            return;

        var rules = new List<CommissionRule>
        {
            new() { Id = Guid.NewGuid(), Name = "Standard Commission – Metals",          Type = CommissionType.Percentage, Value = 1.5m,  CategoryId = CatMetalsId,    MinTransactionAmount = 10000m, Currency = Currency.USD, IsActive = true, Priority = 1, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), Name = "Standard Commission – Chemicals",       Type = CommissionType.Percentage, Value = 2.0m,  CategoryId = CatChemicalsId, MinTransactionAmount = 5000m,  Currency = Currency.USD, IsActive = true, Priority = 1, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), Name = "Standard Commission – Minerals",        Type = CommissionType.Percentage, Value = 1.75m, CategoryId = CatMineralsId,  MinTransactionAmount = 5000m,  Currency = Currency.USD, IsActive = true, Priority = 1, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), Name = "Standard Commission – Agriculture",     Type = CommissionType.Percentage, Value = 2.5m,  CategoryId = CatAgriId,      MinTransactionAmount = 1000m,  Currency = Currency.USD, IsActive = true, Priority = 1, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), Name = "Standard Commission – Energy",          Type = CommissionType.Percentage, Value = 1.0m,  CategoryId = CatEnergyId,    MinTransactionAmount = 25000m, Currency = Currency.USD, IsActive = true, Priority = 1, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), Name = "Standard Commission – Polymers",        Type = CommissionType.Percentage, Value = 1.5m,  CategoryId = CatPolymersId,  MinTransactionAmount = 10000m, Currency = Currency.USD, IsActive = true, Priority = 1, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), Name = "Small Order Flat Fee",                  Type = CommissionType.FixedAmount,Value = 50m,   CategoryId = null,           MaxTransactionAmount = 5000m,  Currency = Currency.USD, IsActive = true, Priority = 10, CreatedAt = Now, CreatedBy = "System" },
            new() { Id = Guid.NewGuid(), Name = "High-Value Discount (>$500k)",          Type = CommissionType.Percentage, Value = 0.75m, CategoryId = null,           MinTransactionAmount = 500000m,Currency = Currency.USD, IsActive = true, Priority = 0,  CreatedAt = Now, CreatedBy = "System" },
        };

        context.CommissionRules.AddRange(rules);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} commission rules", rules.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  14. FEATURE FLAGS
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedFeatureFlagsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.FeatureFlags.AnyAsync())
            return;

        var flags = new List<FeatureFlag>
        {
            Flag("auction-module",          "Live Auction Module",                     true),
            Flag("rfq-module",              "Request for Quotation Module",            true),
            Flag("escrow-payments",         "Escrow Payment System",                   true),
            Flag("real-time-pricing",       "Real-Time Commodity Price Feed",          true),
            Flag("ai-price-prediction",     "AI-Powered Price Predictions",            false),
            Flag("blockchain-verification", "Blockchain Document Verification",        false),
            Flag("biometric-kyc",           "Biometric KYC Verification",              false),
            Flag("multi-language",          "Multi-Language Support (FA/EN)",           true),
            Flag("dark-mode",              "Dark Mode Theme",                          true),
            Flag("push-notifications",      "Push Notification Delivery",              true),
            Flag("sms-notifications",       "SMS Notification Delivery",               true),
            Flag("graphql-api",            "GraphQL API Endpoint",                     false),
            Flag("export-pdf",             "PDF Export for Contracts & Invoices",      true),
            Flag("sanctions-screening",     "Automated Sanctions Screening",           true),
            Flag("chat-module",            "In-Platform Chat & Messaging",             true),
            Flag("mobile-app",             "Mobile App Access",                        false),
        };

        context.FeatureFlags.AddRange(flags);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} feature flags", flags.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  15. KYC / KYB VERIFICATION RECORDS
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedKycKybAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.KycVerifications.AnyAsync(k => k.TenantId == TenantId))
            return;

        // KYC for each demo user
        var kycRecords = new List<KycVerification>
        {
            new() { Id = Guid.NewGuid(), TenantId = TenantId, UserId = BuyerUserId,
                Status = VerificationStatus.Approved, FullName = "علی محمدی", NationalId = "0012345678",
                Nationality = "IR", DateOfBirth = new DateTime(1988, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                City = "تهران", Country = "IR", ReviewedBy = "admin@rawnex.com", ReviewedAt = Now.AddDays(-25),
                CreatedAt = Now.AddDays(-28), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, UserId = SellerUserId,
                Status = VerificationStatus.Approved, FullName = "فاطمه رضایی", NationalId = "0023456789",
                Nationality = "IR", DateOfBirth = new DateTime(1992, 7, 22, 0, 0, 0, DateTimeKind.Utc),
                City = "اصفهان", Country = "IR", ReviewedBy = "admin@rawnex.com", ReviewedAt = Now.AddDays(-50),
                CreatedAt = Now.AddDays(-55), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, UserId = BuyerManagerId,
                Status = VerificationStatus.Approved, FullName = "حسین احمدی", NationalId = "0034567890",
                Nationality = "IR", DateOfBirth = new DateTime(1985, 11, 5, 0, 0, 0, DateTimeKind.Utc),
                City = "تهران", Country = "IR", ReviewedBy = "admin@rawnex.com", ReviewedAt = Now.AddDays(-20),
                CreatedAt = Now.AddDays(-22), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, UserId = SellerManagerId,
                Status = VerificationStatus.Approved, FullName = "مریم کریمی", NationalId = "0045678901",
                Nationality = "IR", DateOfBirth = new DateTime(1990, 1, 18, 0, 0, 0, DateTimeKind.Utc),
                City = "اصفهان", Country = "IR", ReviewedBy = "admin@rawnex.com", ReviewedAt = Now.AddDays(-45),
                CreatedAt = Now.AddDays(-48), CreatedBy = "System" },
        };

        context.KycVerifications.AddRange(kycRecords);

        // KYB for each company
        var kybRecords = new List<KybVerification>
        {
            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = BuyerCompanyId,
                Status = VerificationStatus.Approved,
                ReviewedBy = "admin@rawnex.com", ReviewedAt = Now.AddDays(-28),
                CreatedAt = Now.AddMonths(-5), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = SellerCompanyId,
                Status = VerificationStatus.Approved,
                ReviewedBy = "admin@rawnex.com", ReviewedAt = Now.AddDays(-55),
                CreatedAt = Now.AddMonths(-7), CreatedBy = "System" },

            new() { Id = Guid.NewGuid(), TenantId = TenantId, CompanyId = TraderCompanyId,
                Status = VerificationStatus.Approved,
                ReviewedBy = "admin@rawnex.com", ReviewedAt = Now.AddDays(-12),
                CreatedAt = Now.AddMonths(-2), CreatedBy = "System" },
        };

        context.KybVerifications.AddRange(kybRecords);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} KYC + {KybCount} KYB records", kycRecords.Count, kybRecords.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  16. SAMPLE NOTIFICATIONS
    // ═══════════════════════════════════════════════════════════
    public static async Task SeedNotificationsAsync(ApplicationDbContext context, ILogger logger)
    {
        if (await context.Notifications.AnyAsync(n => n.TenantId == TenantId))
            return;

        var notifications = new List<Notification>
        {
            Notif(AdminUserId, NotificationType.System,           NotificationPriority.Normal, "خوش آمدید به راونکس",   "پلتفرم معاملات مواد اولیه آماده استفاده است.",             "/dashboard",        0),
            Notif(BuyerUserId, NotificationType.System,           NotificationPriority.Normal, "خوش آمدید",             "حساب کاربری شما با موفقیت فعال شد.",                       "/dashboard",        0),
            Notif(BuyerUserId, NotificationType.PriceAlert,       NotificationPriority.High,   "تغییر قیمت مس",        "قیمت کاتد مس ۲.۵٪ افزایش یافت و به $8,950/MT رسید.",      "/products/" + ProdCopperId, 1),
            Notif(BuyerUserId, NotificationType.RfqReceived,      NotificationPriority.Normal, "پاسخ جدید به درخواست",  "یک پیشنهاد جدید برای درخواست خرید سنگ آهن دریافت شد.",      "/rfq",              2),
            Notif(SellerUserId,NotificationType.OrderUpdate,      NotificationPriority.High,   "سفارش جدید",           "سفارش خرید ۵۰۰ تن کاتد مس از شرکت فولاد پارسیان ثبت شد.", "/orders",           1),
            Notif(SellerUserId,NotificationType.PaymentUpdate,    NotificationPriority.Urgent, "پرداخت دریافت شد",     "مبلغ $4,475,000 به حساب امانی واریز شد.",                   "/payments",         2),
            Notif(SellerUserId,NotificationType.ShipmentUpdate,   NotificationPriority.Normal, "به‌روزرسانی حمل",       "محموله #SH-2026-001 به بندرعباس رسید.",                    "/shipments",        3),
            Notif(BuyerManagerId,NotificationType.ApprovalRequired,NotificationPriority.Urgent,"نیاز به تأیید",        "سفارش ۱۰,۰۰۰ تن سنگ آهن نیاز به تأیید مدیر دارد.",       "/orders",           0),
        };

        context.Notifications.AddRange(notifications);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} notifications", notifications.Count);
    }

    // ═══════════════════════════════════════════════════════════
    //  HELPER METHODS
    // ═══════════════════════════════════════════════════════════
    private static Product BuildProduct(Guid id, Guid companyId, Guid categoryId,
        string name, string nameFa, string slug,
        string desc, string descFa,
        string sku, string? casNumber, string purityGrade, string origin,
        PackagingType packaging, string uom, decimal minQty, decimal maxQty,
        decimal price, Currency currency, string priceUnit, ProductStatus status)
    {
        return new Product
        {
            Id = id,
            TenantId = TenantId,
            CompanyId = companyId,
            CategoryId = categoryId,
            Name = name,
            NameFa = nameFa,
            Slug = slug,
            Description = desc,
            DescriptionFa = descFa,
            Sku = sku,
            CasNumber = casNumber,
            PurityGrade = purityGrade,
            Origin = origin,
            Packaging = packaging,
            UnitOfMeasure = uom,
            MinOrderQuantity = minQty,
            MaxOrderQuantity = maxQty,
            BasePrice = price,
            PriceCurrency = currency,
            PriceUnit = priceUnit,
            Status = status,
            Version = 1,
            CreatedAt = Now.AddMonths(-3),
            CreatedBy = "System"
        };
    }

    private static ProductAttribute Attr(Guid productId, string key, string value, string unit, int order) =>
        new() { Id = Guid.NewGuid(), ProductId = productId, Key = key, Value = value, Unit = unit, SortOrder = order };

    private static FeatureFlag Flag(string key, string desc, bool enabled) =>
        new() { Id = Guid.NewGuid(), Key = key, Description = desc, IsEnabled = enabled, CreatedAt = Now, CreatedBy = "System" };

    private static Notification Notif(Guid userId, NotificationType type, NotificationPriority priority,
        string title, string message, string actionUrl, int daysAgo) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TenantId = TenantId,
            Type = type,
            Priority = priority,
            Title = title,
            Message = message,
            ActionUrl = actionUrl,
            IsRead = daysAgo > 2,
            CreatedAt = Now.AddDays(-daysAgo)
        };
}
