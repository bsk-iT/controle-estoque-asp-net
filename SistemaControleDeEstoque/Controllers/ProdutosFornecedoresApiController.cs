using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;
using System.Threading.Tasks;

namespace SistemaControleDeEstoque.Controllers.Api
{
    /// <summary>DTO para associar Produto a Fornecedor (previne overposting de object graph)</summary>
    public record AssociarProdutoFornecedorDto(int ProdutoId, int FornecedorId);

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProdutosFornecedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProdutosFornecedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/ProdutosFornecedores
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<ActionResult<ProdutoFornecedor>> PostProdutoFornecedor([FromBody] AssociarProdutoFornecedorDto dto)
        {
            // Validar existência do Produto
            var produto = await _context.Produto.FindAsync(dto.ProdutoId);
            if (produto == null)
            {
                return NotFound(new { message = "Produto não encontrado" });
            }

            // Validar existência do Fornecedor
            var fornecedor = await _context.Fornecedor.FindAsync(dto.FornecedorId);
            if (fornecedor == null)
            {
                return NotFound(new { message = "Fornecedor não encontrado" });
            }

            // Verificar se já existe essa associação
            var existingRelation = await _context.ProdutoFornecedor
                .FirstOrDefaultAsync(pf =>
                    pf.ProdutoId == dto.ProdutoId &&
                    pf.FornecedorId == dto.FornecedorId);

            if (existingRelation != null)
            {
                return Conflict(new { message = "Esta associação já existe" });
            }

            // Construir entidade sem aceitar object graph do cliente (previne overposting)
            var produtoFornecedor = new ProdutoFornecedor
            {
                ProdutoId = dto.ProdutoId,
                FornecedorId = dto.FornecedorId,
                Produto = produto,
                Fornecedor = fornecedor
            };

            _context.ProdutoFornecedor.Add(produtoFornecedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProdutoFornecedor", new { id = produtoFornecedor.Id }, produtoFornecedor);
        }

        // DELETE: api/ProdutosFornecedores/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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