using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;
using SistemaControleDeEstoque.Models.ViewModels;
using System.Threading.Tasks;

namespace SistemaControleDeEstoque.Controllers
{
    [Authorize]
    public class ProdutosFornecedoresController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: ProdutosFornecedores
        public async Task<IActionResult> Index()
        {
            var lista = await _context.ProdutoFornecedor
                .Include(p => p.Fornecedor)
                .Include(p => p.Produto)
                .ToListAsync();
            return View(lista);
        }

        // GET: ProdutosFornecedores/Details/5
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtoFornecedor = await _context.ProdutoFornecedor
                .Include(p => p.Fornecedor)
                .Include(p => p.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produtoFornecedor == null)
            {
                return NotFound();
            }

            return View(produtoFornecedor);
        }

        // GET: ProdutosFornecedores/Create
        [Authorize(Roles = "Admin,Gerente")]
        public IActionResult Create()
        {
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "CNPJ");
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome");
            return View(new ProdutoFornecedorViewModel());
        }

        // POST: ProdutosFornecedores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Create(ProdutoFornecedorViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var produtoFornecedor = new ProdutoFornecedor
                {
                    ProdutoId = vm.ProdutoId,
                    FornecedorId = vm.FornecedorId,
                    Produto = null!,
                    Fornecedor = null!
                };
                _context.Add(produtoFornecedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "CNPJ", vm.FornecedorId);
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", vm.ProdutoId);
            return View(vm);
        }

        // GET: ProdutosFornecedores/Edit/5
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtoFornecedor = await _context.ProdutoFornecedor.FindAsync(id);
            if (produtoFornecedor == null)
            {
                return NotFound();
            }

            var vm = new ProdutoFornecedorViewModel
            {
                Id = produtoFornecedor.Id,
                ProdutoId = produtoFornecedor.ProdutoId,
                FornecedorId = produtoFornecedor.FornecedorId
            };
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "CNPJ", vm.FornecedorId);
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", vm.ProdutoId);
            return View(vm);
        }

        // POST: ProdutosFornecedores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Edit(int id, ProdutoFornecedorViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var produtoFornecedor = new ProdutoFornecedor
                {
                    Id = vm.Id,
                    ProdutoId = vm.ProdutoId,
                    FornecedorId = vm.FornecedorId,
                    Produto = null!,
                    Fornecedor = null!
                };

                try
                {
                    _context.Update(produtoFornecedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProdutoFornecedorExistsAsync(vm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "CNPJ", vm.FornecedorId);
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", vm.ProdutoId);
            return View(vm);
        }

        // GET: ProdutosFornecedores/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtoFornecedor = await _context.ProdutoFornecedor
                .Include(p => p.Fornecedor)
                .Include(p => p.Produto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produtoFornecedor == null)
            {
                return NotFound();
            }

            return View(produtoFornecedor);
        }

        // POST: ProdutosFornecedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produtoFornecedor = await _context.ProdutoFornecedor.FindAsync(id);
            if (produtoFornecedor != null)
            {
                _context.ProdutoFornecedor.Remove(produtoFornecedor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProdutoFornecedorExistsAsync(int id)
        {
            return await _context.ProdutoFornecedor.AnyAsync(e => e.Id == id);
        }
    }
}
