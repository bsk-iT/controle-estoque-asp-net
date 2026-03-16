namespace SistemaControleDeEstoque.Services;

public interface ISeedDataInitial
{
    Task SeedFornecedoresAsync();
    Task SeedProdutosAsync();
}
