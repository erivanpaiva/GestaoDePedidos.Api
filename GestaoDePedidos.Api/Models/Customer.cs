using Microsoft.AspNetCore.Http.HttpResults;

namespace GestaoDePedidos.Api.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}