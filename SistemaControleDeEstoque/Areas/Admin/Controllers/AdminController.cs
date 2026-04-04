using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaControleDeEstoque.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        public async Task<IActionResult> Index()
        {
            var hoje = DateTime.Today;
            var amanha = hoje.AddDays(1);

            var vm = new AdminDashboardViewModel
            {
                TotalUsuarios = await _userManager.Users.CountAsync(),
                TotalProdutos = await _context.Produto.CountAsync(),
                OperacoesHoje = await _context.Movimentacao
                    .Where(m => m.DataMovimentacao >= hoje && m.DataMovimentacao < amanha)
                    .CountAsync()
            };

            return View(vm);
        }
    }
}
