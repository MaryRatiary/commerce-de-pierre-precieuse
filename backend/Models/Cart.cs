using System.ComponentModel.DataAnnotations;

namespace EcomApi.Models;

public class Cart
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}

public class CartItem
{
    [Key]
    public int Id { get; set; }

    public int CartId { get; set; }
    public virtual Cart Cart { get; set; } = null!;

    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

    public int Quantity { get; set; }
}