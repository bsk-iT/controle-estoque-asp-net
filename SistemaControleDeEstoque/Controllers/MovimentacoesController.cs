using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class MovimentacoesController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

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
                    return NotFound(); // 404 em vez de 403 para não revelar que o registro existe
                }
            }

            return View(movimentacao);
        }

        // GET: Movimentacoes/Create
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var produtosQuantidades = await _context.Produto
                .ToDictionaryAsync(p => p.Id, p => p.Quantidade);

            var vm = new MovimentacaoCreateViewModel
            {
                DataMovimentacao = DateTime.UtcNow,
                UsuarioNome = user?.UserName ?? "Usuário não identificado",
                EstoqueProdutos = produtosQuantidades
            };

            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome");
            return View(vm);
        }

        // POST: Movimentacoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Create(MovimentacaoCreateViewModel vm)
        {
            // Sempre sobrescrever dados de sistema (previne overposting)
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                var movimentacao = new Movimentacao
                {
                    Tipo = vm.Tipo,
                    Quantidade = vm.Quantidade,
                    ProdutoId = vm.ProdutoId,
                    UsuarioId = user.Id,
                    UsuarioNome = user.UserName ?? user.Email ?? "Usuário não identificado",
                    DataMovimentacao = DateTime.UtcNow
                };

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (movimentacao.ProdutoId.HasValue)
                    {
                        var produto = await _context.Produto.FindAsync(movimentacao.ProdutoId);
                        if (produto != null)
                        {
                        if (movimentacao.Tipo == TipoMovimentacao.Saida)
                        {
                            if (movimentacao.Quantidade > produto.Quantidade)
                            {
                                ModelState.AddModelError("Quantidade",
                                    $"Quantidade solicitada ({movimentacao.Quantidade}) excede o estoque disponível ({produto.Quantidade}).");
                                await transaction.RollbackAsync();
                                return await RecarregarViewCreate(vm);
                            }
                            produto.Quantidade -= movimentacao.Quantidade;

                            // Verificar se estoque ficou abaixo do mínimo após a saída
                            if (produto.Quantidade < produto.EstoqueSeguranca)
                            {
                                var deficit = produto.EstoqueSeguranca - produto.Quantidade;
                                TempData["AlertaEstoque"] = $"⚠️ Atenção: {produto.Nome} está com estoque abaixo do mínimo. " +
                                    $"Atual: {produto.Quantidade} un. | Mínimo: {produto.EstoqueSeguranca} un. | " +
                                    $"Déficit: {deficit} un.";
                            }
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

            return await RecarregarViewCreate(vm);
        }

        /// <summary>
        /// Recarrega os dados de apoio para reexibir a view de criação com erros de validação.
        /// </summary>
        private async Task<IActionResult> RecarregarViewCreate(MovimentacaoCreateViewModel vm)
        {
            vm.EstoqueProdutos = await _context.Produto
                .ToDictionaryAsync(p => p.Id, p => p.Quantidade);
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", vm.ProdutoId);
            return View(vm);
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
