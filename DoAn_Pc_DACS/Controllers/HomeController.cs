using DoAn_Pc_DACS.Data;
using DoAn_Pc_DACS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace DoAn_Pc_DACS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                                   .Include(p => p.Category)
                                   .Include(p => p.ComponentSpec)
                                   .ToList();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                                  .Include(p => p.Category)
                                  .Include(p => p.ComponentSpec)
                                  .Include(p => p.ProductImages)
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            const int recommendationCount = 8;
            var similarProducts = await _context.Products
                .AsNoTracking()
                .Where(candidate => candidate.Id != product.Id && candidate.CategoryId == product.CategoryId)
                .OrderBy(_ => Guid.NewGuid())
                .Take(recommendationCount)
                .ToListAsync();

            // Nếu danh mục còn ít sản phẩm, bổ sung ngẫu nhiên từ danh mục khác nhưng vẫn không trùng ID.
            if (similarProducts.Count < recommendationCount)
            {
                var excludedIds = similarProducts.Select(candidate => candidate.Id).Append(product.Id).ToList();
                var additionalProducts = await _context.Products
                    .AsNoTracking()
                    .Where(candidate => !excludedIds.Contains(candidate.Id))
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(recommendationCount - similarProducts.Count)
                    .ToListAsync();

                similarProducts.AddRange(additionalProducts);
            }

            var bundleProducts = await _context.ProductRelations
                .AsNoTracking()
                .Where(relation => relation.ProductId == product.Id)
                .OrderBy(relation => relation.DisplayOrder)
                .Select(relation => new BundleProductViewModel
                {
                    RelationId = relation.Id,
                    ProductId = relation.RelatedProduct.Id,
                    CategoryId = relation.RelatedProduct.CategoryId,
                    CategoryName = relation.RelatedProduct.Category.Name,
                    Name = relation.RelatedProduct.Name,
                    ImageUrl = relation.RelatedProduct.ImageUrl,
                    Price = relation.RelatedProduct.Price,
                    OldPrice = relation.RelatedProduct.OldPrice,
                    StockQuantity = relation.RelatedProduct.StockQuantity
                })
                .Take(5)
                .ToListAsync();

            ViewBag.SimilarProducts = similarProducts;
            ViewBag.BundleProducts = bundleProducts;
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> BundleAlternatives(int relationId, string? keyword, string sort = "newest")
        {
            var relation = await _context.ProductRelations
                .AsNoTracking()
                .Where(item => item.Id == relationId)
                .Select(item => new
                {
                    item.ProductId,
                    CategoryId = item.RelatedProduct.CategoryId,
                    CategoryName = item.RelatedProduct.Category.Name
                })
                .FirstOrDefaultAsync();

            if (relation == null)
            {
                return NotFound();
            }

            var query = _context.Products
                .AsNoTracking()
                .Where(item => item.CategoryId == relation.CategoryId &&
                               item.Id != relation.ProductId &&
                               item.StockQuantity > 0);

            keyword = keyword?.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(item => item.Name.Contains(keyword));
            }

            query = sort switch
            {
                "price-asc" => query.OrderBy(item => item.Price),
                "price-desc" => query.OrderByDescending(item => item.Price),
                _ => query.OrderByDescending(item => item.Id)
            };

            var items = await query
                .Take(40)
                .Select(item => new
                {
                    id = item.Id,
                    item.Name,
                    item.ImageUrl,
                    item.Price,
                    item.OldPrice,
                    saving = item.OldPrice > item.Price ? item.OldPrice - item.Price : 0,
                    item.StockQuantity
                })
                .ToListAsync();

            return Json(new { categoryName = relation.CategoryName, items });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
