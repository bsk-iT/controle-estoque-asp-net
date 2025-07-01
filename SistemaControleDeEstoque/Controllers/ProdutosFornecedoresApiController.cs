using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;
using System.Threading.Tasks;

namespace SistemaControleDeEstoque.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosFornecedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProdutosFornecedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/ProdutosFornecedores
        [HttpPost]
        public async Task<ActionResult<ProdutoFornecedor>> PostProdutoFornecedor([FromBody] ProdutoFornecedor produtoFornecedor)
        {
            // Verificar se já existe essa associação
            var existingRelation = await _context.ProdutoFornecedor
                .FirstOrDefaultAsync(pf =>
                    pf.ProdutoId == produtoFornecedor.ProdutoId &&
                    pf.FornecedorId == produtoFornecedor.FornecedorId);

            if (existingRelation != null)
            {
                return Conflict(new { message = "Esta associação já existe" });
            }

            _context.ProdutoFornecedor.Add(produtoFornecedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProdutoFornecedor", new { id = produtoFornecedor.Id }, produtoFornecedor);
        }

        // DELETE: api/ProdutosFornecedores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProdutoFornecedor(int id)
        {
            var produtoFornecedor = await _context.ProdutoFornecedor.FindAsync(id);
            if (produtoFornecedor == null)
            {
                return NotFound();
            }

            _context.ProdutoFornecedor.Remove(produtoFornecedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}