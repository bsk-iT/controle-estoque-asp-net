using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;
using SistemaControleDeEstoque.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaControleDeEstoque.Controllers
{
    [Authorize]
    public class RelatoriosController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        // GET: Relatorios
        public async Task<IActionResult> Index()
        {
            var relatorios = await _context.Relatorio.ToListAsync();

            // Filtrar por usuário se não for Admin ou Gerente (previne IDOR)
            if (!User.IsInRole("Admin") && !User.IsInRole("Gerente"))
            {
                var user = await _userManager.GetUserAsync(User);
                relatorios = relatorios.Where(r => r.UsuarioGerador == user?.UserName).ToList();
            }

            return View(relatorios);
        }

        // GET: Relatorios/Details/5
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatorio = await _context.Relatorio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (relatorio == null)
            {
                return NotFound();
            }

            // Controle de ownership: apenas Admin/Gerente ou o próprio usuário podem ver
            if (!User.IsInRole("Admin") && !User.IsInRole("Gerente"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (relatorio.UsuarioGerador != user?.UserName)
                {
                    return NotFound(); // 404 em vez de 403 para não revelar que o registro existe
                }
            }

            return View(relatorio);
        }

        // GET: Relatorios/Visualizar/5
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Visualizar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatorio = await _context.Relatorio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (relatorio == null)
            {
                return NotFound();
            }

            switch (relatorio.Tipo)
            {
                case TipoRelatorio.Estoque:
                    var produtos = await _context.Produto
                        .Include(p => p.Fornecedor)
                        .ToListAsync();
                    ViewBag.Produtos = produtos;
                    ViewBag.DataInicio = relatorio.DataInicio ?? DateTime.MinValue;
                    ViewBag.DataFim = relatorio.DataFim ?? DateTime.Now;
                    break;

                case TipoRelatorio.Movimentacoes:
                    var movimentacoes = await _context.Movimentacao
                        .Include(m => m.Produto)
                        .Where(m =>
                            (!relatorio.DataInicio.HasValue || m.DataMovimentacao >= relatorio.DataInicio) &&
                            (!relatorio.DataFim.HasValue || m.DataMovimentacao <= relatorio.DataFim))
                        .ToListAsync();
                    ViewBag.Movimentacoes = movimentacoes;
                    ViewBag.DataInicio = relatorio.DataInicio ?? DateTime.MinValue;
                    ViewBag.DataFim = relatorio.DataFim ?? DateTime.Now;
                    break;
            }

            return View(relatorio);
        }

        // GET: Relatorios/Create
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            var vm = new RelatorioCreateViewModel
            {
                DataGeracao = DateTime.Now,
                UsuarioGerador = user?.UserName ?? "Usuário não identificado"
            };

            return View(vm);
        }

        // POST: Relatorios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Create(RelatorioCreateViewModel vm)
        {
            // Garantir que o usuário gerador seja o usuário logado (previne overposting)
            var user = await _userManager.GetUserAsync(User);
            vm.UsuarioGerador = user?.UserName ?? "Usuário não identificado";
            vm.DataGeracao = DateTime.Now;

            // Se data final não foi informada, usar data atual
            if (!vm.DataFim.HasValue)
            {
                vm.DataFim = DateTime.Now;
            }

            if (ModelState.IsValid)
            {
                var relatorio = new Relatorio
                {
                    Tipo = vm.Tipo,
                    DataGeracao = vm.DataGeracao,
                    UsuarioGerador = vm.UsuarioGerador,
                    DataInicio = vm.DataInicio,
                    DataFim = vm.DataFim,
                    FornecedorId = vm.FornecedorId,
                    ProdutoId = vm.ProdutoId,
                    IncluirProdutosZerados = vm.IncluirProdutosZerados,
                    Ordenacao = vm.Ordenacao,
                    ApenasAbaixoDoMinimo = vm.ApenasAbaixoDoMinimo,
                    TipoMovimentacao = vm.TipoMovimentacao,
                    Resumido = vm.Resumido
                };

                switch (relatorio.Tipo)
                {
                    case TipoRelatorio.Estoque:
                        TempData["MensagemSucesso"] = "Relatório de Inventário de Estoque gerado com sucesso!";
                        break;
                    case TipoRelatorio.Movimentacoes:
                        TempData["MensagemSucesso"] = "Relatório de Movimentações gerado com sucesso!";
                        break;
                }

                _context.Add(relatorio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        // GET: Relatorios/Edit/5
        [Authorize(Roles = "Admin,Gerente")]
        public IActionResult Edit(int? id)
        {
            TempData["Mensagem"] = "A edição de relatórios não é permitida. Relatórios são registros históricos imutáveis.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Relatorios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public IActionResult Edit(int id, RelatorioCreateViewModel vm)
        {
            TempData["Mensagem"] = "A edição de relatórios não é permitida. Relatórios são registros históricos imutáveis.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Relatorios/Delete/5
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relatorio = await _context.Relatorio
                .FirstOrDefaultAsync(m => m.Id == id);
            if (relatorio == null)
            {
                return NotFound();
            }

            return View(relatorio);
        }

        // POST: Relatorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var relatorio = await _context.Relatorio.FindAsync(id);
            if (relatorio != null)
            {
                _context.Relatorio.Remove(relatorio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RelatorioExistsAsync(int id)
        {
            return await _context.Relatorio.AnyAsync(e => e.Id == id);
        }
    }
}
