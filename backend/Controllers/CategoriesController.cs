using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcomApi.Data;
using EcomApi.DTOs;
using EcomApi.Models;

namespace EcomApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<CategoryDto> MapCategoryWithSubCategories(Category category)
    {
        // Charger les sous-catégories si elles existent
        await _context.Entry(category)
            .Collection(c => c.SubCategories)
            .LoadAsync();

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            ParentCategoryId = category.ParentCategoryId,
            Path = category.Path,
            Level = category.Level,
            ProductCount = category.Products.Count,
            SubCategories = new List<CategoryDto>()
        };

        // Récursivement charger toutes les sous-catégories
        if (category.SubCategories?.Any() == true)
        {
            foreach (var subCategory in category.SubCategories)
            {
                dto.SubCategories.Add(await MapCategoryWithSubCategories(subCategory));
            }
        }

        return dto;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(bool? topLevelOnly = false)
    {
        var query = _context.Categories
            .Include(c => c.Products)
            .AsQueryable();

        if (topLevelOnly == true)
        {
            query = query.Where(c => c.ParentCategoryId == null);
        }

        var categories = await query.ToListAsync();
        var dtos = new List<CategoryDto>();

        foreach (var category in categories)
        {
            dtos.Add(await MapCategoryWithSubCategories(category));
        }

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(await MapCategoryWithSubCategories(category));
    }

    [HttpGet("{id}/products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetCategoryProducts(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return Ok(category.Products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            ImageUrl = p.ImageUrl,
            CategoryId = p.CategoryId,
            CategoryName = category.Name
        }));
    }
}