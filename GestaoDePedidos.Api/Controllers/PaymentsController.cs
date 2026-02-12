using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoDePedidos.Api.Data;
using GestaoDePedidos.Api.Models;

namespace GestaoDePedidos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly GestaoDePedidosDbContext _context;

        public PaymentsController(GestaoDePedidosDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> CreatePayment(Payment payment)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == payment.OrderId);

            if (order == null)
                return NotFound("Pedido não encontrado.");

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var total = order.OrderItems.Sum(i => i.Quantity * i.UnitPriceCents);

            var totalPago = order.Payments.Sum(p => p.AmountCents);

            if (totalPago >= total)
            {
                order.Status = "PAGO";
                await _context.SaveChangesAsync();
            }

            return Ok("Pagamento realizado com sucesso.");
        }
    }
}
