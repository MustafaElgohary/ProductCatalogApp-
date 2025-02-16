using ProductCatalog.Core.Entities;
using ProductCatalog.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalog.Infrastructure.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            var currentTime = DateTime.UtcNow;
            return await _productRepository.GetActiveProductsAsync(currentTime);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _productRepository.GetProductsByCategoryAsync(categoryId);
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(Product product)
        {
            await _productRepository.DeleteAsync(product);
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.ListAllWithCategoriesAsync();
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdWithCategoryAsync(id);
        }
    }
}
