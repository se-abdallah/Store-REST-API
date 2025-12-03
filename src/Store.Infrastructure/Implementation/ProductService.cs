using System;
using AutoMapper;
using Store.Application.DTOs.Store;
using Store.Application.Interfaces;
using Store.Application.Pagination;
using Store.Application.Params;
using Store.Domain.Entities;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Implementation;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedList<ProductListItemDto>> GetPublicProductsAsync(ProductParams productParams)
    {
        var products = await _unitOfWork.Products.GetPagedAsync(productParams);

        var items = new List<ProductListItemDto>();

        foreach (var product in products)
        {
            var translation = SelectTranslation(product, productParams.Language);

            var item = new ProductListItemDto
            {
                Id = product.Id,
                Name = translation?.Name ?? string.Empty,
                Price = product.Price,
                IsInStock = product.IsInStock,
                StockQuantity = product.StockQuantity
            };

            items.Add(item);
        }

        return new PagedList<ProductListItemDto>(
            items,
            products.TotalCount,
            products.CurrentPage,
            products.PageSize);
    }

    public async Task<ProductPublicDetailDto?> GetPublicProductByIdAsync(int id, string language)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
        {
            return null;
        }

        var translation = SelectTranslation(product, language);

        var dto = new ProductPublicDetailDto
        {
            Id = product.Id,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsInStock = product.IsInStock,
            Name = translation?.Name ?? string.Empty,
            Description = translation?.Description ?? string.Empty
        };

        return dto;
    }


    public async Task<PagedList<ProductAdminDetailDto>> GetAdminProductsAsync(ProductParams productParams)
    {
        var products = await _unitOfWork.Products.GetPagedAsync(productParams);

        var dtoItems = _mapper.Map<IEnumerable<ProductAdminDetailDto>>(products);

        return new PagedList<ProductAdminDetailDto>(
            dtoItems,
            products.TotalCount,
            products.CurrentPage,
            products.PageSize);
    }

    public async Task<ProductAdminDetailDto?> GetAdminProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
        {
            return null;
        }

        var dto = _mapper.Map<ProductAdminDetailDto>(product);
        return dto;
    }

    public async Task<ProductOperationResult<ProductAdminDetailDto>> CreateProductAsync(CreateProductDto dto)
    {
        var result = new ProductOperationResult<ProductAdminDetailDto>();

        if (dto.Translations == null || dto.Translations.Count == 0)
        {
            result.Success = false;
            result.Errors.Add("At least one translation is required.");
            return result;
        }

        var duplicateLanguages = dto.Translations
            .GroupBy(t => t.Language.Trim().ToLowerInvariant())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateLanguages.Any())
        {
            result.Success = false;
            result.Errors.Add("Duplicate language codes are not allowed in translations.");
            return result;
        }

        var product = new Product
        {
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            IsDeleted = false
        };

        foreach (var t in dto.Translations)
        {
            var translation = new ProductTranslation
            {
                Language = t.Language.Trim().ToLowerInvariant(),
                Name = t.Name,
                Description = t.Description
            };

            product.Translations.Add(translation);
        }

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.CompleteAsync();

        var createdDto = _mapper.Map<ProductAdminDetailDto>(product);

        result.Success = true;
        result.Data = createdDto;

        return result;
    }

    public async Task<ProductOperationResult<ProductAdminDetailDto>> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var result = new ProductOperationResult<ProductAdminDetailDto>();

        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
        {
            result.Success = false;
            result.Errors.Add("Product not found.");
            return result;
        }

        if (dto.Translations == null || dto.Translations.Count == 0)
        {
            result.Success = false;
            result.Errors.Add("At least one translation is required.");
            return result;
        }

        var duplicateLanguages = dto.Translations
            .GroupBy(t => t.Language.Trim().ToLowerInvariant())
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateLanguages.Any())
        {
            result.Success = false;
            result.Errors.Add("Duplicate language codes are not allowed in translations.");
            return result;
        }

        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;

        foreach (var t in dto.Translations)
        {
            var lang = t.Language.Trim().ToLowerInvariant();

            var existing = product.Translations
                .FirstOrDefault(x => x.Language.ToLower() == lang);

            if (existing == null)
            {
                product.Translations.Add(new ProductTranslation
                {
                    Language = lang,
                    Name = t.Name,
                    Description = t.Description
                });
            }
            else
            {
                existing.Name = t.Name;
                existing.Description = t.Description;
            }
        }

        var dtoLanguageSet = dto.Translations
            .Select(t => t.Language.Trim().ToLowerInvariant())
            .ToHashSet();

        var toRemove = product.Translations
            .Where(x => !dtoLanguageSet.Contains(x.Language.ToLower()))
            .ToList();

        foreach (var tr in toRemove)
        {
            product.Translations.Remove(tr);
        }

        _unitOfWork.Products.Update(product);
        await _unitOfWork.CompleteAsync();

        var updatedDto = _mapper.Map<ProductAdminDetailDto>(product);

        result.Success = true;
        result.Data = updatedDto;

        return result;
    }

    public async Task<ProductOperationResult<bool>> RemoveProductAsync(int id)
    {
        var result = new ProductOperationResult<bool>();

        var product = await _unitOfWork.Products.GetByIdAsync(id);

        if (product == null)
        {
            result.Success = false;
            result.Errors.Add("Product not found.");
            result.Data = false;
            return result;
        }

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.CompleteAsync();

        result.Success = true;
        result.Data = true;

        return result;
    }


    private static ProductTranslation? SelectTranslation(Product product, string language)
    {
        if (product == null || product.Translations == null || product.Translations.Count == 0)
        {
            return null;
        }

        var lang = (language ?? string.Empty).Trim().ToLowerInvariant();

        var translation = product.Translations
            .FirstOrDefault(t => t.Language.ToLower() == lang);

        if (translation != null)
        {
            return translation;
        }

        translation = product.Translations
            .FirstOrDefault(t => t.Language.ToLower() == "en");

        if (translation != null)
        {
            return translation;
        }

        return product.Translations.FirstOrDefault();
    }
}
