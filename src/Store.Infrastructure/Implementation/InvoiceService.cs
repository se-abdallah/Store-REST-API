using System;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Store.Application.DTOs.Store;
using Store.Application.Interfaces;
using Store.Application.Pagination;
using Store.Application.Params;
using Store.Domain.Entities;
using Store.Infrastructure.Identity;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Implementation;

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public InvoiceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<InvoiceCreateResultDto> CreateInvoiceAsync(
        int userId,
        CreateInvoiceDto dto,
        string language)
    {
        var result = new InvoiceCreateResultDto();

        if (dto == null || dto.Items == null || dto.Items.Count == 0)
        {
            result.Success = false;
            result.Errors.Add("At least one item is required.");
            return result;
        }

        var normalizedLanguage = string.IsNullOrWhiteSpace(language)
            ? "en"
            : language.Trim().ToLower();

        var errors = new List<string>();
        var details = new List<InvoiceDetail>();

        foreach (var item in dto.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);

            if (product == null || product.IsDeleted)
            {
                errors.Add($"Product with id {item.ProductId} was not found.");
                continue;
            }

            if (item.Quantity <= 0)
            {
                errors.Add($"Quantity for product {item.ProductId} must be at least 1.");
                continue;
            }

            if (product.StockQuantity < item.Quantity)
            {
                errors.Add(
                    $"Not enough stock for product {item.ProductId}. Available: {product.StockQuantity}, requested: {item.Quantity}.");
                continue;
            }

            var translation =
                product.Translations
                    .FirstOrDefault(t => t.Language.ToLower() == normalizedLanguage)
                ?? product.Translations
                    .FirstOrDefault(t => t.Language.ToLower() == "en")
                ?? product.Translations.FirstOrDefault();

            var productName = translation?.Name ?? $"Product #{product.Id}";
            var productDescription = translation?.Description;

            var detail = new InvoiceDetail
            {
                ProductId = product.Id,
                ProductName = productName,
                ProductDescription = productDescription,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                LineTotal = product.Price * item.Quantity
            };

            details.Add(detail);

            // update stock
            product.StockQuantity -= item.Quantity;
        }

        if (errors.Any())
        {
            result.Success = false;
            result.Errors.AddRange(errors);
            return result;
        }

        if (!details.Any())
        {
            result.Success = false;
            result.Errors.Add("No valid items to create an invoice.");
            return result;
        }

        var invoice = new Invoice
        {
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            TotalAmount = details.Sum(d => d.LineTotal),
            Details = details
        };

        await _unitOfWork.Invoices.AddAsync(invoice);
        await _unitOfWork.CompleteAsync();

        var invoiceDto = _mapper.Map<InvoiceDto>(invoice);

        result.Success = true;
        result.Invoice = invoiceDto;
        return result;
    }

    public async Task<PagedList<InvoiceDto>> GetUserInvoicesAsync(
        int userId,
        InvoiceParams invoiceParams)
    {
        var invoices = await _unitOfWork.Invoices
            .GetInvoicesForUserAsync(userId, invoiceParams);

        var dtoList = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);

        return new PagedList<InvoiceDto>(
            dtoList,
            invoices.TotalCount,
            invoices.CurrentPage,
            invoices.PageSize);
    }

    public async Task<InvoiceDto> GetUserInvoiceByIdAsync(int userId, int invoiceId)
    {
        var invoice = await _unitOfWork.Invoices
            .GetByIdWithDetailsForUserAsync(invoiceId, userId);

        if (invoice == null)
        {
            return null!;
        }

        return _mapper.Map<InvoiceDto>(invoice);
    }

    public async Task<PagedList<InvoiceAdminListItemDto>> GetAdminInvoicesAsync(
        InvoiceParams invoiceParams)
    {
        var invoices = await _unitOfWork.Invoices
            .GetInvoicesAsync(invoiceParams);

        var adminDtos = new List<InvoiceAdminListItemDto>();

        foreach (var invoice in invoices)
        {
            var user = await _userManager.FindByIdAsync(invoice.UserId.ToString());

            var dto = new InvoiceAdminListItemDto
            {
                Id = invoice.Id,
                CreatedAtUtc = invoice.CreatedAtUtc,
                TotalAmount = invoice.TotalAmount,
                TotalProducts = invoice.Details.Count,
                TotalQuantity = invoice.Details.Sum(d => d.Quantity),
                UserId = invoice.UserId,
                UserEmail = user?.Email ?? string.Empty,
                UserFullName = user?.FullName ?? string.Empty
            };

            adminDtos.Add(dto);
        }

        return new PagedList<InvoiceAdminListItemDto>(
            adminDtos,
            invoices.TotalCount,
            invoices.CurrentPage,
            invoices.PageSize);
    }

    public async Task<InvoiceAdminDetailDto> GetAdminInvoiceByIdAsync(int invoiceId)
    {
        var invoice = await _unitOfWork.Invoices
            .GetByIdWithDetailsAsync(invoiceId);

        if (invoice == null)
        {
            return null!;
        }

        var user = await _userManager.FindByIdAsync(invoice.UserId.ToString());

        var result = new InvoiceAdminDetailDto
        {
            Id = invoice.Id,
            CreatedAtUtc = invoice.CreatedAtUtc,
            TotalAmount = invoice.TotalAmount,
            TotalProducts = invoice.Details.Count,
            TotalQuantity = invoice.Details.Sum(d => d.Quantity),
            UserId = invoice.UserId,
            UserEmail = user?.Email ?? string.Empty,
            UserFullName = user?.FullName ?? string.Empty,
            Items = invoice.Details
                .Select(d => new InvoiceItemDto
                {
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    ProductDescription = d.ProductDescription,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,
                    LineTotal = d.LineTotal
                })
                .ToList()
        };

        return result;
    }
}
