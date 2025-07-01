using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Controllers
{
    public class ProdutosFornecedoresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdutosFornecedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProdutosFornecedores
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProdutoFornecedor.Include(p => p.Fornecedor).Include(p => p.Produto);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProdutosFornecedores/Details/5
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
        public IActionResult Create()
        {
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "CNPJ");
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome");
            return View();
        }

        // POST: ProdutosFornecedores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProdutoId,FornecedorId")] ProdutoFornecedor produtoFornecedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produtoFornecedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "CNPJ", produtoFornecedor.FornecedorId);
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", produtoFornecedor.ProdutoId);
            return View(produtoFornecedor);
        }

        // GET: ProdutosFornecedores/Edit/5
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
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "CNPJ", produtoFornecedor.FornecedorId);
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", produtoFornecedor.ProdutoId);
            return View(produtoFornecedor);
        }

        // POST: ProdutosFornecedores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProdutoId,FornecedorId")] ProdutoFornecedor produtoFornecedor)
        {
            if (id != produtoFornecedor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produtoFornecedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoFornecedorExists(produtoFornecedor.Id))
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
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "CNPJ", produtoFornecedor.FornecedorId);
            ViewData["ProdutoId"] = new SelectList(_context.Produto, "Id", "Nome", produtoFornecedor.ProdutoId);
            return View(produtoFornecedor);
        }

        // GET: ProdutosFornecedores/Delete/5
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

        private bool ProdutoFornecedorExists(int id)
        {
            return _context.ProdutoFornecedor.Any(e => e.Id == id);
        }
    }
}
