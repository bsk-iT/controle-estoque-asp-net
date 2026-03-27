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
    public class MovimentacoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MovimentacoesController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Movimentacoes
        public async Task<IActionResult> Index()
        {
            var movimentacoes = await _context.Movimentacao
                .Include(m => m.Produto)
                .ToListAsync();

            // Filtrar por usuário se não for Admin ou Gerente (previne IDOR)
            if (!User.IsInRole("Admin") && !User.IsInRole("Gerente"))
            {
                var userId = _userManager.GetUserId(User);
                movimentacoes = movimentacoes.Where(m => m.UsuarioId == userId).ToList();
            }

            return View(movimentacoes);
        }

        // GET: Movimentacoes/Details/5
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimentacao = await _context.Movimentacao
                .Include(m => m.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movimentacao == null)
            {
                return NotFound();
            }

            // Controle de ownership: apenas Admin/Gerente ou o próprio usuário podem ver
            if (!User.IsInRole("Admin") && !User.IsInRole("Gerente"))
            {
                var userId = _userManager.GetUserId(User);
                if (movimentacao.UsuarioId != userId)
                {
                    return NotFound(); // Retorna 404 em vez de 403 para não revelar que o registro existe
                }
            }

            return View(movimentacao);
        }

        // GET: Movimentacoes/Create
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Create()
        {
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome");

            // Criar um dicionário com os IDs dos produtos e suas quantidades disponíveis
            var produtosQuantidades = await _context.Produto
                .ToDictionaryAsync(p => p.Id, p => p.Quantidade);
            ViewData["EstoqueProdutos"] = produtosQuantidades;

            // Obter usuário atual
            var user = await _userManager.GetUserAsync(User);

            // Criar instância de movimentação com usuário preenchido
            var movimentacao = new Movimentacao
            {
                DataMovimentacao = DateTime.UtcNow,
                UsuarioId = user?.Id,
                UsuarioNome = user?.UserName ?? "Usuário não identificado"
            };

            return View(movimentacao);
        }

        // POST: Movimentacoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Create([Bind("Tipo,Quantidade,ProdutoId")] Movimentacao movimentacao)
        {
            // Sempre sobrescrever usuário autenticado (previne overposting)
            var user = await _userManager.GetUserAsync(User);
            movimentacao.UsuarioId = user!.Id;
            movimentacao.UsuarioNome = user.UserName;
            movimentacao.DataMovimentacao = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Validar e atualizar o estoque do produto dentro da mesma transação
                    if (movimentacao.ProdutoId.HasValue)
                    {
                        var produto = await _context.Produto.FindAsync(movimentacao.ProdutoId);
                        if (produto != null)
                        {
                            if (movimentacao.Tipo == TipoMovimentacao.Saida)
                            {
                                if (movimentacao.Quantidade > produto.Quantidade)
                                {
                                    ModelState.AddModelError("Quantidade", $"Quantidade solicitada ({movimentacao.Quantidade}) excede o estoque disponível ({produto.Quantidade}).");
                                    await transaction.RollbackAsync();
                                    return await RecarregarViewCreate(movimentacao);
                                }
                                produto.Quantidade -= movimentacao.Quantidade;
                            }
                            else if (movimentacao.Tipo == TipoMovimentacao.Entrada)
                            {
                                produto.Quantidade += movimentacao.Quantidade;
                            }
                            _context.Update(produto);
                        }
                    }

                    _context.Add(movimentacao);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return await RecarregarViewCreate(movimentacao);
        }

        /// <summary>
        /// Recarrega os dados necessários para exibir a view de criação de movimentação.
        /// </summary>
        private async Task<IActionResult> RecarregarViewCreate(Movimentacao movimentacao)
        {
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", movimentacao.ProdutoId);
            var produtosQuantidades = await _context.Produto
                .ToDictionaryAsync(p => p.Id, p => p.Quantidade);
            ViewData["EstoqueProdutos"] = produtosQuantidades;
            return View(movimentacao);
        }

        // GET: Movimentacoes/Edit/5
        // Movimentações são registros imutáveis — a edição não é permitida.
        [Authorize(Roles = "Admin,Gerente")]
        public IActionResult Edit(int? id)
        {
            return Forbid();
        }

        // POST: Movimentacoes/Edit/5
        // Movimentações são registros imutáveis — a edição não é permitida.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public IActionResult Edit(int id, IFormCollection form)
        {
            return Forbid();
        }

        // GET: Movimentacoes/Delete/5
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimentacao = await _context.Movimentacao
                .Include(m => m.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movimentacao == null)
            {
                return NotFound();
            }

            return View(movimentacao);
        }

        // POST: Movimentacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movimentacao = await _context.Movimentacao.FindAsync(id);
            if (movimentacao != null)
            {
                _context.Movimentacao.Remove(movimentacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MovimentacaoExistsAsync(int id)
        {
            return await _context.Movimentacao.AnyAsync(e => e.Id == id);
        }

        // Método para obter o estoque de um produto pelo ID (para chamadas AJAX)
        [HttpGet]
        public async Task<IActionResult> GetEstoque(int produtoId)
        {
            var produto = await _context.Produto.FindAsync(produtoId);
            if (produto == null)
            {
                return NotFound();
            }

            return Json(new { estoque = produto.Quantidade });
        }
    }
}