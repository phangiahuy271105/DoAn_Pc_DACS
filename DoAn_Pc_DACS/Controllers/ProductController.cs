using DoAn_Pc_DACS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DoAn_Pc_DACS.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hàm này sẽ hứng toàn bộ các tham số từ form bộ lọc truyền lên URL
        public IActionResult Index(string slug, string priceRange, string[] cpuBrand, string sortBy, string keyword)
        {
            var query = _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.ComponentSpec)
                                .AsQueryable();

            // 1. Xử lý Lọc theo Danh mục (Slug)
            if (!string.IsNullOrEmpty(slug))
            {
                string searchSlug = slug.ToLower();
                query = query.Where(p => p.Category != null && (
                    (p.Category.Slug != null && p.Category.Slug.ToLower() == searchSlug) ||
                    (p.Category.Name != null && p.Category.Name.ToLower().Contains(searchSlug.Replace("-", " ")))
                ));
            }

            // 2. Xử lý Lọc theo Khoảng Giá
            if (!string.IsNullOrEmpty(priceRange))
            {
                var prices = priceRange.Split('-');
                if (prices.Length == 2 && decimal.TryParse(prices[0], out decimal minPrice) && decimal.TryParse(prices[1], out decimal maxPrice))
                {
                    query = query.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                }
            }

            // 3. Xử lý Lọc theo Dòng CPU
            if (cpuBrand != null && cpuBrand.Length > 0)
            {
                bool hasIntel = cpuBrand.Contains("Intel");
                bool hasAmd = cpuBrand.Contains("AMD");

                if (hasIntel && !hasAmd)
                {
                    query = query.Where(p => p.ComponentSpec != null && (p.ComponentSpec.Socket.Contains("Intel") || p.ComponentSpec.Socket.Contains("Core")));
                }
                else if (hasAmd && !hasIntel)
                {
                    query = query.Where(p => p.ComponentSpec != null && (p.ComponentSpec.Socket.Contains("AMD") || p.ComponentSpec.Socket.Contains("Ryzen")));
                }
            }

            // 4. Xử lý Tìm kiếm theo từ khóa (Keyword)
            if (!string.IsNullOrEmpty(keyword))
            {
                string kw = keyword.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(kw)
                                      || (p.ComponentSpec != null && p.ComponentSpec.Socket != null && p.ComponentSpec.Socket.ToLower().Contains(kw))
                                      || (p.ComponentSpec != null && p.ComponentSpec.Vga != null && p.ComponentSpec.Vga.ToLower().Contains(kw)));
            }

            // 5. Xử lý Sắp xếp
            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                default:
                    query = query.OrderByDescending(p => p.Id);
                    break;
            }

            var products = query.ToList();

            ViewBag.CurrentSlug = slug;
            ViewBag.CurrentPrice = priceRange;
            ViewBag.CurrentCpu = cpuBrand;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentKeyword = keyword;

            return View(products);
        } // <--- ĐÓNG NGOẶC HÀM INDEX CHUẨN Ở ĐÂY NÀY!

        // ==========================================
        // HÀM LIVE SEARCH (Nằm hoàn toàn bên ngoài hàm Index, không bị lỗi vặt)
        [HttpGet]
        public IActionResult SearchSuggest(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Json(new { success = false });
            }

            string kw = keyword.ToLower();
            var query = _context.Products.Where(p => p.Name.ToLower().Contains(kw));
            int totalCount = query.Count();

            var products = query.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                price = p.Price.ToString("N0") + " VNĐ",
                imageUrl = p.ImageUrl
            }).Take(5).ToList();

            return Json(new { success = true, items = products, total = totalCount });
        }
    }
}
