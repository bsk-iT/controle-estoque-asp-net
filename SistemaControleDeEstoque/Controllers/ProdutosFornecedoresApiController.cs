using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Controllers.Api
{
    /// <summary>DTO de entrada para associar Produto a Fornecedor (previne overposting de object graph).</summary>
    public record AssociarProdutoFornecedorDto(int ProdutoId, int FornecedorId);

    /// <summary>DTO de saída para retornar dados da associação criada (sem navigation properties).</summary>
    public record ProdutoFornecedorResponseDto(int Id, int ProdutoId, int FornecedorId);

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
        public async Task<ActionResult<ProdutoFornecedorResponseDto>> PostProdutoFornecedor([FromBody] AssociarProdutoFornecedorDto dto)
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

            // Construir entidade sem navigation properties para evitar overposting.
            // O EF Core rastreia a associação pelas FKs; null! suprime o aviso do compilador.
            var produtoFornecedor = new ProdutoFornecedor
            {
                ProdutoId = dto.ProdutoId,
                FornecedorId = dto.FornecedorId,
                Produto = null!,
                Fornecedor = null!,
            };

            _context.ProdutoFornecedor.Add(produtoFornecedor);
            await _context.SaveChangesAsync();

            var response = new ProdutoFornecedorResponseDto(
                produtoFornecedor.Id,
                produtoFornecedor.ProdutoId,
                produtoFornecedor.FornecedorId);

            // Usa nameof(PostProdutoFornecedor) pois não há GET action neste controller.
            // O Location header aponta para o próprio POST endpoint como referência do recurso criado.
            return CreatedAtAction(nameof(PostProdutoFornecedor), new { id = produtoFornecedor.Id }, response);
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
