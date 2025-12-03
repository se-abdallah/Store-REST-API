using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Store.Domain.Entities;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Data;

public static class StoreSeed
{
    public static async Task SeedProductsAsync(AppDbContext context)
    {

        if (context.Products.Any())
        {
            return;
        }

        var basePath = AppContext.BaseDirectory;

        var filePath = Path.Combine(basePath, "Data", "Seed", "Products.json");

        if (!File.Exists(filePath))
        {
            return;
        }

        var productsData = await File.ReadAllTextAsync(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var products = JsonSerializer.Deserialize<List<Product>>(productsData, options);

        if (products == null || products.Count == 0)
        {
            return;
        }

        await context.Products.AddRangeAsync(products);

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
        }
    }
}


