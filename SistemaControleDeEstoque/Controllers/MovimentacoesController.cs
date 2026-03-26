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
        public async Task<IActionResult> Create([Bind("Id,Tipo,Quantidade,DataMovimentacao,ProdutoId,UsuarioId,UsuarioNome")] Movimentacao movimentacao)
        {
            if (ModelState.IsValid)
            {
                // Se não foi enviado um usuário, garantir que seja o atual
                if (string.IsNullOrEmpty(movimentacao.UsuarioId))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null)
                    {
                        movimentacao.UsuarioId = user.Id;
                        movimentacao.UsuarioNome = user.UserName;
                    }
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Validar e atualizar o estoque do produto dentro da mesma transação
                    if (movimentacao.ProdutoId.HasValue)
                    {
                        var produto = await _context.Produto.FindAsync(movimentacao.ProdutoId);
                        if (produto != null)
                        {
                            if (movimentacao.Tipo == "Saída")
                            {
                                if (movimentacao.Quantidade > produto.Quantidade)
                                {
                                    ModelState.AddModelError("Quantidade", $"Quantidade solicitada ({movimentacao.Quantidade}) excede o estoque disponível ({produto.Quantidade}).");
                                    await transaction.RollbackAsync();
                                    goto ValidationFailed;
                                }
                                produto.Quantidade -= movimentacao.Quantidade;
                            }
                            else if (movimentacao.Tipo == "Entrada")
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

            ValidationFailed:
            // Se chegou aqui, é porque houve erro de validação, recarregar dados para a view
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

        private bool MovimentacaoExists(int id)
        {
            return _context.Movimentacao.Any(e => e.Id == id);
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