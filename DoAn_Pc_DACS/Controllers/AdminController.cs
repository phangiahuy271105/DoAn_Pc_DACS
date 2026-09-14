using Microsoft.AspNetCore.Authorization;
using DoAn_Pc_DACS.Data;
using DoAn_Pc_DACS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DoAn_Pc_DACS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private static readonly string[] PcCategorySlugs = ["pc-gaming", "pc-workstation", "pc-workstation-2d-3d"];
        private static readonly string[] BundleCategorySlugs = ["man-hinh", "ban-phim", "chuot", "tai-nghe"];
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Dashboard()
        {
            var products = _context.Products.AsNoTracking();
            var orders = _context.Orders.AsNoTracking();
            var model = new AdminDashboardViewModel
            {
                ProductCount = await products.CountAsync(),
                OrderCount = await orders.CountAsync(),
                PendingOrderCount = await orders.CountAsync(order => order.Status == "Chờ xác nhận"),
                CompletedRevenue = await orders.Where(order => order.Status == "Hoàn tất")
                    .SumAsync(order => (decimal?)order.TotalAmount) ?? 0,
                OutOfStockCount = await products.CountAsync(product => product.StockQuantity <= 0),
                LowStockProducts = await products.Where(product => product.StockQuantity > 0 && product.StockQuantity <= 5)
                    .Include(product => product.Category)
                    .OrderBy(product => product.StockQuantity).ThenBy(product => product.Name).ToListAsync(),
                RecentOrders = await orders.OrderByDescending(order => order.OrderDate)
                    .ThenByDescending(order => order.Id).Take(10).ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Index(string group = "all", string? q = null)
        {
            string[] validGroups =
            [
                "all", "gaming", "workstation", "components", "monitor", "keyboard", "mouse", "headset"
            ];
            group = validGroups.Contains(group) ? group : "all";
            q = q?.Trim();

            var baseQuery = _context.Products
                .AsNoTracking()
                .Include(product => product.Category)
                .Include(product => product.ComponentSpec)
                .Include(product => product.ProductImages)
                .AsSplitQuery();

            var counts = new Dictionary<string, int>
            {
                ["all"] = await baseQuery.CountAsync(),
                ["gaming"] = await baseQuery.CountAsync(product => product.Category.Slug == "pc-gaming"),
                ["workstation"] = await baseQuery.CountAsync(product =>
                    product.Category.Slug == "pc-workstation" || product.Category.Slug == "pc-workstation-2d-3d"),
                ["components"] = await baseQuery.CountAsync(product => product.Category.Slug == "linh-kien-may-tinh"),
                ["monitor"] = await baseQuery.CountAsync(product => product.Category.Slug == "man-hinh"),
                ["keyboard"] = await baseQuery.CountAsync(product => product.Category.Slug == "ban-phim"),
                ["mouse"] = await baseQuery.CountAsync(product => product.Category.Slug == "chuot"),
                ["headset"] = await baseQuery.CountAsync(product => product.Category.Slug == "tai-nghe")
            };

            IQueryable<Product> productsQuery = group switch
            {
                "gaming" => baseQuery.Where(product => product.Category.Slug == "pc-gaming"),
                "workstation" => baseQuery.Where(product =>
                    product.Category.Slug == "pc-workstation" || product.Category.Slug == "pc-workstation-2d-3d"),
                "components" => baseQuery.Where(product => product.Category.Slug == "linh-kien-may-tinh"),
                "monitor" => baseQuery.Where(product => product.Category.Slug == "man-hinh"),
                "keyboard" => baseQuery.Where(product => product.Category.Slug == "ban-phim"),
                "mouse" => baseQuery.Where(product => product.Category.Slug == "chuot"),
                "headset" => baseQuery.Where(product => product.Category.Slug == "tai-nghe"),
                _ => baseQuery
            };

            if (!string.IsNullOrWhiteSpace(q))
            {
                productsQuery = productsQuery.Where(product => product.Name.Contains(q));
            }

            var viewModel = new AdminProductIndexViewModel
            {
                Products = await productsQuery.OrderByDescending(product => product.Id).ToListAsync(),
                GroupCounts = counts,
                CurrentGroup = group,
                SearchTerm = q ?? string.Empty
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CreateProduct()
        {
            var viewModel = new ProductCreateViewModel
            {
                Categories = await GetCategoryOptionsAsync(),
                AvailableRelatedProducts = await GetRelatedProductOptionsAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductCreateViewModel model)
        {
            await ValidateProductImagesAsync(model.ImageFile, model.GalleryFiles);
            ValidateProductData(model.Price, model.OldPrice, model.CategoryId);
            if (!await _context.Categories.AnyAsync(c => c.Id == model.CategoryId && c.Slug == "linh-kien-may-tinh"))
                model.ComponentType = null;
            if (model.ComponentType is not ("CPU" or "Mainboard")) model.BuildSocket = null;
            if (model.ComponentType is not ("RAM" or "Mainboard")) model.BuildMemoryType = null;
            bool isPcCategory = await IsPcCategoryAsync(model.CategoryId);
            if (!isPcCategory) model.RelatedProductIds.Clear();
            model.RelatedProductIds = model.RelatedProductIds.Distinct().ToList();
            await ValidateRelatedProductsAsync(0, model.CategoryId, model.RelatedProductIds);

            if (ModelState.IsValid)
            {
                var savedImageUrls = new List<string>();
                try
                {
                    string mainImageUrl = await SaveImageAsync(model.ImageFile);
                    savedImageUrls.Add(mainImageUrl);

                    var product = new Product
                    {
                        Name = model.Name.Trim(),
                        Price = model.Price,
                        OldPrice = model.OldPrice,
                        Discount = Product.CalculateDiscount(model.Price, model.OldPrice),
                        StockQuantity = model.StockQuantity,
                        CategoryId = model.CategoryId,
                        ComponentType = NormalizeOptionalText(model.ComponentType),
                        BuildSocket = NormalizeOptionalText(model.BuildSocket),
                        BuildMemoryType = NormalizeOptionalText(model.BuildMemoryType),
                        ImageUrl = mainImageUrl,
                        Description = NormalizeOptionalText(model.Description),
                        TechnicalSpecifications = NormalizeOptionalText(model.TechnicalSpecifications),
                        ComponentSpec = isPcCategory ? CreateComponentSpec(model) : null,
                        ProductImages = new List<ProductImage>(),
                        RelatedProducts = model.RelatedProductIds.Select((relatedId, index) => new ProductRelation
                        {
                            RelatedProductId = relatedId,
                            DisplayOrder = index
                        }).ToList()
                    };

                    foreach (var file in model.GalleryFiles ?? [])
                    {
                        string galleryUrl = await SaveImageAsync(file);
                        savedImageUrls.Add(galleryUrl);
                        product.ProductImages.Add(new ProductImage { ImageUrl = galleryUrl });
                    }

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Admin");
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DbUpdateException)
                {
                    foreach (string imageUrl in savedImageUrls) DeleteUploadedImage(imageUrl);
                    ModelState.AddModelError(string.Empty, "Không thể lưu sản phẩm hoặc ảnh. Vui lòng thử lại.");
                }
            }

            model.Categories = await GetCategoryOptionsAsync();
            model.AvailableRelatedProducts = await GetRelatedProductOptionsAsync();

            return View(model);
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ComponentSpec)
                .Include(p => p.ProductImages)
                .Include(p => p.RelatedProducts)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                OldPrice = product.OldPrice,
                Discount = product.Discount,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                ComponentType = product.ComponentType,
                BuildSocket = product.BuildSocket,
                BuildMemoryType = product.BuildMemoryType,
                Description = product.Description,
                TechnicalSpecifications = product.TechnicalSpecifications,
                ExistingImageUrl = product.ImageUrl,
                ExistingGalleryImages = product.ProductImages.ToList(),
                RelatedProductIds = product.RelatedProducts
                    .OrderBy(relation => relation.DisplayOrder)
                    .Select(relation => relation.RelatedProductId)
                    .ToList(),

                Socket = product.ComponentSpec?.Socket,
                SocketSl = product.ComponentSpec?.SocketSl ?? 1,
                SocketBh = product.ComponentSpec?.SocketBh ?? "36 Tháng",
                Mainboard = product.ComponentSpec?.Mainboard,
                MainboardSl = product.ComponentSpec?.MainboardSl ?? 1,
                MainboardBh = product.ComponentSpec?.MainboardBh ?? "36 Tháng",
                RamType = product.ComponentSpec?.RamType,
                RamSl = product.ComponentSpec?.RamSl ?? 1,
                RamBh = product.ComponentSpec?.RamBh ?? "36 Tháng",
                Storage = product.ComponentSpec?.Storage,
                StorageSl = product.ComponentSpec?.StorageSl ?? 1,
                StorageBh = product.ComponentSpec?.StorageBh ?? "36 Tháng",
                PowerSupply = product.ComponentSpec?.PowerSupply,
                PowerSupplySl = product.ComponentSpec?.PowerSupplySl ?? 1,
                PowerSupplyBh = product.ComponentSpec?.PowerSupplyBh ?? "36 Tháng",
                Vga = product.ComponentSpec?.Vga,
                VgaSl = product.ComponentSpec?.VgaSl ?? 1,
                VgaBh = product.ComponentSpec?.VgaBh ?? "36 Tháng",
                FormFactor = product.ComponentSpec?.FormFactor,
                FormFactorSl = product.ComponentSpec?.FormFactorSl ?? 1,
                FormFactorBh = product.ComponentSpec?.FormFactorBh ?? "12 Tháng",
                Cooler = product.ComponentSpec?.Cooler,
                CoolerSl = product.ComponentSpec?.CoolerSl ?? 1,
                CoolerBh = product.ComponentSpec?.CoolerBh ?? "12 Tháng",

                Categories = await GetCategoryOptionsAsync(),
                AvailableRelatedProducts = await GetRelatedProductOptionsAsync(product.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductEditViewModel model)
        {
            await ValidateProductImagesAsync(model.ImageFile, model.GalleryFiles, requireMainImage: false);
            ValidateProductData(model.Price, model.OldPrice, model.CategoryId);
            if (!await _context.Categories.AnyAsync(c => c.Id == model.CategoryId && c.Slug == "linh-kien-may-tinh"))
                model.ComponentType = null;
            if (model.ComponentType is not ("CPU" or "Mainboard")) model.BuildSocket = null;
            if (model.ComponentType is not ("RAM" or "Mainboard")) model.BuildMemoryType = null;
            bool isPcCategory = await IsPcCategoryAsync(model.CategoryId);
            if (!isPcCategory) model.RelatedProductIds.Clear();
            model.RelatedProductIds = model.RelatedProductIds.Distinct().ToList();
            await ValidateRelatedProductsAsync(model.Id, model.CategoryId, model.RelatedProductIds);

            if (ModelState.IsValid)
            {
                var product = await _context.Products
                    .Include(p => p.ComponentSpec)
                    .Include(p => p.ProductImages)
                    .Include(p => p.RelatedProducts)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.Id == model.Id);

                if (product == null) return NotFound();

                var savedImageUrls = new List<string>();
                var oldImageUrlsToDelete = new List<string>();
                product.ProductImages ??= new List<ProductImage>();

                if (model.DeleteOldGallery && product.ProductImages.Any())
                {
                    oldImageUrlsToDelete.AddRange(product.ProductImages.Select(image => image.ImageUrl));
                    _context.ProductImages.RemoveRange(product.ProductImages);
                    product.ProductImages.Clear();
                }

                try
                {
                    if (model.ImageFile != null)
                    {
                        string newMainImageUrl = await SaveImageAsync(model.ImageFile);
                        savedImageUrls.Add(newMainImageUrl);
                        oldImageUrlsToDelete.Add(product.ImageUrl);
                        product.ImageUrl = newMainImageUrl;
                    }

                    foreach (var file in model.GalleryFiles ?? [])
                    {
                        string galleryUrl = await SaveImageAsync(file);
                        savedImageUrls.Add(galleryUrl);
                        product.ProductImages.Add(new ProductImage { ImageUrl = galleryUrl });
                    }

                    product.Name = model.Name.Trim();
                    product.Price = model.Price;
                    product.OldPrice = model.OldPrice;
                    product.Discount = Product.CalculateDiscount(model.Price, model.OldPrice);
                    product.StockQuantity = model.StockQuantity;
                    product.CategoryId = model.CategoryId;
                    product.ComponentType = NormalizeOptionalText(model.ComponentType);
                    product.BuildSocket = NormalizeOptionalText(model.BuildSocket);
                    product.BuildMemoryType = NormalizeOptionalText(model.BuildMemoryType);
                    product.Description = NormalizeOptionalText(model.Description);
                    product.TechnicalSpecifications = NormalizeOptionalText(model.TechnicalSpecifications);

                    if (isPcCategory)
                    {
                        product.ComponentSpec ??= new ComponentSpec();
                        ApplyComponentSpec(product.ComponentSpec, model);
                    }
                    else if (product.ComponentSpec != null)
                    {
                        _context.ComponentSpecs.Remove(product.ComponentSpec);
                        product.ComponentSpec = null;
                    }

                    _context.ProductRelations.RemoveRange(product.RelatedProducts);
                    product.RelatedProducts = model.RelatedProductIds.Select((relatedId, index) => new ProductRelation
                    {
                        ProductId = product.Id,
                        RelatedProductId = relatedId,
                        DisplayOrder = index
                    }).ToList();

                    await _context.SaveChangesAsync();

                    foreach (string imageUrl in oldImageUrlsToDelete) DeleteUploadedImage(imageUrl);

                    return RedirectToAction("Index", "Admin");
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DbUpdateException)
                {
                    foreach (string imageUrl in savedImageUrls) DeleteUploadedImage(imageUrl);
                    _context.ChangeTracker.Clear();
                    ModelState.AddModelError(string.Empty, "Không thể cập nhật sản phẩm hoặc ảnh. Vui lòng thử lại.");
                }
            }

            await PopulateEditViewModelAsync(model);

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (await _context.OrderDetails.AnyAsync(detail => detail.ProductId == id))
            {
                TempData["AdminError"] = "Không thể xóa sản phẩm đã xuất hiện trong đơn hàng. Hãy đặt tồn kho về 0 để ngừng bán.";
                return RedirectToAction("Index");
            }

            var product = await _context.Products
                .Include(p => p.ComponentSpec)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var incomingRelations = await _context.ProductRelations
                .Where(relation => relation.RelatedProductId == id)
                .ToListAsync();
            _context.ProductRelations.RemoveRange(incomingRelations);

            var imageUrlsToDelete = product.ProductImages.Select(image => image.ImageUrl).ToList();
            imageUrlsToDelete.Add(product.ImageUrl);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            foreach (string imageUrl in imageUrlsToDelete) DeleteUploadedImage(imageUrl);

            return RedirectToAction("Index", "Admin");
        }
        // ==========================================
        // QUẢN LÝ ĐƠN HÀNG
        // ==========================================

        // 1. Hiển thị danh sách Đơn hàng
        public IActionResult Orders()
        {
            // Lấy tất cả đơn hàng, sắp xếp mới nhất lên đầu
            var orders = _context.Orders
                                 .Include(o => o.OrderDetails) // Lọc chi tiết để biết 1 đơn có bao nhiêu món
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToList();
            return View(orders);
        }

        public IActionResult OrderDetails(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(detail => detail.Product)
                .FirstOrDefault(o => o.Id == id);

            return order == null ? NotFound() : View(order);
        }

        // 2. Cập nhật trạng thái Đơn hàng 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string newStatus)
        {
            string[] allowedStatuses = ["Chờ xác nhận", "Đang giao", "Hoàn tất", "Đã hủy"];
            if (!allowedStatuses.Contains(newStatus))
            {
                return BadRequest(new { success = false, message = "Trạng thái không hợp lệ." });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(detail => detail.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy đơn hàng!" });
            }

            if (order.Status == newStatus)
            {
                return Json(new { success = true, message = "Trạng thái không thay đổi." });
            }

            bool wasCancelled = order.Status == "Đã hủy";
            bool isBeingCancelled = newStatus == "Đã hủy";

            if (!wasCancelled && isBeingCancelled)
            {
                foreach (var detail in order.OrderDetails.Where(detail => detail.Product != null))
                {
                    detail.Product.StockQuantity += detail.Quantity;
                }
            }
            else if (wasCancelled && !isBeingCancelled)
            {
                var insufficientProduct = order.OrderDetails
                    .FirstOrDefault(detail => detail.Product == null || detail.Product.StockQuantity < detail.Quantity);

                if (insufficientProduct != null)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new
                    {
                        success = false,
                        message = "Không đủ tồn kho để khôi phục đơn hàng đã hủy."
                    });
                }

                foreach (var detail in order.OrderDetails)
                {
                    detail.Product.StockQuantity -= detail.Quantity;
                }
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Json(new { success = true, message = "Cập nhật thành công!" });
        }

        private void ValidateProductData(decimal price, decimal oldPrice, int categoryId)
        {
            if (oldPrice > 0 && oldPrice < price)
            {
                ModelState.AddModelError("OldPrice", "Giá gốc không được thấp hơn giá bán.");
            }

            if (!_context.Categories.Any(category => category.Id == categoryId))
            {
                ModelState.AddModelError("CategoryId", "Danh mục đã chọn không tồn tại.");
            }
        }

        private async Task ValidateRelatedProductsAsync(int productId, int productCategoryId, List<int> relatedProductIds)
        {
            if (relatedProductIds.Count > 5)
            {
                ModelState.AddModelError("RelatedProductIds", "Chỉ được chọn tối đa 5 sản phẩm mua kèm.");
            }

            if (productId > 0 && relatedProductIds.Contains(productId))
            {
                ModelState.AddModelError("RelatedProductIds", "Sản phẩm không thể mua kèm chính nó.");
            }

            var relatedProducts = await _context.Products
                .AsNoTracking()
                .Where(product => relatedProductIds.Contains(product.Id))
                .Select(product => new { product.Id, product.CategoryId, CategorySlug = product.Category.Slug })
                .ToListAsync();

            if (relatedProducts.Count != relatedProductIds.Count)
            {
                ModelState.AddModelError("RelatedProductIds", "Có sản phẩm mua kèm không còn tồn tại.");
            }

            if (relatedProducts.Any(product => !BundleCategorySlugs.Contains(product.CategorySlug)))
            {
                ModelState.AddModelError("RelatedProductIds", "Sản phẩm mua kèm chỉ gồm màn hình, bàn phím, chuột hoặc tai nghe.");
            }

            if (relatedProducts.Any(product => product.CategoryId == productCategoryId))
            {
                ModelState.AddModelError("RelatedProductIds", "Sản phẩm mua kèm phải thuộc danh mục khác sản phẩm chính.");
            }

            if (relatedProducts.Select(product => product.CategoryId).Distinct().Count() != relatedProducts.Count)
            {
                ModelState.AddModelError("RelatedProductIds", "Mỗi danh mục chỉ được chọn một sản phẩm mua kèm mặc định.");
            }
        }

        private async Task<List<SelectListItem>> GetRelatedProductOptionsAsync(int excludedProductId = 0)
        {
            return await _context.Products
                .AsNoTracking()
                .Where(product => product.Id != excludedProductId && BundleCategorySlugs.Contains(product.Category.Slug))
                .OrderBy(product => product.Category.Name)
                .ThenBy(product => product.Name)
                .Select(product => new SelectListItem
                {
                    Value = product.Id.ToString(),
                    Text = product.Category.Name + " — " + product.Name
                })
                .ToListAsync();
        }

        private Task<bool> IsPcCategoryAsync(int categoryId)
        {
            return _context.Categories
                .AnyAsync(category => category.Id == categoryId && PcCategorySlugs.Contains(category.Slug));
        }

        private async Task<List<SelectListItem>> GetCategoryOptionsAsync()
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            string[] displayOrder =
            [
                "pc-gaming", "pc-workstation", "pc-workstation-2d-3d",
                "linh-kien-may-tinh", "man-hinh", "ban-phim", "chuot", "tai-nghe"
            ];

            return categories
                .OrderBy(category => Array.IndexOf(displayOrder, category.Slug) is int index && index >= 0 ? index : int.MaxValue)
                .ThenBy(category => category.Name)
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                })
                .ToList();
        }

        private static string? NormalizeOptionalText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private async Task ValidateProductImagesAsync(
            IFormFile? mainImage,
            List<IFormFile>? galleryFiles,
            bool requireMainImage = true)
        {
            if (requireMainImage && mainImage == null)
            {
                ModelState.AddModelError("ImageFile", "Vui lòng chọn ảnh đại diện.");
            }

            if (mainImage != null && !await IsValidImageAsync(mainImage))
            {
                ModelState.AddModelError("ImageFile", "Ảnh đại diện phải là JPG, PNG hoặc WEBP và không vượt quá 5 MB.");
            }

            if (galleryFiles != null && galleryFiles.Count > 8)
            {
                ModelState.AddModelError("GalleryFiles", "Mỗi lần chỉ được tải lên tối đa 8 ảnh phụ.");
            }

            foreach (var file in galleryFiles ?? [])
            {
                if (!await IsValidImageAsync(file))
                {
                    ModelState.AddModelError("GalleryFiles", $"Ảnh phụ \"{Path.GetFileName(file.FileName)}\" không hợp lệ.");
                }
            }
        }

        private static async Task<bool> IsValidImageAsync(IFormFile file)
        {
            const long maxImageSize = 5 * 1024 * 1024;
            string extension = Path.GetExtension(Path.GetFileName(file.FileName)).ToLowerInvariant();

            if (file.Length <= 0 || file.Length > maxImageSize ||
                extension is not (".jpg" or ".jpeg" or ".png" or ".webp"))
            {
                return false;
            }

            byte[] header = new byte[12];
            await using var stream = file.OpenReadStream();
            int bytesRead = await stream.ReadAsync(header.AsMemory(0, header.Length));

            bool isJpeg = bytesRead >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
            bool isPng = bytesRead >= 8 &&
                         header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                         header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;
            bool isWebP = bytesRead >= 12 &&
                          header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                          header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;

            return extension switch
            {
                ".jpg" or ".jpeg" => isJpeg,
                ".png" => isPng,
                ".webp" => isWebP,
                _ => false
            };
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            string extension = Path.GetExtension(Path.GetFileName(file.FileName)).ToLowerInvariant();
            string fileName = $"{Guid.NewGuid():N}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            await using var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(fileStream);

            return $"/images/uploads/{fileName}";
        }

        private void DeleteUploadedImage(string? imageUrl)
        {
            const string uploadsPrefix = "/images/uploads/";
            if (string.IsNullOrWhiteSpace(imageUrl) ||
                !imageUrl.StartsWith(uploadsPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string fileName = imageUrl[uploadsPrefix.Length..];
            if (!string.Equals(fileName, Path.GetFileName(fileName), StringComparison.Ordinal))
            {
                return;
            }

            string uploadsFolder = Path.GetFullPath(Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads"));
            string filePath = Path.GetFullPath(Path.Combine(uploadsFolder, fileName));
            string safeRoot = uploadsFolder.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            if (filePath.StartsWith(safeRoot, StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        private async Task PopulateEditViewModelAsync(ProductEditViewModel model)
        {
            model.Categories = await GetCategoryOptionsAsync();
            model.AvailableRelatedProducts = await GetRelatedProductOptionsAsync(model.Id);

            var existingProduct = await _context.Products
                .AsNoTracking()
                .Include(product => product.ProductImages)
                .FirstOrDefaultAsync(product => product.Id == model.Id);

            if (existingProduct != null)
            {
                model.ExistingImageUrl = existingProduct.ImageUrl;
                model.ExistingGalleryImages = existingProduct.ProductImages.ToList();
            }
        }

        private static void ApplyComponentSpec(ComponentSpec componentSpec, ProductEditViewModel model)
        {
            componentSpec.Socket = model.Socket;
            componentSpec.SocketSl = model.SocketSl;
            componentSpec.SocketBh = model.SocketBh;
            componentSpec.Mainboard = model.Mainboard;
            componentSpec.MainboardSl = model.MainboardSl;
            componentSpec.MainboardBh = model.MainboardBh;
            componentSpec.RamType = model.RamType;
            componentSpec.RamSl = model.RamSl;
            componentSpec.RamBh = model.RamBh;
            componentSpec.Storage = model.Storage;
            componentSpec.StorageSl = model.StorageSl;
            componentSpec.StorageBh = model.StorageBh;
            componentSpec.PowerSupply = model.PowerSupply;
            componentSpec.PowerSupplySl = model.PowerSupplySl;
            componentSpec.PowerSupplyBh = model.PowerSupplyBh;
            componentSpec.Vga = model.Vga;
            componentSpec.VgaSl = model.VgaSl;
            componentSpec.VgaBh = model.VgaBh;
            componentSpec.FormFactor = model.FormFactor;
            componentSpec.FormFactorSl = model.FormFactorSl;
            componentSpec.FormFactorBh = model.FormFactorBh;
            componentSpec.Cooler = model.Cooler;
            componentSpec.CoolerSl = model.CoolerSl;
            componentSpec.CoolerBh = model.CoolerBh;
            componentSpec.Wattage = model.Wattage;
        }

        private static ComponentSpec CreateComponentSpec(ProductCreateViewModel model)
        {
            return new ComponentSpec
            {
                Socket = model.Socket,
                SocketSl = model.SocketSl,
                SocketBh = model.SocketBh,
                Mainboard = model.Mainboard,
                MainboardSl = model.MainboardSl,
                MainboardBh = model.MainboardBh,
                RamType = model.RamType,
                RamSl = model.RamSl,
                RamBh = model.RamBh,
                Storage = model.Storage,
                StorageSl = model.StorageSl,
                StorageBh = model.StorageBh,
                PowerSupply = model.PowerSupply,
                PowerSupplySl = model.PowerSupplySl,
                PowerSupplyBh = model.PowerSupplyBh,
                Vga = model.Vga,
                VgaSl = model.VgaSl,
                VgaBh = model.VgaBh,
                FormFactor = model.FormFactor,
                FormFactorSl = model.FormFactorSl,
                FormFactorBh = model.FormFactorBh,
                Cooler = model.Cooler,
                CoolerSl = model.CoolerSl,
                CoolerBh = model.CoolerBh,
                Wattage = model.Wattage
            };
        }
    }
}
