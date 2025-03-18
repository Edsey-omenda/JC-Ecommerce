using JC_Ecommerce.Models.Domain;

namespace JC_Ecommerce.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Guid id, Product updatedProduct);
        Task<bool> DeleteAsync(Guid id);
    }
}
