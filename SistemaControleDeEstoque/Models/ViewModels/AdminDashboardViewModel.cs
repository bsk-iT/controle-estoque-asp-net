namespace SistemaControleDeEstoque.Models.ViewModels
{
    /// <summary>
    /// ViewModel para o dashboard administrativo.
    /// Agrega os contadores exibidos na página inicial da área Admin,
    /// eliminando o uso de ViewBag dinâmico.
    /// </summary>
    public class AdminDashboardViewModel
    {
        /// <summary>
        /// Total de usuários cadastrados no sistema.
        /// </summary>
        public int TotalUsuarios { get; set; }

        /// <summary>
        /// Total de produtos cadastrados no estoque.
        /// </summary>
        public int TotalProdutos { get; set; }

        /// <summary>
        /// Número de movimentações realizadas no dia atual.
        /// </summary>
        public int OperacoesHoje { get; set; }
    }
}
