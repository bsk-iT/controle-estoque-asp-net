using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaControleDeEstoque.Controllers
{
    [Authorize]
    public class RelatoriosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RelatoriosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Relatorios
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Relatorio.ToListAsync());
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

            // Dependendo do tipo de relatório, vamos buscar dados diferentes
            switch (relatorio.Tipo)
            {
                case TipoRelatorio.Estoque:
                    // Buscar produtos no estoque
                    var produtos = await _context.Produto
                        .Include(p => p.Fornecedor)
                        .ToListAsync();

                    ViewBag.Produtos = produtos;
                    ViewBag.DataInicio = relatorio.DataInicio ?? DateTime.MinValue;
                    ViewBag.DataFim = relatorio.DataFim ?? DateTime.Now;
                    break;

                case TipoRelatorio.Movimentacoes:
                    // Buscar movimentações
                    var movimentacoes = await _context.Movimentacao
                        .Include(m => m.Produto)
                        .Where(m =>
                            (!relatorio.DataInicio.HasValue || m.DataMovimentacao >= relatorio.DataInicio) &&
                            (!relatorio.DataFim.HasValue || m.DataMovimentacao <= relatorio.DataFim)
                        )
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
            // Obter usuário atual
            var user = await _userManager.GetUserAsync(User);

            var relatorio = new Relatorio
            {
                DataGeracao = DateTime.Now,
                UsuarioGerador = user?.UserName ?? "Usuário não identificado"
            };

            return View(relatorio);
        }

        // POST: Relatorios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo,DataGeracao,UsuarioGerador,DataInicio,DataFim,FornecedorId,ProdutoId,IncluirProdutosZerados,Ordenacao,ApenasAbaixoDoMinimo,TipoMovimentacao,Resumido")] Relatorio relatorio)
        {
            // Garantir que o usuário gerador seja o usuário logado
            var user = await _userManager.GetUserAsync(User);
            relatorio.UsuarioGerador = user?.UserName ?? "Usuário não identificado";

            // Implementar regras de datas conforme solicitado
            // Se data final não foi informada, usar data atual
            if (!relatorio.DataFim.HasValue)
            {
                relatorio.DataFim = DateTime.Now;
            }

            // Validação adicional para data início maior que data fim
            if (relatorio.DataInicio.HasValue && relatorio.DataFim.HasValue && relatorio.DataInicio > relatorio.DataFim)
            {
                ModelState.AddModelError("DataInicio", "A data inicial não pode ser maior que a data final");
            }

            if (ModelState.IsValid)
            {
                // Ação específica dependendo do tipo de relatório
                switch (relatorio.Tipo)
                {
                    case TipoRelatorio.Estoque:
                        // Aqui você implementaria a lógica para gerar o relatório de inventário
                        TempData["MensagemSucesso"] = $"Relatório de Inventário de Estoque gerado com sucesso!";
                        break;

                    case TipoRelatorio.Movimentacoes:
                        // Aqui você implementaria a lógica para gerar o relatório de movimentações
                        TempData["MensagemSucesso"] = $"Relatório de Movimentações gerado com sucesso!";
                        break;
                }

                _context.Add(relatorio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(relatorio);
        }

        // GET: Relatorios/Edit/5
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Redirecionar para detalhes ou index com mensagem informativa
            TempData["Mensagem"] = "A edição de relatórios não é permitida. Relatórios são registros históricos imutáveis.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Relatorios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,DataGeracao,UsuarioGerador")] Relatorio relatorio)
        {
            // Redirecionar para detalhes ou index com mensagem informativa
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

        private bool RelatorioExists(int id)
        {
            return _context.Relatorio.Any(e => e.Id == id);
        }
    }
}