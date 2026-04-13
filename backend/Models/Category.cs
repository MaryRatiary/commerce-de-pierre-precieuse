using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcomApi.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    // Relation parent-enfant pour les sous-catégories
    public int? ParentCategoryId { get; set; }

    [ForeignKey("ParentCategoryId")]
    public Category? ParentCategory { get; set; }

    // Collection des sous-catégories
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();

    // Collection des produits dans cette catégorie
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    // Level dans la hiérarchie (0 = catégorie principale, 1 = première sous-catégorie, etc.)
    public int Level { get; set; }

    // Chemin complet de la catégorie (ex: "Bijoux/Bagues/Or")
    public string Path { get; set; } = string.Empty;
}