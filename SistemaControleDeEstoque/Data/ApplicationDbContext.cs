using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Data
{
    public class ApplicationDbContext : IdentityDbContext, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>Tabela de chaves do Data Protection — persiste chaves entre reinicializações.</summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = default!;

        public DbSet<Fornecedor> Fornecedor { get; set; } = default!;
        public DbSet<Produto> Produto { get; set; } = default!;
        public DbSet<ProdutoFornecedor> ProdutoFornecedor { get; set; } = default!;
        public DbSet<Movimentacao> Movimentacao { get; set; } = default!;
        public DbSet<Relatorio> Relatorio { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar enum Tipo como string para compatibilidade com dados existentes
            modelBuilder.Entity<Movimentacao>()
                .Property(m => m.Tipo)
                .HasConversion<string>();

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