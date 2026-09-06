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
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).ToList();
            return View(products);
        }

        public IActionResult CreateProduct()
        {
            var viewModel = new ProductCreateViewModel
            {
                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (model.ImageFile != null)
                {
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    OldPrice = model.OldPrice,
                    Discount = model.Discount,
                    CategoryId = model.CategoryId,
                    ImageUrl = "/images/uploads/" + uniqueFileName,
                    ComponentSpec = new ComponentSpec
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
                        CoolerBh = model.CoolerBh
                    },
                    ProductImages = new List<ProductImage>()
                };

                if (model.GalleryFiles != null && model.GalleryFiles.Count > 0)
                {
                    foreach (var file in model.GalleryFiles)
                    {
                        string galleryFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string galleryFilePath = Path.Combine(uploadsFolder, galleryFileName);
                        using (var fileStream = new FileStream(galleryFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        product.ProductImages.Add(new ProductImage { ImageUrl = "/images/uploads/" + galleryFileName });
                    }
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Admin");
            }

            model.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(model);
        }

        public IActionResult EditProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.ComponentSpec)
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id);

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
                CategoryId = product.CategoryId,
                ExistingImageUrl = product.ImageUrl,
                ExistingGalleryImages = product.ProductImages.ToList(),

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

                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products
                    .Include(p => p.ComponentSpec)
                    .Include(p => p.ProductImages)
                    .FirstOrDefault(p => p.Id == model.Id);

                if (product == null) return NotFound();

                if (model.DeleteOldGallery && product.ProductImages != null && product.ProductImages.Any())
                {
                    foreach (var oldImg in product.ProductImages)
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImg.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath); // Xóa file vật lý
                    }
                    _context.ProductImages.RemoveRange(product.ProductImages);
                    product.ProductImages.Clear();
                }

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "uploads");

                if (model.ImageFile != null)
                {
                    // =============== FIX LỖI 2: XÓA ẢNH ĐẠI DIỆN CŨ TRONG Ổ CỨNG ===============
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath); // Xóa file vật lý
                    }

                    // Lưu ảnh mới
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = "/images/uploads/" + uniqueFileName;
                }

                if (model.GalleryFiles != null && model.GalleryFiles.Count > 0)
                {
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    if (product.ProductImages == null) product.ProductImages = new List<ProductImage>();

                    foreach (var file in model.GalleryFiles)
                    {
                        string galleryFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string galleryFilePath = Path.Combine(uploadsFolder, galleryFileName);
                        using (var fileStream = new FileStream(galleryFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        product.ProductImages.Add(new ProductImage { ImageUrl = "/images/uploads/" + galleryFileName });
                    }
                }

                product.Name = model.Name;
                product.Price = model.Price;
                product.OldPrice = model.OldPrice;
                product.Discount = model.Discount;
                product.CategoryId = model.CategoryId;

                if (product.ComponentSpec == null) product.ComponentSpec = new ComponentSpec();

                product.ComponentSpec.Socket = model.Socket; product.ComponentSpec.SocketSl = model.SocketSl; product.ComponentSpec.SocketBh = model.SocketBh;
                product.ComponentSpec.Mainboard = model.Mainboard; product.ComponentSpec.MainboardSl = model.MainboardSl; product.ComponentSpec.MainboardBh = model.MainboardBh;
                product.ComponentSpec.RamType = model.RamType; product.ComponentSpec.RamSl = model.RamSl; product.ComponentSpec.RamBh = model.RamBh;
                product.ComponentSpec.Storage = model.Storage; product.ComponentSpec.StorageSl = model.StorageSl; product.ComponentSpec.StorageBh = model.StorageBh;
                product.ComponentSpec.PowerSupply = model.PowerSupply; product.ComponentSpec.PowerSupplySl = model.PowerSupplySl; product.ComponentSpec.PowerSupplyBh = model.PowerSupplyBh;
                product.ComponentSpec.Vga = model.Vga; product.ComponentSpec.VgaSl = model.VgaSl; product.ComponentSpec.VgaBh = model.VgaBh;
                product.ComponentSpec.FormFactor = model.FormFactor; product.ComponentSpec.FormFactorSl = model.FormFactorSl; product.ComponentSpec.FormFactorBh = model.FormFactorBh;
                product.ComponentSpec.Cooler = model.Cooler; product.ComponentSpec.CoolerSl = model.CoolerSl; product.ComponentSpec.CoolerBh = model.CoolerBh;

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Admin");
            }

            model.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ComponentSpec)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            string webRootPath = _webHostEnvironment.WebRootPath;

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagePath = Path.Combine(webRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
            }

            if (product.ProductImages != null && product.ProductImages.Count > 0)
            {
                foreach (var img in product.ProductImages)
                {
                    var galleryPath = Path.Combine(webRootPath, img.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(galleryPath)) System.IO.File.Delete(galleryPath);
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Admin");
        }
    }
}