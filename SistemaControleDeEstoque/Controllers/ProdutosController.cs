using Microsoft.AspNetCore.Authorization;
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
    public class ProdutosController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Produtos
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Produto
                                      .Include(p => p.Fornecedor)
                                      .Include(p => p.Fornecedores);
            return View(await applicationDbContext.ToListAsync());
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
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public IActionResult Create()
        {
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome");
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Tipo,Quantidade,Valor,FornecedorId")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome", produto.FornecedorId);
            return View(produto);
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

            // Obter todos os fornecedores associados a este produto
            var produtoFornecedores = await _context.ProdutoFornecedor
                .Include(pf => pf.Fornecedor)
                .Where(pf => pf.ProdutoId == id)
                .ToListAsync();

            // Obter todos os fornecedores disponíveis que ainda não estão associados a este produto
            var fornecedoresJaAssociados = produtoFornecedores.Select(pf => pf.FornecedorId).ToList();
            var fornecedoresDisponiveis = await _context.Fornecedor
                .Where(f => !fornecedoresJaAssociados.Contains(f.Id))
                .ToListAsync();

            ViewBag.ProdutoFornecedores = produtoFornecedores;
            ViewBag.FornecedoresDisponiveis = fornecedoresDisponiveis;
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome", produto.FornecedorId);

            return View(produto);
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Tipo,Quantidade,Valor,FornecedorId")] Produto produto)
        {
            if (id != produto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
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
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "Id", "Nome", produto.FornecedorId);
            return View(produto);
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produto.FindAsync(id);
            if (produto != null)
            {
                _context.Produto.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produto.Any(e => e.Id == id);
        }
    }
}