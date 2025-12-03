using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Errors;
using Store.API.Extensions;
using Store.API.Helpers;
using Store.Application.DTOs.Store;
using Store.Application.Interfaces;
using Store.Application.Pagination;
using Store.Application.Params;

namespace Store.API.Controllers
{
    [Authorize]
    public class InvoicesController : BaseApiController
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceDto>> CreateInvoice(
            [FromBody] CreateInvoiceDto dto,
            [FromQuery] string language = "en")
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = new ApiValidationErrorResponse
                {
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                };

                return BadRequest(errorResponse);
            }

            var userId = User.GetUserId();

            var result = await _invoiceService.CreateInvoiceAsync(userId, dto, language);

            if (!result.Success)
            {
                var errorResponse = new ApiValidationErrorResponse
                {
                    Errors = result.Errors
                };

                return BadRequest(errorResponse);
            }

            return Ok(result.Invoice);
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<InvoiceDto>>> GetMyInvoices(
            [FromQuery] InvoiceParams invoiceParams)
        {
            var userId = User.GetUserId();

            var invoices = await _invoiceService.GetUserInvoicesAsync(userId, invoiceParams);

            Response.AddPaginationHeader(
                new PaginationHeader(
                    invoices.CurrentPage,
                    invoices.TotalCount,
                    invoices.TotalPages,
                    invoices.PageSize));

            return Ok(invoices);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InvoiceDto>> GetMyInvoiceById(int id)
        {
            var userId = User.GetUserId();

            var invoice = await _invoiceService.GetUserInvoiceByIdAsync(userId, id);

            if (invoice == null)
            {
                return NotFound(new ApiResponse(404, "Invoice not found or does not belong to the current user."));
            }

            return Ok(invoice);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("admin")]
        public async Task<ActionResult<PagedList<InvoiceAdminListItemDto>>> GetAllInvoices(
            [FromQuery] InvoiceParams invoiceParams)
        {
            var invoices = await _invoiceService.GetAdminInvoicesAsync(invoiceParams);

            Response.AddPaginationHeader(
                new PaginationHeader(
                    invoices.CurrentPage,
                    invoices.TotalCount,
                    invoices.TotalPages,
                    invoices.PageSize));

            return Ok(invoices);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("admin/{id:int}")]
        public async Task<ActionResult<InvoiceAdminDetailDto>> GetInvoiceByIdForAdmin(int id)
        {
            var invoice = await _invoiceService.GetAdminInvoiceByIdAsync(id);

            if (invoice == null)
            {
                return NotFound(new ApiResponse(404, "Invoice not found."));
            }

            return Ok(invoice);
        }
    }
}
