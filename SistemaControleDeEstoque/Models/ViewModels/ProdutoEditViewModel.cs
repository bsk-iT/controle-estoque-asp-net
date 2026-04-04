using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Models.ViewModels
{
    /// <summary>
    /// ViewModel para a view de edição de produtos.
    /// Agrega o formulário de edição junto com dados de apoio (fornecedores associados
    /// e fornecedores disponíveis para associação), eliminando o uso de ViewBag.
    /// </summary>
    public class ProdutoEditViewModel
    {
        /// <summary>
        /// Dados do formulário de edição do produto.
        /// </summary>
        public required ProdutoViewModel Produto { get; set; }

        /// <summary>
        /// Lista de fornecedores já associados a este produto.
        /// </summary>
        public IList<ProdutoFornecedor> ProdutoFornecedores { get; set; } = [];

        /// <summary>
        /// Lista de fornecedores disponíveis para associação (ainda não vinculados ao produto).
        /// </summary>
        public IList<Fornecedor> FornecedoresDisponiveis { get; set; } = [];

        /// <summary>
        /// SelectList de todos os fornecedores para o dropdown de fornecedor principal.
        /// </summary>
        public SelectList? FornecedoresSelectList { get; set; }
    }
}
