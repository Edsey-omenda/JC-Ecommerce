using JC_Ecommerce.Models.Domain;
using JC_Ecommerce.Models.DTOs;
using JC_Ecommerce.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JC_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        //Get all products
        //GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var products = await productRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);

            var response = products.Select(p => new ProductResponseDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Category = p.Category
            });

            return Ok(response);
        }

        //Get a single product by Id
        //GET: api/products/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetProductById([FromRoute] Guid id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if(product == null)
            {
                return NotFound();
            }

            var response = new ProductResponseDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Category = product.Category
            };

            return Ok(response);
        }


        //Add/Create a new product
        //POST: api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productCreateDTO)
        {
            var newProduct = new Product
            {
                Name = productCreateDTO.Name,
                Description = productCreateDTO.Description,
                Price = productCreateDTO.Price,
                Stock = productCreateDTO.Stock,
                ImageUrl = productCreateDTO.ImageUrl,
                Category = productCreateDTO.Category,
                CreatedAt = DateTime.UtcNow
            };

            await productRepository.CreateAsync(newProduct);

            return Ok(newProduct);
        }

        //Update a product
        //PUT: api/products/{id:Guid}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] ProductCreateDTO productCreateDTO)
        {
            var existingProduct = await productRepository.GetByIdAsync(id);

            if ((existingProduct == null)){

                return BadRequest("Product does not exist!");
            }

            var updatedProduct = new Product
            {
                Name = productCreateDTO.Name,
                Description = productCreateDTO.Description,
                Price = productCreateDTO.Price,
                Stock = productCreateDTO.Stock,
                ImageUrl = productCreateDTO.ImageUrl,
                Category = productCreateDTO.Category
            };

            var result = await productRepository.UpdateAsync(id, updatedProduct);

            return Ok(result);  
        }

        //Delete a product
        //DELETE: api/products/{id:Guid}
        [HttpDelete]
        [Route("{id:Guid}")]

        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id) 
        { 
            var existingProduct = await productRepository.GetByIdAsync(id);

            if ((existingProduct == null))
            {
                return BadRequest("Product does not exist!");
            }

            var deletedProduct = await productRepository.DeleteAsync(id);

            return Ok(deletedProduct);
        }

    }
}
