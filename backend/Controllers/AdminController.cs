using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EcomApi.Data;
using EcomApi.Models;
using EcomApi.DTOs;
using System.Linq;

namespace EcomApi.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/admin/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(bool? topLevelOnly = false)
        {
            var query = _context.Categories
                .Include(c => c.SubCategories)
                .AsQueryable();

            if (topLevelOnly == true)
            {
                query = query.Where(c => c.ParentCategoryId == null);
            }

            var categories = await query.ToListAsync();
            return Ok(MapCategoriesToDtos(categories));
        }

        // GET: api/admin/categories/{id}
        [HttpGet("categories/{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return MapCategoryToDto(category);
        }

        // POST: api/admin/categories
        [HttpPost("categories")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            var category = new Category
            {
                Name = createDto.Name,
                Description = createDto.Description,
                ImageUrl = createDto.ImageUrl,
                ParentCategoryId = createDto.ParentCategoryId
            };

            if (createDto.ParentCategoryId.HasValue)
            {
                var parentCategory = await _context.Categories.FindAsync(createDto.ParentCategoryId);
                if (parentCategory == null)
                {
                    return BadRequest("Catégorie parente non trouvée");
                }
                category.Level = parentCategory.Level + 1;
                category.Path = parentCategory.Path + "/" + createDto.Name;
            }
            else
            {
                category.Level = 0;
                category.Path = createDto.Name;
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, MapCategoryToDto(category));
        }

        // PUT: api/admin/categories/{id}
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = updateDto.Name;
            category.Description = updateDto.Description;
            category.ImageUrl = updateDto.ImageUrl;

            // Keep existing category hierarchy
            if (category.ParentCategoryId.HasValue)
            {
                var parent = await _context.Categories.FindAsync(category.ParentCategoryId);
                if (parent != null)
                {
                    category.Path = parent.Path + "/" + updateDto.Name;
                }
            }
            else
            {
                category.Path = updateDto.Name;
            }

            // Update paths of subcategories to reflect the name change
            await UpdateSubcategoriesPaths(category);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        // DELETE: api/admin/categories/{id}
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Supprimer récursivement toutes les sous-catégories
            await DeleteCategoryRecursive(category);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Méthodes privées d'aide
        private async Task DeleteCategoryRecursive(Category category)
        {
            foreach (var subcategory in category.SubCategories.ToList())
            {
                await DeleteCategoryRecursive(subcategory);
            }
            _context.Categories.Remove(category);
        }

        private async Task<bool> HasCycle(int categoryId, int newParentId)
        {
            var current = await _context.Categories.FindAsync(newParentId);
            while (current != null)
            {
                if (current.Id == categoryId)
                {
                    return true;
                }
                current = current.ParentCategoryId.HasValue ? 
                    await _context.Categories.FindAsync(current.ParentCategoryId) : null;
            }
            return false;
        }

        private async Task UpdateSubcategoriesPaths(Category category)
        {
            var subcategories = await _context.Categories
                .Where(c => c.ParentCategoryId == category.Id)
                .ToListAsync();

            foreach (var sub in subcategories)
            {
                sub.Path = category.Path + "/" + sub.Name;
                sub.Level = category.Level + 1;
                await UpdateSubcategoriesPaths(sub);
            }
        }

        private CategoryDto MapCategoryToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ParentCategoryId = category.ParentCategoryId,
                Path = category.Path,
                Level = category.Level,
                SubCategories = category.SubCategories
                    .Select(MapCategoryToDto)
                    .ToList()
            };
        }

        private List<CategoryDto> MapCategoriesToDtos(List<Category> categories)
        {
            return categories.Select(MapCategoryToDto).ToList();
        }

        // Autres méthodes existantes...

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardStats>> GetDashboardStats()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
            if (user == null || !user.IsAdmin)
                return Forbid();

            var stats = new DashboardStats
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .Select(o => new OrderSummaryDto
                    {
                        Id = o.Id,
                        Username = o.User.Username,
                        TotalAmount = o.TotalAmount,
                        OrderDate = o.OrderDate,
                        Status = o.Status
                    })
                    .ToListAsync(),
                TopSellingProducts = await _context.OrderItems
                    .GroupBy(oi => oi.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalSold = g.Count()
                    })
                    .OrderByDescending(x => x.TotalSold)
                    .Take(5)
                    .Join(
                        _context.Products,
                        g => g.ProductId,
                        p => p.Id,
                        (g, p) => new ProductStatsDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            TotalSold = g.TotalSold
                        }
                    )
                    .ToListAsync()
            };

            return Ok(stats);
        }

        [HttpPost("products")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
            if (user == null || !user.IsAdmin)
                return Forbid();

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", "Products", new { id = product.Id }, product);
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
            if (user == null || !user.IsAdmin)
                return Forbid();

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserStatsDto>>> GetUserStats()
        {
            var userStats = await _context.Users
                .Select(u => new UserStatsDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    OrderCount = u.Orders.Count,
                    TotalSpent = u.Orders.Sum(o => o.TotalAmount),
                    LastOrderDate = u.Orders.Max(o => (DateTime?)o.OrderDate)
                })
                .ToListAsync();

            return Ok(userStats);
        }

        [HttpPost("upload-image")]
        public async Task<ActionResult<string>> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier n'a été envoyé");

            // Vérifier le type de fichier
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return BadRequest("Type de fichier non autorisé. Utilisez JPG, PNG ou GIF.");

            try
            {
                // Créer le dossier uploads s'il n'existe pas
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Générer un nom de fichier unique
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Sauvegarder le fichier
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Retourner l'URL de l'image
                return Ok($"/uploads/{uniqueFileName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Une erreur est survenue lors du téléchargement du fichier");
            }
        }
    }
}