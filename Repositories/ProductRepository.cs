using JC_Ecommerce.Data;
using JC_Ecommerce.Models.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JC_Ecommerce.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly JCEcommerceDbContext jCEcommerceDbContext;

        public ProductRepository(JCEcommerceDbContext jCEcommerceDbContext)
        {
            this.jCEcommerceDbContext = jCEcommerceDbContext;
        }

        public async Task<List<Product>> GetAllAsync(string? filterOn = null, string? filterQuery = null, 
            string? sortBy = null, bool isAscending = true, 
            int pageNumber = 1, int pageSize = 1000)
        {
            var products = jCEcommerceDbContext.Products.AsQueryable();

            //Filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Name.Contains(filterQuery));
                }
                else if (filterOn.Equals("Category", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(p => p.Category.Contains(filterQuery));
                }
            }

            //Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending
                        ? products.OrderBy(p => p.Name)
                        : products.OrderByDescending(p => p.Name);
                }
                else if (sortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending
                        ? products.OrderBy(p => p.Price)
                        : products.OrderByDescending(p => p.Price);
                }
                else if (sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending
                        ? products.OrderBy(p => p.CreatedAt)
                        : products.OrderByDescending(p => p.CreatedAt);
                }
            }

            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;
            return await products.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public Task<Product?> GetByIdAsync(Guid id)
        {
            var product = jCEcommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId  == id);

            return product;

        }
        public async Task<Product> CreateAsync(Product product)
        {
            product.ProductId = Guid.NewGuid();
            product.CreatedAt = DateTime.UtcNow;

            await jCEcommerceDbContext.Products.AddAsync(product);
            await jCEcommerceDbContext.SaveChangesAsync();

            return product;
        }

        public async Task<Product?> UpdateAsync(Guid id, Product updatedProduct)
        {
            var existingProduct = await jCEcommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);

            if(existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Stock = updatedProduct.Stock;
            existingProduct.ImageUrl = updatedProduct.ImageUrl;
            existingProduct.Category = updatedProduct.Category;

            await jCEcommerceDbContext.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingProduct = await jCEcommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);

            if (existingProduct == null)
            {
                return false;
            }

            jCEcommerceDbContext.Products.Remove(existingProduct);
            await jCEcommerceDbContext.SaveChangesAsync();

            return true;
        }

    }
}
