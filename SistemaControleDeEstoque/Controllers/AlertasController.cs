using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Controllers
{
    /// <summary>
    /// Controller responsável pela exibição de alertas de estoque baixo.
    /// Apenas Gerente e Admin podem acessar.
    /// </summary>
    [Authorize(Roles = "Admin,Gerente")]
    public class AlertasController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        /// <summary>
        /// GET: Alertas/Index
        /// Exibe a lista de produtos com estoque abaixo do nível de segurança,
        /// ordenados do mais crítico (menor quantidade) para o menos crítico.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var produtosComAlerta = await _context.Produto
                .Include(p => p.Fornecedor)
                .Where(p => p.Quantidade < p.EstoqueSeguranca)
                .OrderBy(p => p.Quantidade)
                .ToListAsync();

            return View(produtosComAlerta);
        }
    }
}
