using JC_Ecommerce.Data;
using JC_Ecommerce.Models.Domain;
using JC_Ecommerce.Models.DTOs;
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

        /*public async Task<List<Product>> GetAllAsync(string? filterOn = null, string? filterQuery = null, 
            string? sortBy = null, bool isAscending = true, 
            int pageIndex = 1, int pageSize = 1000)
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
            var skipResults = (pageIndex - 1) * pageSize;
            return await products.Skip(skipResults).Take(pageSize).ToListAsync();
        }*/

        public async Task<PagedResult<Product>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageIndex = 1, int pageSize = 10)
        {
            var query = jCEcommerceDbContext.Products.AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    //query = query.Where(p => p.Name.Contains(filterQuery));
                    query = query.Where(p => p.Name.ToLower().Contains(filterQuery.ToLower()));
                }
                else if (filterOn.Equals("Category", StringComparison.OrdinalIgnoreCase))
                {
                    //Filtering by a single category
                    //query = query.Where(p => p.Category.ToLower().Contains(filterQuery.ToLower()));

                    //Filtering by multiple categories
                    var categories = filterQuery?.Split(',').Select(c => c.Trim().ToLower()).ToList();

                    if (categories != null && categories.Any())
                    {
                        query = query.Where(p => categories.Contains(p.Category.ToLower()));
                    }
                }
            }

            var totalItems = await jCEcommerceDbContext.Products.CountAsync();
            var filteredItems = await query.CountAsync();

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "name" => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                    "price" => isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "createdat" => isAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    _ => query
                };
            }

            var skip = (pageIndex - 1) * pageSize;
            var items = await query.Skip(skip).Take(pageSize).ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalItems = totalItems,
                FilteredItems = filteredItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
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
