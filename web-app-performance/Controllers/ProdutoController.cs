using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using StackExchange.Redis;
using web_app_performance.Model;

namespace web_app_performance.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class ProdutoController : ControllerBase
        {
            private static ConnectionMultiplexer redis;

            [HttpGet]
            public async Task<IActionResult> GetProduto()
            {
                string key = "getProduto";
                redis = ConnectionMultiplexer.Connect("localhost:6379");
                IDatabase db = redis.GetDatabase();
                await db.KeyExpireAsync(key, TimeSpan.FromMinutes(10));
                string user = await db.StringGetAsync(key);

                if (!string.IsNullOrEmpty(user))
                {
                    return Ok(user);
                }

                string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();
                string query = "select Id, Nome, preco , qtd_estoque , data_criacao from Produtos;";
                var Produtos = await connection.QueryAsync<Produto>(query);
                string ProdutosJson = JsonConvert.SerializeObject(Produtos);
                await db.StringSetAsync(key, ProdutosJson);

                return Ok(Produtos);
            }

            [HttpPost]
            public async Task<IActionResult> Post([FromBody] Produto Produto)
            {
                string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                string sql = @"insert into Produtos(nome, preco , qtd_estoque , data_criacao) 
                            values(@nome, @preco , @qtd_estoque , @data_criacao);";
                await connection.ExecuteAsync(sql, Produto);

                //apagar o cachê
                string key = "getProduto";
                redis = ConnectionMultiplexer.Connect("localhost:6379");
                IDatabase db = redis.GetDatabase();
                await db.KeyDeleteAsync(key);

                return Ok();
            }

            [HttpPut]
            public async Task<IActionResult> Put([FromBody] Produto Produto)
            {
                string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                string sql = @"update Produtos 
                            set Nome = @nome, 
	                        set Preco = @preco, 
                            set Qtd_estoque = @ qtd_estoque, 
                            set data_criacao = @data_criacao,
                            where Id = @id;";

                await connection.ExecuteAsync(sql, Produto);

                //apagar o cachê
                string key = "getProduto";
                redis = ConnectionMultiplexer.Connect("localhost:6379");
                IDatabase db = redis.GetDatabase();
                await db.KeyDeleteAsync(key);

                return Ok();
            }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Remover produto do banco de dados
            string connectionString = "Server=localhost;Database=sys;User=root;Password=123;";
            using var connection = new MySqlConnection(connectionString);
            {
                await connection.OpenAsync();

                string sql = "DELETE FROM produtos WHERE Id = @id;";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });

                // Verifica se alguma linha foi afetada
                if (rowsAffected == 0)
                {
                    return NotFound(); // Produto não encontrado
                }
            }

            // Apagar o cache para o produto específico
            string key = $"produto:{id}"; // Ajustar a chave para o produto específico
            IDatabase db = redis.GetDatabase();
            await db.KeyDeleteAsync(key);

            return Ok(); // Retornar 200 OK após a remoção
        }
    }
   }

