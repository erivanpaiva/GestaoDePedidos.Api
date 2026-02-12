using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoDePedidos.Api.Data;

namespace GestaoDePedidos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController
        : ControllerBase
    {
        private readonly GestaoDePedidosDbContext _context;
        public OrdersController(GestaoDePedidosDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetOrders()
        {
            var orders = await _context.Orders.Select(o => new
            {
                o.Id,
                o.Status,
                o.CreatedAt,
                Total = o.OrderItems.Sum(i => i.Quantity * i.UnitPriceCents)
            }).ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    o.Status,
                    o.CreatedAt,
                    Total = o.OrderItems.Sum(i => i.Quantity * i.UnitPriceCents),
                    Items = o.OrderItems.Select(i => new
                    {
                        i.ProductId,
                        i.Quantity,
                        i.UnitPriceCents
                    })
                }).FirstOrDefaultAsync();
            if (order == null)
                return NotFound();
            return Ok(order);
        }
    }
}
