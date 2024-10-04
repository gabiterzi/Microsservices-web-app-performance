using Dapper;
using MySqlConnector;
using web_app_domain;
using web_app_performance.Model;

namespace web_app_repository
{
    public class ProdutoRepository : IProdutoRepository

    {
        private readonly MySqlConnection mySqlConnection;
        public ProdutoRepository()
        {
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
        }

        public async Task<IEnumerable<Produto>> ListarProdutos()
        {
            await mySqlConnection.OpenAsync();
            string query = "select Id, Nome, preco , qtd_estoque , data_criacao from Produtos;";
            var produtos = await mySqlConnection.QueryAsync<Produto>(query);
            await mySqlConnection.CloseAsync();

            return produtos;
        }

        public async Task SalvarProduto(Produto produto)
        {
            await mySqlConnection.OpenAsync();
            string sql = @"insert into Produtos(nome, preco , qtd_estoque , data_criacao) 
                            values(@nome, @preco , @qtd_estoque , @data_criacao);";
            await mySqlConnection.ExecuteAsync(sql, produto);
            await mySqlConnection.CloseAsync();
        }

        public async Task AtualizarProduto(Produto produto)
        {
            await mySqlConnection.OpenAsync();
            string sql = @"update Produtos 
                            set Nome = @nome, 
	                        set Preco = @preco, 
                            set Qtd_estoque = @ qtd_estoque, 
                            set data_criacao = @data_criacao,
                            where Id = @id;";
            await mySqlConnection.ExecuteAsync(sql, produto);
            await mySqlConnection.CloseAsync();

        }

        public async Task RemoverProduto(int id)
        {
            await mySqlConnection.OpenAsync();

            string sql = "DELETE FROM produtos WHERE Id = @id;";

            await mySqlConnection.ExecuteAsync(sql, new { id });
            await mySqlConnection.CloseAsync();
        }
    }
}
