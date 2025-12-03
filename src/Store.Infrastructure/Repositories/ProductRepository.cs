using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Application.Pagination;
using Store.Application.Params;
using Store.Domain.Entities;
using Store.Infrastructure.Pagination;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedList<Product>> GetPagedAsync(ProductParams productParams)
    {
        IQueryable<Product> query = _context.Products
            .Include(p => p.Translations)
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        if (productParams.InStockOnly)
        {
            query = query.Where(p => p.StockQuantity > 0);
        }

        if (!string.IsNullOrWhiteSpace(productParams.Search))
        {
            var search = productParams.Search.Trim().ToLower();

            query = query.Where(p =>
                p.Translations.Any(t => t.Name.ToLower().Contains(search) ||
            (t.Description != null && t.Description.ToLower().Contains(search))));
        }

        query = query.OrderByDescending(p => p.Id);

        return await query.ToPagedListAsync(productParams.PageNumber, productParams.PageSize);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Remove(Product product)
    {
        if (product == null)
        {
            return;
        }

        product.IsDeleted = true;
    }
}
