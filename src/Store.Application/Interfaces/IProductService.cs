using System;
using System.Threading.Tasks;
using Store.Application.DTOs.Store;
using Store.Application.Pagination;
using Store.Application.Params;

namespace Store.Application.Interfaces;

public interface IProductService
{
 Task<PagedList<ProductListItemDto>> GetPublicProductsAsync(ProductParams productParams);
 Task<ProductPublicDetailDto> GetPublicProductByIdAsync(int id, string language);

 // Admin
 Task<PagedList<ProductAdminDetailDto>> GetAdminProductsAsync(ProductParams productParams);
 Task<ProductAdminDetailDto> GetAdminProductByIdAsync(int id);

 Task<ProductOperationResult<ProductAdminDetailDto>> CreateProductAsync(CreateProductDto dto);

 Task<ProductOperationResult<ProductAdminDetailDto>> UpdateProductAsync(int id, UpdateProductDto dto);

 Task<ProductOperationResult<bool>> RemoveProductAsync(int id);
}
