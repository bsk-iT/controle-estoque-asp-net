using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<TraduzirErrosIdentity>()
    .AddDefaultTokenProviders();

// Configura��o de localiza��o para pt-BR
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
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireUserAdminGerenteRole", policy => policy.RequireRole("User", "Gerente", "Admin"));

builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
builder.Services.AddScoped<ISeedDataInitial, SeedDataInitial>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Usar a configura��o de localiza��o
app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Seed executado após o pipeline de autenticação/autorização estar configurado
await CriarPerfisUsuariosAsync(app);
await PopularDadosAsync(app);

app.MapControllerRoute(
    name: "MinhaArea",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

static async Task CriarPerfisUsuariosAsync(WebApplication app)
{
    // Obt�m o escopo de servi�os para resolver depend�ncias
    var scopeFactory = app.Services.GetService<IServiceScopeFactory>() ?? throw new InvalidOperationException("IServiceScopeFactory n�o est� registrado nos servi�os.");
    using var serviceScope = scopeFactory.CreateScope();
    // Obt�m o servi�o ISeedUserRoleInitial do escopo de servi�os
    var service = serviceScope.ServiceProvider.GetService<ISeedUserRoleInitial>() ?? throw new InvalidOperationException("ISeedUserRoleInitial n�o est� registrado nos servi�os.");
    await service.SeedRolesAsync();
    await service.SeedUsersAsync();
}

static async Task PopularDadosAsync(WebApplication app)
{
    var scopeFactory = app.Services.GetService<IServiceScopeFactory>() ?? throw new InvalidOperationException("IServiceScopeFactory n�o est� registrado nos servi�os.");
    using var serviceScope = scopeFactory.CreateScope();
    var service = serviceScope.ServiceProvider.GetService<ISeedDataInitial>() ?? throw new InvalidOperationException("ISeedDataInitial n�o est� registrado nos servi�os.");
    await service.SeedFornecedoresAsync();
    await service.SeedProdutosAsync();
}