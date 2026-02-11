using Microsoft.EntityFrameworkCore;
using GestaoDePedidos.Api.Models;

namespace GestaoDePedidos.Api.Data
{
    public class GestaoDePedidosDbContext 
        : DbContext
    {
        public GestaoDePedidosDbContext(DbContextOptions<GestaoDePedidosDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
