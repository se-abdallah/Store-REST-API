using System;
using Store.Application.Interfaces;
using Store.Infrastructure.Repositories;

namespace Store.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
 private readonly AppDbContext _context;
 public UnitOfWork(AppDbContext context)
 {
  _context = context; ;
 }

 public IProductRepository Products => new ProductRepository(_context);
 public IInvoiceRepository Invoices => new InvoiceRepository(_context);

 public async Task CompleteAsync()
 {
  await _context.SaveChangesAsync();
 }

 public void Dispose()
 {
  _context.Dispose();
 }
}
