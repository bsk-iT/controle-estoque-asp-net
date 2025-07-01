using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Fornecedor> Fornecedor { get; set; } = default!;
        public DbSet<Produto> Produto { get; set; } = default!;
        public DbSet<ProdutoFornecedor> ProdutoFornecedor { get; set; } = default!;
        public DbSet<Movimentacao> Movimentacao { get; set; } = default!;
        public DbSet<Relatorio> Relatorio { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da relação muitos-para-muitos via ProdutoFornecedor
            modelBuilder.Entity<ProdutoFornecedor>()
                .HasOne(pf => pf.Produto)
                .WithMany(p => p.Fornecedores)
                .HasForeignKey(pf => pf.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict); // Evita cascata

            modelBuilder.Entity<ProdutoFornecedor>()
                .HasOne(pf => pf.Fornecedor)
                .WithMany(f => f.Produtos)
                .HasForeignKey(pf => pf.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict); // Evita cascata
        }
    }
}