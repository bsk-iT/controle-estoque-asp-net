using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Obter dados para a visão geral do estoque

            // 1. Total de produtos cadastrados
            var totalProdutos = await _context.Produto.CountAsync();

            // 2. Valor total do estoque (quantidade * valor de cada produto)
            var valorTotalEstoque = await _context.Produto.SumAsync(p => p.Quantidade * p.Valor);

            // 3. Produtos com estoque abaixo do estoque de segurança
            var produtosEstoqueBaixo = await _context.Produto
                .Where(p => p.Quantidade < p.EstoqueSeguranca)
                .CountAsync();

            // Passar os dados para a view usando ViewBag
            ViewBag.TotalProdutos = totalProdutos;
            ViewBag.ValorTotalEstoque = valorTotalEstoque;
            ViewBag.ProdutosEstoqueBaixo = produtosEstoqueBaixo;

            // Variação mensal - exemplo estático, mas poderia ser calculado dinamicamente
            // comparando com dados do mês anterior armazenados no banco de dados
            ViewBag.VariacaoTotalProdutos = 0; // 0% de variação (sem alteração)
            ViewBag.VariacaoValorEstoque = 0;  // 0% de variação (sem alteração)

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}