using System.Threading.Tasks;
using Store.Application.Pagination;
using Store.Application.Params;
using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface IProductRepository
{
 Task<PagedList<Product>> GetPagedAsync(ProductParams productParams);

 Task<Product> GetByIdAsync(int id);

 Task AddAsync(Product product);

 void Update(Product product);

 void Remove(Product product);
}
