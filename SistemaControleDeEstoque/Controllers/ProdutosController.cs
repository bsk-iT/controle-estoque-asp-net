using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;
using SistemaControleDeEstoque.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaControleDeEstoque.Controllers
{
    [Authorize]
    public class ProdutosController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produto
                .Include(p => p.Fornecedor)
                .Include(p => p.Fornecedores)
                .ToListAsync();
            return View(produtos);
        }

        // GET: Produtos/Details/5
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .Include(p => p.Fornecedor)
                .Include(p => p.Fornecedores)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produtos/Create
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Create()
        {
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome");
            ViewData["TiposProduto"] = await _context.Produto
                .Select(p => p.Tipo)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
            return View(new ProdutoViewModel { Nome = string.Empty, Tipo = string.Empty });
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Create(ProdutoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var produto = new Produto
                {
                    Nome = vm.Nome,
                    Tipo = System.Globalization.CultureInfo.CurrentCulture.TextInfo
                        .ToTitleCase(vm.Tipo.Trim().ToLower()),
                    Quantidade = vm.Quantidade,
                    Valor = vm.Valor,
                    FornecedorId = vm.FornecedorId,
                    EstoqueSeguranca = vm.EstoqueSeguranca
                };
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome", vm.FornecedorId);
            ViewData["TiposProduto"] = await _context.Produto
                .Select(p => p.Tipo)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
            return View(vm);
        }

        // GET: Produtos/Edit/5
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            var produtoFornecedores = await _context.ProdutoFornecedor
                .Include(pf => pf.Fornecedor)
                .Where(pf => pf.ProdutoId == id)
                .ToListAsync();

            var fornecedoresJaAssociados = produtoFornecedores.Select(pf => pf.FornecedorId).ToList();
            var fornecedoresDisponiveis = await _context.Fornecedor
                .Where(f => !fornecedoresJaAssociados.Contains(f.Id))
                .ToListAsync();

            var vm = new ProdutoEditViewModel
            {
                Produto = new ProdutoViewModel
                {
                    Id = produto.Id,
                    Nome = produto.Nome,
                    Tipo = produto.Tipo,
                    Quantidade = produto.Quantidade,
                    Valor = produto.Valor,
                    FornecedorId = produto.FornecedorId,
                    EstoqueSeguranca = produto.EstoqueSeguranca
                },
                ProdutoFornecedores = produtoFornecedores,
                FornecedoresDisponiveis = fornecedoresDisponiveis,
                FornecedoresSelectList = new SelectList(_context.Fornecedor, "Id", "Nome", produto.FornecedorId)
            };

            ViewData["TiposProduto"] = await _context.Produto
                .Select(p => p.Tipo)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return View(vm);
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Edit(int id, ProdutoEditViewModel vm)
        {
            if (id != vm.Produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var produto = new Produto
                {
                    Id = vm.Produto.Id,
                    Nome = vm.Produto.Nome,
                    Tipo = System.Globalization.CultureInfo.CurrentCulture.TextInfo
                        .ToTitleCase(vm.Produto.Tipo.Trim().ToLower()),
                    Quantidade = vm.Produto.Quantidade,
                    Valor = vm.Produto.Valor,
                    FornecedorId = vm.Produto.FornecedorId,
                    EstoqueSeguranca = vm.Produto.EstoqueSeguranca
                };

                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProdutoExistsAsync(vm.Produto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["MensagemSucesso"] = $"Produto \"{produto.Nome}\" atualizado com sucesso.";
                return RedirectToAction(nameof(Index));
            }

            // Recompor o ViewModel completo para reexibir a view com erros
            var produtoFornecedores = await _context.ProdutoFornecedor
                .Include(pf => pf.Fornecedor)
                .Where(pf => pf.ProdutoId == id)
                .ToListAsync();
            var fornecedoresJaAssociados = produtoFornecedores.Select(pf => pf.FornecedorId).ToList();
            var fornecedoresDisponiveis = await _context.Fornecedor
                .Where(f => !fornecedoresJaAssociados.Contains(f.Id))
                .ToListAsync();

            vm.ProdutoFornecedores = produtoFornecedores;
            vm.FornecedoresDisponiveis = fornecedoresDisponiveis;
            vm.FornecedoresSelectList = new SelectList(_context.Fornecedor, "Id", "Nome", vm.Produto.FornecedorId);

            ViewData["TiposProduto"] = await _context.Produto
                .Select(p => p.Tipo)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return View(vm);
        }

        // GET: Produtos/Delete/5
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produto
                .Include(p => p.Fornecedor)
                .Include(p => p.Fornecedores)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produto.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            try
            {
                _context.Produto.Remove(produto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProdutoExistsAsync(id))
                {
                    return RedirectToAction(nameof(Index));
                }
                throw;
            }

            TempData["MensagemSucesso"] = $"Produto \"{produto.Nome}\" excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProdutoExistsAsync(int id)
        {
            return await _context.Produto.AnyAsync(e => e.Id == id);
        }
    }
}
