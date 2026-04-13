namespace EcomApi.DTOs;

public class DashboardStats
{
    public int TotalUsers { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public List<OrderSummaryDto> RecentOrders { get; set; } = new();
    public List<ProductStatsDto> TopSellingProducts { get; set; } = new();
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ProductStatsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalSold { get; set; }
}

public class UserStatsDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime? LastOrderDate { get; set; }
}