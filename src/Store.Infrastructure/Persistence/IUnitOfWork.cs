using System;
using Store.Application.Interfaces;

namespace Store.Infrastructure.Persistence;

public interface IUnitOfWork : IDisposable
{
 IProductRepository Products { get; }
 IInvoiceRepository Invoices { get; }
 Task CompleteAsync();

}
