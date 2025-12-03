using Store.Application.Pagination;
using Store.Application.Params;
using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface IInvoiceRepository
{
 Task AddAsync(Invoice invoice);

 Task<Invoice> GetByIdWithDetailsForUserAsync(int invoiceId, int userId);

 Task<PagedList<Invoice>> GetInvoicesForUserAsync(int userId, InvoiceParams invoiceParams);

 Task<Invoice> GetByIdWithDetailsAsync(int invoiceId);

 Task<PagedList<Invoice>> GetInvoicesAsync(InvoiceParams invoiceParams);
}
