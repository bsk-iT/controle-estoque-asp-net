using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace SistemaControleDeEstoque.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public SeedUserRoleInitial(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            if (!await _roleManager.RoleExistsAsync("Gerente"))
                await _roleManager.CreateAsync(new IdentityRole("Gerente"));

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        public async Task SeedUsersAsync()
        {
            var userPassword = _configuration["SeedUsers:UserPassword"]
                ?? throw new InvalidOperationException("Senha do usuário seed não configurada em SeedUsers:UserPassword.");
            var gerentePassword = _configuration["SeedUsers:GerentePassword"]
                ?? throw new InvalidOperationException("Senha do gerente seed não configurada em SeedUsers:GerentePassword.");
            var adminPassword = _configuration["SeedUsers:AdminPassword"]
                ?? throw new InvalidOperationException("Senha do admin seed não configurada em SeedUsers:AdminPassword.");

            if (await _userManager.FindByEmailAsync("usuario@localhost") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "usuario@localhost",
                    Email = "usuario@localhost",
                    EmailConfirmed = true,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(user, "User");
            }

            if (await _userManager.FindByEmailAsync("gerente@localhost") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "gerente@localhost",
                    Email = "gerente@localhost",
                    EmailConfirmed = true,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, gerentePassword);
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(user, "Gerente");
            }

            if (await _userManager.FindByEmailAsync("admin@localhost") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin@localhost",
                    Email = "admin@localhost",
                    EmailConfirmed = true,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
