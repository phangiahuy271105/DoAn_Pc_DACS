using DoAn_Pc_DACS.Models;
using System.Linq;

namespace DoAn_Pc_DACS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Categories.Any())
            {
                return;
            }

            var categories = new Category[]
            {
                new Category { Name = "PC GAMING", Slug = "pc-gaming" },
                new Category { Name = "PC WORKSTATION 2D 3D", Slug = "pc-workstation-2d-3d" },
                new Category { Name = "Linh kiện máy tính", Slug = "linh-kien-may-tinh" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            var products = new Product[]
 {
    new Product { Name = "PC TTG AMD GAMING LUXURY RYZEN 7 9800X3D - RTX 5070 Ti 16GB", Price = 79990000 , OldPrice = 78680000 , Discount = 10, ImageUrl = "/images/PC_amd/RYZEN 7 9800X3D - RTX 5070 Ti 16GB/AMD_ryzen7.png", CategoryId = 1 },
    new Product { Name = "PC TTG Designer 3D Render - Edit Video i7", Price = 50980000, OldPrice = 52980000, Discount = 4, ImageUrl = "/images/PC_workstation/ULTRA7.jpg", CategoryId = 2 },
    new Product { Name = "CPU Intel Core Ultra 5 250K Plus - Tray", Price = 7690000, OldPrice = 8280000, Discount = 7, ImageUrl = "/images/Linh_kien/cpu_intel_core_ultra_5.jpg", CategoryId = 3 }
 };
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}