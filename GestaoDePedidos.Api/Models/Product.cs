using Microsoft.AspNetCore.Http.HttpResults;

namespace GestaoDePedidos.Api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int PriceCents { get; set; }
        public bool Active { get; set; } = true;
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}