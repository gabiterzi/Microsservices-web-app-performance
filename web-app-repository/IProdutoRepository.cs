using web_app_domain;
using web_app_performance.Model;

namespace web_app_repository
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> ListarProdutos();
        Task SalvarProduto(Produto produto);
        Task AtualizarProduto(Produto produto);
        Task RemoverProduto(int id);
    }
}