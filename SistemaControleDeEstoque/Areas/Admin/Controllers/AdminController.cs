using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaControleDeEstoque.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Obter contagem de usuários cadastrados
            var totalUsuarios = await _userManager.Users.CountAsync();

            // Obter contagem de produtos cadastrados
            var totalProdutos = await _context.Produto.CountAsync();

            // Obter contagem de operações (movimentações) realizadas hoje
            var hoje = DateTime.Today;
            var amanha = hoje.AddDays(1);
            var operacoesHoje = await _context.Movimentacao
                .Where(m => m.DataMovimentacao >= hoje && m.DataMovimentacao < amanha)
                .CountAsync();

            // Passar os dados para a view através do ViewBag
            ViewBag.TotalUsuarios = totalUsuarios;
            ViewBag.TotalProdutos = totalProdutos;
            ViewBag.OperacoesHoje = operacoesHoje;

            return View();
        }
    }
}