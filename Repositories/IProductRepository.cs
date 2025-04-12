using JC_Ecommerce.Models.Domain;
using JC_Ecommerce.Models.DTOs;

namespace JC_Ecommerce.Repositories
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageIndex = 1, int pageSize = 1000);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Guid id, Product updatedProduct);
        Task<bool> DeleteAsync(Guid id);
    }
}
