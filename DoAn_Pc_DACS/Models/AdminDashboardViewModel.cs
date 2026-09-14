namespace DoAn_Pc_DACS.Models;

public class AdminDashboardViewModel
{
    public int ProductCount { get; set; }
    public int OrderCount { get; set; }
    public int PendingOrderCount { get; set; }
    public decimal CompletedRevenue { get; set; }
    public int OutOfStockCount { get; set; }
    public List<Product> LowStockProducts { get; set; } = [];
    public List<Order> RecentOrders { get; set; } = [];
}
