namespace web_app_performance.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; } 
        public int Qtd_estoque { get; set; } 
        public DateTime DataCriacao { get; set; } 
    }
}
