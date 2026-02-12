using GestaoDePedidos.Api.Data;
using GestaoDePedidos.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoDePedidos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly GestaoDePedidosDbContext _context;
        public ProductsController(GestaoDePedidosDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? category, bool? active, string? search)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);

            if (active.HasValue)
                query = query.Where(p => p.Active == active.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));

            var products = await query.ToListAsync();
            return products;
        }
    }
}
