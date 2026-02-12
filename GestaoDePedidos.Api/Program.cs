using GestaoDePedidos.Api.Data;
using GestaoDePedidos.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GestaoDePedidosDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GestaoDePedidosDbContext>();

    if (!context.Customers.Any())
    {
        context.Customers.Add(
            new Customer
            {
                Name = "Cliente 1",
                Email = "cliente1@cliente.com",
                CreatedAt = DateTime.UtcNow
            }
            );
        context.SaveChanges();
    }

        if (!context.Products.Any())
    {         context.Products.AddRange(
            new Product
            {
                Name = "Teclado Mecânico Logitech MX",
                Category = "Eletrônicos",
                PriceCents = 100000,
                Active = true
            },
            new Product
            {
                Name = "Smartphone iPhone 16",
                Category = "Eletrônicos",
                PriceCents = 450000,
                Active = true
            },
            new Product
            {
                Name = "Mouse Gamer Razer DeathAdder Essential",
                Category = "Eletrônicos",
                PriceCents = 17900,
                Active = true
            }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
