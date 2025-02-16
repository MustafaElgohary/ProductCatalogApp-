using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Core.Entities;
using ProductCatalog.Infrastructure.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Core.DTOs;
using ProductCatalog.Infrastructure.Services;

namespace ProductCatalog.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public ProductController(ProductService productService, CategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetActiveProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateProductDto dto, IFormFile imageFile)
        {

            if (imageFile != null)
            {
                if (!IsValidImage(imageFile))
                {
                    ModelState.AddModelError("ImageFile", "Invalid image type or size exceeds 1MB. The image must be of type JPG, JPEG, or PNG.");
                    ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                    return View(dto);
                }

                dto.ImagePath = await SaveImageFileAsync(imageFile);
            }

            var product = new Product
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                DurationInDays = dto.DurationInDays,
                Price = dto.Price,
                ImagePath = dto.ImagePath,
                CategoryId = dto.CategoryId,
                CreatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            };

            await _productService.AddProductAsync(product);
            return RedirectToAction(nameof(AdminIndex));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var updateDto = new UpdateProductDto
            {
                Id = product.Id,
                Name = product.Name,
                StartDate = product.StartDate,
                DurationInDays = product.DurationInDays,
                Price = product.Price,
                ImagePath = product.ImagePath,
                CategoryId = product.CategoryId
            };

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(updateDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(UpdateProductDto dto, IFormFile imageFile)
        {
            var product = await _productService.GetProductByIdAsync(dto.Id);
            if (product == null)
                return NotFound();

            product.Name = dto.Name;
            product.StartDate = dto.StartDate;
            product.DurationInDays = dto.DurationInDays;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            if (imageFile != null)
            {
                if (!IsValidImage(imageFile))
                {
                    ModelState.AddModelError("ImageFile", "Invalid image type or size exceeds 1MB. The image must be of type JPG, JPEG, or PNG.");
                    ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                    return View(dto);
                }
                product.ImagePath = await SaveImageFileAsync(imageFile);
            }

            product.UpdatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _productService.UpdateProductAsync(product);
            return RedirectToAction(nameof(AdminIndex));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            await _productService.DeleteProductAsync(product);
            return RedirectToAction(nameof(AdminIndex));
        }

        public async Task<IActionResult> FilterByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return View("Index", products);
        }

        #region Helper Methods

        private bool IsValidImage(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return false;
            if (file.Length > 1024 * 1024) // 1MB
                return false;
            return true;
        }

        private async Task<string> SaveImageFileAsync(IFormFile file)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "/images/" + fileName;
        }

        #endregion
    }
}
