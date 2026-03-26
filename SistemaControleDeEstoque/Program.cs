using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Services;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

// Garantir que a pasta logs/ existe antes de qualquer operação
var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logsDirectory))
{
    Directory.CreateDirectory(logsDirectory);
}

// Configurar Serilog o mais cedo possível para capturar erros de inicialização
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Iniciando aplicação Sistema de Controle de Estoque...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Substituir o sistema de logging padrão pelo Serilog, lendo config do appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Persistir as chaves do Data Protection no banco de dados.
    // Sem isso, a cada reinício do servidor as chaves são geradas em memória
    // e tokens antiforgery / cookies de sessão antigos ficam inválidos → erro 500.
    builder.Services.AddDataProtection()
        .PersistKeysToDbContext<ApplicationDbContext>()
        .SetApplicationName("SistemaControleDeEstoque");

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    }

    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddErrorDescriber<TraduzirErrosIdentity>()
        .AddDefaultTokenProviders();

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { new CultureInfo("pt-BR") };
        options.DefaultRequestCulture = new RequestCulture("pt-BR");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
    });

    builder.Services.AddControllersWithViews()
        .AddViewLocalization()
        .AddDataAnnotationsLocalization();

    builder.Services.AddRazorPages();

    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        options.User.RequireUniqueEmail = true;
    });

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

    builder.Services.AddAuthorizationBuilder()
        .AddPolicy("RequireUserAdminGerenteRole", policy => policy.RequireRole("User", "Gerente", "Admin"))
        .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

    builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();

    builder.Services.AddTransient<IEmailSender, EmailSender>();

    var app = builder.Build();

    // Aplicar migrations e seed automaticamente ao iniciar
    // Em produção, garante que o banco esteja atualizado sem precisar rodar comandos manuais
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Aplicando {Count} migration(s) pendente(s)...", pendingMigrations.Count());
                await db.Database.MigrateAsync();
                logger.LogInformation("Migrations aplicadas com sucesso.");
            }

            var seedRoleService = scope.ServiceProvider.GetRequiredService<ISeedUserRoleInitial>();
            await seedRoleService.SeedRolesAsync();
            await seedRoleService.SeedUsersAsync();
            logger.LogInformation("Seed de roles e usuários concluído.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao aplicar migrations ou seed na inicialização.");
            throw;
        }
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    // Middleware de logging de requisições HTTP (Serilog)
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000} ms";
    });

    app.UseRequestLocalization();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "MinhaArea",
        pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();

    Log.Information("Aplicação iniciada com sucesso.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada inesperadamente.");
}
finally
{
    // Garante que todos os logs sejam gravados antes de encerrar
    await Log.CloseAndFlushAsync();
}
