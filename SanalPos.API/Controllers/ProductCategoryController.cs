using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanalPos.Domain.Entities;
using SanalPos.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace SanalPos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IApplicationDbContext _context;

        public ProductCategoryController(IApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetCategories()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductCategory>> GetCategory(Guid id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductCategory>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                DisplayOrder = request.DisplayOrder,
                IsActive = true,
                CreatedBy = User.Identity?.Name ?? "system",
                LastModifiedBy = User.Identity?.Name ?? "system",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync(default);

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        [HttpPost("seed")]
        [AllowAnonymous] // Geliştirme aşamasında test amaçlı
        public async Task<ActionResult> SeedSampleCategory()
        {
            // Test için varsayılan bir kategori oluştur
            var category = new ProductCategory
            {
                Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), // İstenilen sabit ID
                Name = "Test Kategorisi",
                Description = "Test amaçlı oluşturulmuş örnek kategori",
                ImageUrl = "https://example.com/image.jpg",
                DisplayOrder = 1,
                IsActive = true,
                CreatedBy = "system",
                LastModifiedBy = "system",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            // Eğer bu ID'ye sahip kategori yoksa ekle
            if (!await _context.ProductCategories.AnyAsync(c => c.Id == category.Id))
            {
                _context.ProductCategories.Add(category);
                await _context.SaveChangesAsync(default);
                return Ok("Test kategorisi başarıyla oluşturuldu.");
            }

            return Ok("Bu ID'ye sahip bir kategori zaten mevcut.");
        }
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }
} 