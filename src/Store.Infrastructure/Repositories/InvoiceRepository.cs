using System;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Application.Pagination;
using Store.Application.Params;
using Store.Domain.Entities;
using Store.Infrastructure.Pagination;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
    }

    public async Task<Invoice?> GetByIdWithDetailsForUserAsync(int invoiceId, int userId)
    {
        return await _context.Invoices
            .Include(i => i.Details)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == invoiceId && i.UserId == userId);
    }

    public async Task<PagedList<Invoice>> GetInvoicesForUserAsync(int userId, InvoiceParams invoiceParams)
    {
        var query = _context.Invoices
            .Include(i => i.Details)
            .AsNoTracking()
            .Where(i => i.UserId == userId);

        if (invoiceParams.From.HasValue)
        {
            var fromUtc = invoiceParams.From.Value.ToUniversalTime();
            query = query.Where(i => i.CreatedAtUtc >= fromUtc);
        }

        if (invoiceParams.To.HasValue)
        {
            var toUtc = invoiceParams.To.Value.ToUniversalTime();
            query = query.Where(i => i.CreatedAtUtc <= toUtc);
        }

        var order = invoiceParams.OrderBy?.Trim().ToLower();

        query = order switch
        {
            "date_asc" => query.OrderBy(i => i.CreatedAtUtc),
            "amount_desc" => query.OrderByDescending(i => (double)i.TotalAmount),
            "amount_asc" => query.OrderBy(i => (double)i.TotalAmount),
            _ => query.OrderByDescending(i => i.CreatedAtUtc)
        };

        return await query.ToPagedListAsync(invoiceParams.PageNumber, invoiceParams.PageSize);
    }

    public async Task<Invoice?> GetByIdWithDetailsAsync(int invoiceId)
    {
        return await _context.Invoices
        .Include(i => i.Details)
        .AsNoTracking()
        .FirstOrDefaultAsync(i => i.Id == invoiceId);
    }

    public async Task<PagedList<Invoice>> GetInvoicesAsync(InvoiceParams invoiceParams)
    {
        var query = _context.Invoices
            .Include(i => i.Details)
            .AsNoTracking()
            .AsQueryable();

        if (invoiceParams.From.HasValue)
        {
            var fromUtc = invoiceParams.From.Value.ToUniversalTime();
            query = query.Where(i => i.CreatedAtUtc >= fromUtc);
        }

        if (invoiceParams.To.HasValue)
        {
            var toUtc = invoiceParams.To.Value.ToUniversalTime();
            query = query.Where(i => i.CreatedAtUtc <= toUtc);
        }

        query = invoiceParams.OrderBy switch
        {
            "date_asc" => query.OrderBy(i => i.CreatedAtUtc),
            "amount_desc" => query.OrderByDescending(i => i.TotalAmount),
            "amount_asc" => query.OrderBy(i => i.TotalAmount),
            _ => query.OrderByDescending(i => i.CreatedAtUtc)
        };

        return await query.ToPagedListAsync(invoiceParams.PageNumber, invoiceParams.PageSize);
    }
}
