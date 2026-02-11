using Microsoft.AspNetCore.Http.HttpResults;

namespace GestaoDePedidos.Api.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Method { get; set; }
        public int AmountCents { get; set; }
        public DateTime? PaidAt { get; set; }
        public Order Order { get; set; }
    }
}