using Microsoft.EntityFrameworkCore;
using ProductCatalog.Core.Entities;
using ProductCatalog.Core.Interfaces;
using ProductCatalog.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync(DateTime currentTime)
        {
            return await _dbSet.Where(p => p.StartDate <= currentTime
                                           && p.StartDate.AddDays(p.DurationInDays) >= currentTime)
                                  .Include(p => p.Category)
                                  .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet.Where(p => p.CategoryId == categoryId)
                               .Include(p => p.Category)
                               .ToListAsync();
        }
        public async Task<IEnumerable<Product>> ListAllWithCategoriesAsync()
        {
            return await _dbSet.Include(p => p.Category).ToListAsync();
        }
        public async Task<Product> GetByIdWithCategoryAsync(int id)
        {
            return await _dbSet
                         .Include(p => p.Category)
                         .FirstOrDefaultAsync(p => p.Id == id);
        }

    }
}
