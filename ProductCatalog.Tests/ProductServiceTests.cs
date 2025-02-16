using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Core.Entities;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Infrastructure.Repositories;
using ProductCatalog.Infrastructure.Services;
using ProductCatalog.Web.Controllers;

namespace ProductCatalog.Tests
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductService _productService;
        private readonly ProductRepository _productRepository;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductCatalogDb")
                .Options;
            _context = new ApplicationDbContext(options);

            _context.Categories.Add(new Category { Id = 1, Name = "TestCategory" });
            _context.Products.Add(new Product
            {
                Id = 1,
                Name = "Test Product",
                StartDate = DateTime.UtcNow.AddDays(-1), 
                DurationInDays = 5,
                Price = 100,
                CategoryId = 1,
                CreatedByUserId = "testUser",
                ImagePath = "/images/120297bb-f922-45ad-9877-d6faf2cdb1cf.jpg"
            });
            _context.SaveChanges();

            _productRepository = new ProductRepository(_context);
            _productService = new ProductService(_productRepository);
        }

        [Fact]
        public async Task GetProductByIdAsync_Returns_Correct_Product()
        {
            var product = await _productService.GetProductByIdAsync(1);

            Assert.NotNull(product);
            Assert.Equal("Test Product", product.Name);
        }

        [Fact]
        public async Task GetActiveProductsAsync_Returns_Active_Products()
        {
            var products = await _productService.GetActiveProductsAsync();

            Assert.NotEmpty(products);
            var product = Assert.Single(products);
            Assert.Equal("Test Product", product.Name);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}