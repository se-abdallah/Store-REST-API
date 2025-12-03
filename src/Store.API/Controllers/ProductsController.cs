using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedList<ProductListItemDto>>> GetProducts(
            [FromQuery] ProductParams productParams)
        {
            // Default language if not provided
            if (string.IsNullOrWhiteSpace(productParams.Language))
            {
                productParams.Language = "en";
            }

            var result = await _productService.GetPublicProductsAsync(productParams);

            Response.AddPaginationHeader(new PaginationHeader(
                result.CurrentPage,
                result.TotalCount,
                result.TotalPages,
                result.PageSize));

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductPublicDetailDto>> GetProductById(
            int id,
            [FromQuery] string language = "en")
        {
            var product = await _productService.GetPublicProductByIdAsync(id, language);

            if (product == null)
            {
                return NotFound(new ApiResponse(404, "Product not found."));
            }

            return Ok(product);
        }

        [HttpGet("admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<PagedList<ProductAdminDetailDto>>> GetAdminProducts(
            [FromQuery] ProductParams productParams)
        {

            var result = await _productService.GetAdminProductsAsync(productParams);

            Response.AddPaginationHeader(new PaginationHeader(
                result.CurrentPage,
                result.TotalCount,
                result.TotalPages,
                result.PageSize));

            return Ok(result);
        }

        [HttpGet("admin/{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ProductAdminDetailDto>> GetAdminProductById(int id)
        {
            var product = await _productService.GetAdminProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(new ApiResponse(404, "Product not found."));
            }

            return Ok(product);
        }


        [HttpPost("admin")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ProductAdminDetailDto>> CreateProduct(
            [FromBody] CreateProductDto dto)
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

            var result = await _productService.CreateProductAsync(dto);

            if (!result.Success)
            {
                var errorResponse = new ApiValidationErrorResponse
                {
                    Errors = result.Errors
                };

                return BadRequest(errorResponse);
            }


            return Ok(result.Data);
        }

        [HttpPut("admin/{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ProductAdminDetailDto>> UpdateProduct(
            int id,
            [FromBody] UpdateProductDto dto)
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

            var result = await _productService.UpdateProductAsync(id, dto);

            if (!result.Success)
            {

                if (result.Errors.Any(e => e.Contains("not found", System.StringComparison.OrdinalIgnoreCase)))
                {
                    return NotFound(new ApiResponse(404, result.Errors.First()));
                }

                var errorResponse = new ApiValidationErrorResponse
                {
                    Errors = result.Errors
                };

                return BadRequest(errorResponse);
            }

            return Ok(result.Data);
        }


        [HttpDelete("admin/{id:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> RemoveProduct(int id)
        {
            var result = await _productService.RemoveProductAsync(id);

            if (!result.Success)
            {
                if (result.Errors.Any(e => e.Contains("not found", System.StringComparison.OrdinalIgnoreCase)))
                {
                    return NotFound(new ApiResponse(404, result.Errors.First()));
                }

                var errorResponse = new ApiValidationErrorResponse
                {
                    Errors = result.Errors
                };

                return BadRequest(errorResponse);
            }

            return NoContent();
        }
    }
}
