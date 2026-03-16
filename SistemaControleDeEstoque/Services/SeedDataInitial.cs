using Microsoft.EntityFrameworkCore;
using SistemaControleDeEstoque.Data;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Services
{
    public class SeedDataInitial : ISeedDataInitial
    {
        private readonly ApplicationDbContext _context;

        public SeedDataInitial(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedFornecedoresAsync()
        {
            // Verifica se já existem fornecedores
            if (await _context.Fornecedor.AnyAsync())
            {
                return; // Banco já possui dados
            }

            var fornecedores = new List<Fornecedor>
            {
                new Fornecedor
                {
                    Nome = "African Beauty Distribuição",
                    CNPJ = "12345678000190",
                    Email = "contato@africanbeauty.com.br",
                    Telefone = "11987654321"
                },
                new Fornecedor
                {
                    Nome = "Weng Hair Importação LTDA",
                    CNPJ = "23456789000191",
                    Email = "vendas@wenghair.com.br",
                    Telefone = "11976543210"
                },
                new Fornecedor
                {
                    Nome = "Beleza Natural Cosméticos",
                    CNPJ = "34567890000192",
                    Email = "sac@belezanatural.com.br",
                    Telefone = "11965432109"
                },
                new Fornecedor
                {
                    Nome = "Mega Hair Brasil",
                    CNPJ = "45678901000193",
                    Email = "contato@megahairbrasil.com.br",
                    Telefone = "11954321098"
                },
                new Fornecedor
                {
                    Nome = "Produtos Capilares Premium",
                    CNPJ = "56789012000194",
                    Email = "vendas@capilarespremium.com.br",
                    Telefone = "11943210987"
                }
            };

            await _context.Fornecedor.AddRangeAsync(fornecedores);
            await _context.SaveChangesAsync();
        }

        public async Task SeedProdutosAsync()
        {
            // Verifica se já existem produtos
            if (await _context.Produto.AnyAsync())
            {
                return; // Banco já possui dados
            }

            // Aguarda a criação dos fornecedores primeiro
            await SeedFornecedoresAsync();

            var fornecedores = await _context.Fornecedor.ToListAsync();

            var produtos = new List<Produto>
            {
                // Cabelos
                new Produto
                {
                    Nome = "Jumbo Super X - Preto Natural",
                    Tipo = "Cabelo Sintético",
                    Quantidade = 50,
                    EstoqueSeguranca = 10,
                    Valor = 8.90m,
                    FornecedorId = fornecedores[0].Id
                },
                new Produto
                {
                    Nome = "Jumbo Super X - Castanho Escuro",
                    Tipo = "Cabelo Sintético",
                    Quantidade = 45,
                    EstoqueSeguranca = 10,
                    Valor = 8.90m,
                    FornecedorId = fornecedores[0].Id
                },
                new Produto
                {
                    Nome = "Jumbo Super X - Loiro Mel",
                    Tipo = "Cabelo Sintético",
                    Quantidade = 30,
                    EstoqueSeguranca = 8,
                    Valor = 9.50m,
                    FornecedorId = fornecedores[0].Id
                },
                new Produto
                {
                    Nome = "Cabelo Orgânico African Beauty 60cm",
                    Tipo = "Cabelo Natural",
                    Quantidade = 25,
                    EstoqueSeguranca = 5,
                    Valor = 45.00m,
                    FornecedorId = fornecedores[0].Id
                },
                new Produto
                {
                    Nome = "Cabelo Orgânico Weng 70cm",
                    Tipo = "Cabelo Natural",
                    Quantidade = 20,
                    EstoqueSeguranca = 5,
                    Valor = 55.00m,
                    FornecedorId = fornecedores[1].Id
                },
                // Tranças
                new Produto
                {
                    Nome = "Trança Box Braids - Preto",
                    Tipo = "Trança Sintética",
                    Quantidade = 60,
                    EstoqueSeguranca = 15,
                    Valor = 12.00m,
                    FornecedorId = fornecedores[1].Id
                },
                new Produto
                {
                    Nome = "Trança Afro Twist - Marsala",
                    Tipo = "Trança Sintética",
                    Quantidade = 40,
                    EstoqueSeguranca = 10,
                    Valor = 14.50m,
                    FornecedorId = fornecedores[1].Id
                },
                new Produto
                {
                    Nome = "Trança Nagô - Natural",
                    Tipo = "Trança Sintética",
                    Quantidade = 35,
                    EstoqueSeguranca = 10,
                    Valor = 11.00m,
                    FornecedorId = fornecedores[3].Id
                },
                new Produto
                {
                    Nome = "Trança Rastafári - Colorida",
                    Tipo = "Trança Sintética",
                    Quantidade = 28,
                    EstoqueSeguranca = 8,
                    Valor = 15.00m,
                    FornecedorId = fornecedores[3].Id
                },
                // Cremes
                new Produto
                {
                    Nome = "Creme para Pentear Beleza Natural 500ml",
                    Tipo = "Creme Capilar",
                    Quantidade = 80,
                    EstoqueSeguranca = 20,
                    Valor = 18.90m,
                    FornecedorId = fornecedores[2].Id
                },
                new Produto
                {
                    Nome = "Creme Multifuncional African Beauty 300ml",
                    Tipo = "Creme Capilar",
                    Quantidade = 65,
                    EstoqueSeguranca = 15,
                    Valor = 22.50m,
                    FornecedorId = fornecedores[0].Id
                },
                new Produto
                {
                    Nome = "Creme Ativador de Cachos 250ml",
                    Tipo = "Creme Capilar",
                    Quantidade = 55,
                    EstoqueSeguranca = 15,
                    Valor = 25.00m,
                    FornecedorId = fornecedores[2].Id
                },
                new Produto
                {
                    Nome = "Creme Hidratante Intensivo 1kg",
                    Tipo = "Creme Capilar",
                    Quantidade = 40,
                    EstoqueSeguranca = 10,
                    Valor = 35.00m,
                    FornecedorId = fornecedores[4].Id
                },
                // Pomadas
                new Produto
                {
                    Nome = "Pomada Modeladora Premium 100g",
                    Tipo = "Pomada",
                    Quantidade = 70,
                    EstoqueSeguranca = 20,
                    Valor = 15.90m,
                    FornecedorId = fornecedores[4].Id
                },
                new Produto
                {
                    Nome = "Pomada para Dreadlocks 150g",
                    Tipo = "Pomada",
                    Quantidade = 45,
                    EstoqueSeguranca = 12,
                    Valor = 19.50m,
                    FornecedorId = fornecedores[4].Id
                },
                new Produto
                {
                    Nome = "Pomada Finalizadora Brilho Intenso 80g",
                    Tipo = "Pomada",
                    Quantidade = 60,
                    EstoqueSeguranca = 15,
                    Valor = 12.00m,
                    FornecedorId = fornecedores[2].Id
                },
                new Produto
                {
                    Nome = "Pomada Nutritiva para Couro Cabeludo 120g",
                    Tipo = "Pomada",
                    Quantidade = 50,
                    EstoqueSeguranca = 12,
                    Valor = 17.90m,
                    FornecedorId = fornecedores[2].Id
                },
                // Produtos complementares
                new Produto
                {
                    Nome = "Kit Manutenção Tranças Completo",
                    Tipo = "Kit",
                    Quantidade = 30,
                    EstoqueSeguranca = 8,
                    Valor = 45.00m,
                    FornecedorId = fornecedores[1].Id
                },
                new Produto
                {
                    Nome = "Touca de Cetim - Diversos Tamanhos",
                    Tipo = "Acessório",
                    Quantidade = 100,
                    EstoqueSeguranca = 25,
                    Valor = 8.50m,
                    FornecedorId = fornecedores[3].Id
                },
                new Produto
                {
                    Nome = "Gel Fixador Extra Forte 500ml",
                    Tipo = "Finalizador",
                    Quantidade = 55,
                    EstoqueSeguranca = 15,
                    Valor = 14.90m,
                    FornecedorId = fornecedores[4].Id
                }
            };

            await _context.Produto.AddRangeAsync(produtos);
            await _context.SaveChangesAsync();

            // Criar relacionamentos Produto-Fornecedor adicionais (alguns produtos são oferecidos por múltiplos fornecedores)
            var produtosFornecedores = new List<ProdutoFornecedor>();

            var todosProdutos = await _context.Produto.ToListAsync();

            // Jumbo Super X também vendido pela Mega Hair Brasil
            var jumboPreto = todosProdutos.FirstOrDefault(p => p.Nome.Contains("Jumbo Super X - Preto"));
            if (jumboPreto != null)
            {
                produtosFornecedores.Add(new ProdutoFornecedor
                {
                    ProdutoId = jumboPreto.Id,
                    FornecedorId = fornecedores[3].Id,
                    Produto = jumboPreto,
                    Fornecedor = fornecedores[3]
                });
            }

            // Creme Ativador também vendido pela African Beauty
            var cremeAtivador = todosProdutos.FirstOrDefault(p => p.Nome.Contains("Creme Ativador"));
            if (cremeAtivador != null)
            {
                produtosFornecedores.Add(new ProdutoFornecedor
                {
                    ProdutoId = cremeAtivador.Id,
                    FornecedorId = fornecedores[0].Id,
                    Produto = cremeAtivador,
                    Fornecedor = fornecedores[0]
                });
            }

            // Pomada Modeladora também vendida pela Beleza Natural
            var pomadaModeladora = todosProdutos.FirstOrDefault(p => p.Nome.Contains("Pomada Modeladora"));
            if (pomadaModeladora != null)
            {
                produtosFornecedores.Add(new ProdutoFornecedor
                {
                    ProdutoId = pomadaModeladora.Id,
                    FornecedorId = fornecedores[2].Id,
                    Produto = pomadaModeladora,
                    Fornecedor = fornecedores[2]
                });
            }

            if (produtosFornecedores.Any())
            {
                await _context.ProdutoFornecedor.AddRangeAsync(produtosFornecedores);
                await _context.SaveChangesAsync();
            }
        }
    }
}
