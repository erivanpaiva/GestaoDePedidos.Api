using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoDePedidos.Api.Data;
using GestaoDePedidos.Api.Models;
using GestaoDePedidos.Api.DTOs;

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

        [HttpPost]
        public async Task<ActionResult> CreateOrder(CreateOrderDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("O pedido deve conter ao menos um item.");
            var order = new Order
            {
                CustomerId = dto.CustomerId,
                Status = "NOVO",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null)
                    return BadRequest($"Produto com ID {item.ProductId} não encontrado.");

                if (!product.Active)
                    return BadRequest($"Produto com ID {item.ProductId} está inativo.");

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPriceCents = product.PriceCents
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new { order.Id });

        }
    }
}