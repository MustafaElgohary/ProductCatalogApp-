using ProductCatalog.Core.Entities;
using ProductCatalog.Core.Interfaces;

namespace ProductCatalog.Infrastructure.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.ListAllAsync();
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _categoryRepository.GetByNameAsync(name);
        }
    }
}
