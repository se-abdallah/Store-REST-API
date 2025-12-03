using Store.Application.DTOs.Store;
using Store.Application.Pagination;
using Store.Application.Params;

namespace Store.Application.Interfaces;

public interface IInvoiceService
{
 Task<InvoiceCreateResultDto> CreateInvoiceAsync(int userId, CreateInvoiceDto dto, string language);
 Task<PagedList<InvoiceDto>> GetUserInvoicesAsync(int userId, InvoiceParams invoiceParams);

 Task<InvoiceDto> GetUserInvoiceByIdAsync(int userId, int invoiceId);

 Task<PagedList<InvoiceAdminListItemDto>> GetAdminInvoicesAsync(InvoiceParams invoiceParams);

 Task<InvoiceAdminDetailDto> GetAdminInvoiceByIdAsync(int invoiceId);
}
