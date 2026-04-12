using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;
using SistemaControleDeEstoque.Models.ViewModels;
using System.Threading.Tasks;

namespace SistemaControleDeEstoque.Controllers
{
    [Authorize]
    public class FornecedoresController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Fornecedores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fornecedor.ToListAsync());
        }

        // GET: Fornecedores/Details/5
        [Authorize(Policy = "RequireUserAdminGerenteRole")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedor = await _context.Fornecedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            return View(fornecedor);
        }

        // GET: Fornecedores/Create
        [Authorize(Roles = "Admin,Gerente")]
        public IActionResult Create()
        {
            return View(new FornecedorViewModel
            {
                Nome = string.Empty,
                CNPJ = string.Empty,
                Email = string.Empty,
                Telefone = string.Empty
            });
        }

        // POST: Fornecedores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Create(FornecedorViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var fornecedor = new Fornecedor
                {
                    Nome = vm.Nome,
                    CNPJ = vm.CNPJ,
                    Email = vm.Email,
                    Telefone = vm.Telefone
                };
                _context.Add(fornecedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // GET: Fornecedores/Edit/5
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedor = await _context.Fornecedor.FindAsync(id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            var vm = new FornecedorViewModel
            {
                Id = fornecedor.Id,
                Nome = fornecedor.Nome,
                CNPJ = fornecedor.CNPJ,
                Email = fornecedor.Email,
                Telefone = fornecedor.Telefone
            };
            return View(vm);
        }

        // POST: Fornecedores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Edit(int id, FornecedorViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var fornecedor = new Fornecedor
                {
                    Id = vm.Id,
                    Nome = vm.Nome,
                    CNPJ = vm.CNPJ,
                    Email = vm.Email,
                    Telefone = vm.Telefone
                };

                try
                {
                    _context.Update(fornecedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await FornecedorExistsAsync(vm.Id))
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
            return View(vm);
        }

        // GET: Fornecedores/Delete/5
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedor = await _context.Fornecedor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            return View(fornecedor);
        }

        // POST: Fornecedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fornecedor = await _context.Fornecedor.FindAsync(id);
            if (fornecedor == null)
            {
                return NotFound();
            }

            try
            {
                _context.Fornecedor.Remove(fornecedor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FornecedorExistsAsync(id))
                {
                    return RedirectToAction(nameof(Index));
                }
                throw;
            }

            TempData["MensagemSucesso"] = $"Fornecedor \"{fornecedor.Nome}\" excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> FornecedorExistsAsync(int id)
        {
            return await _context.Fornecedor.AnyAsync(e => e.Id == id);
        }
    }
}
