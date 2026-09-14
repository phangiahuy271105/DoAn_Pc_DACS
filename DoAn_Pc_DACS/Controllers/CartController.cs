using Microsoft.AspNetCore.Mvc;
using DoAn_Pc_DACS.Data;
using DoAn_Pc_DACS.Models;
using DoAn_Pc_DACS.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Pc_DACS.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị Giỏ hàng
        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            await RefreshCartAsync(cart);

            return View(cart);
        }

        // Xử lý nút Thêm vào giỏ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            // Sản phẩm hết hàng
            if (product.StockQuantity <= 0)
            {
                TempData["CartError"] = "Sản phẩm này hiện đã hết hàng.";
                return RedirectToAction("Index");
            }

            // Không cho quantity nhỏ hơn 1
            if (quantity < 1)
                quantity = 1;

            var cart = HttpContext.Session.Get<List<CartItem>>("Cart")
                       ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            // Số lượng hiện đã nằm trong giỏ
            int currentQuantity = cartItem?.Quantity ?? 0;

            // Số lượng sau khi cộng thêm
            int newQuantity = currentQuantity + quantity;

            if (newQuantity > product.StockQuantity)
            {
                TempData["CartError"] =
                    $"Sản phẩm \"{product.Name}\" chỉ còn {product.StockQuantity} sản phẩm trong kho.";

                return RedirectToAction("Index");
            }

            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
                cartItem.StockQuantity = product.StockQuantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = quantity,
                    StockQuantity = product.StockQuantity
                });
            }

            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBundleToCart(int id, int[]? relationIds, int[]? relatedProductIds)
        {
            var selectedRelationIds = relationIds ?? [];
            var selectedRelatedIds = (relatedProductIds ?? [])
                .ToList();

            if (selectedRelationIds.Length != selectedRelatedIds.Count ||
                selectedRelationIds.Length > 5 ||
                selectedRelationIds.Distinct().Count() != selectedRelationIds.Length ||
                selectedRelatedIds.Distinct().Count() != selectedRelatedIds.Count)
            {
                TempData["CartError"] = "Danh sách sản phẩm mua kèm không hợp lệ.";
                return RedirectToAction("Details", "Home", new { id });
            }

            var configuredRelations = await _context.ProductRelations
                .AsNoTracking()
                .Where(relation => relation.ProductId == id && selectedRelationIds.Contains(relation.Id))
                .Select(relation => new
                {
                    relation.Id,
                    AllowedCategoryId = relation.RelatedProduct.CategoryId
                })
                .ToListAsync();

            if (configuredRelations.Count != selectedRelationIds.Length)
            {
                TempData["CartError"] = "Danh sách sản phẩm mua kèm không hợp lệ.";
                return RedirectToAction("Details", "Home", new { id });
            }

            var productIds = selectedRelatedIds.Append(id).Distinct().ToList();
            var products = await _context.Products
                .AsNoTracking()
                .Where(product => productIds.Contains(product.Id))
                .ToDictionaryAsync(product => product.Id);

            if (!products.ContainsKey(id))
            {
                return NotFound();
            }

            var relationCategories = configuredRelations.ToDictionary(relation => relation.Id, relation => relation.AllowedCategoryId);
            for (int index = 0; index < selectedRelationIds.Length; index++)
            {
                int selectedProductId = selectedRelatedIds[index];
                if (!products.TryGetValue(selectedProductId, out var selectedProduct) ||
                    selectedProduct.CategoryId != relationCategories[selectedRelationIds[index]] ||
                    selectedProductId == id)
                {
                    TempData["CartError"] = "Sản phẩm thay thế không thuộc đúng danh mục mua kèm.";
                    return RedirectToAction("Details", "Home", new { id });
                }
            }

            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            foreach (int productId in productIds)
            {
                if (!products.TryGetValue(productId, out var product))
                {
                    TempData["CartError"] = "Có sản phẩm mua kèm không còn được bán.";
                    return RedirectToAction("Details", "Home", new { id });
                }

                int currentQuantity = cart.FirstOrDefault(item => item.ProductId == productId)?.Quantity ?? 0;
                if (product.StockQuantity <= currentQuantity)
                {
                    TempData["CartError"] = $"Sản phẩm \"{product.Name}\" không đủ tồn kho để thêm combo.";
                    return RedirectToAction("Details", "Home", new { id });
                }
            }

            foreach (int productId in productIds)
            {
                var product = products[productId];
                var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);

                if (cartItem == null)
                {
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = 1,
                        StockQuantity = product.StockQuantity
                    });
                }
                else
                {
                    cartItem.Quantity++;
                    cartItem.ProductName = product.Name;
                    cartItem.ImageUrl = product.ImageUrl;
                    cartItem.Price = product.Price;
                    cartItem.StockQuantity = product.StockQuantity;
                }
            }

            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng (+ / -) trực tiếp trong giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");

            if (cart == null)
                return RedirectToAction("Index");

            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item == null)
                return RedirectToAction("Index");

            var product = _context.Products.Find(id);

            if (product == null)
                return RedirectToAction("Index");

            if (product.StockQuantity <= 0)
            {
                item.StockQuantity = 0;

                TempData["CartError"] =
                    $"Sản phẩm \"{product.Name}\" hiện đã hết hàng.";

                HttpContext.Session.Set("Cart", cart);

                return RedirectToAction("Index");
            }

            if (quantity < 1)
                quantity = 1;

            // Nếu nhập vượt tồn kho thì tự đưa về số lượng tối đa
            if (quantity > product.StockQuantity)
            {
                quantity = product.StockQuantity;

                TempData["CartError"] =
                    $"Sản phẩm \"{product.Name}\" chỉ còn {product.StockQuantity} sản phẩm.";
            }

            item.Quantity = quantity;
            item.StockQuantity = product.StockQuantity;

            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction("Index");
        }

        // Xóa 1 sản phẩm khỏi giỏ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == id);
                if (item != null)
                {
                    cart.Remove(item);
                    HttpContext.Session.Set("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }
       

        // 1. Giao diện trang Thanh toán (GET)
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart"); // Giỏ hàng trống thì đuổi về trang giỏ hàng
            }

            await RefreshCartAsync(cart);
            if (cart.Any(item => item.StockQuantity <= 0 || item.Quantity > item.StockQuantity))
            {
                TempData["CartError"] = "Tồn kho đã thay đổi. Vui lòng kiểm tra lại giỏ hàng.";
                return RedirectToAction("Index", "Cart");
            }

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(item => item.Price * item.Quantity);

            return View(new Order());
        }

        // 2. Xử lý lưu Đơn hàng vào CSDL (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            if (ModelState.IsValid)
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
                try
                {
                    var productIds = cart.Select(item => item.ProductId).Distinct().ToList();
                    var products = await _context.Products
                        .Where(product => productIds.Contains(product.Id))
                        .ToDictionaryAsync(product => product.Id);

                    foreach (var item in cart)
                    {
                        if (!products.TryGetValue(item.ProductId, out var product))
                        {
                            ModelState.AddModelError(string.Empty, $"Sản phẩm \"{item.ProductName}\" không còn được bán.");
                            continue;
                        }

                        item.ProductName = product.Name;
                        item.ImageUrl = product.ImageUrl;
                        item.Price = product.Price;
                        item.StockQuantity = product.StockQuantity;

                        if (item.Quantity < 1 || item.Quantity > product.StockQuantity)
                        {
                            ModelState.AddModelError(string.Empty,
                                $"Sản phẩm \"{product.Name}\" chỉ còn {product.StockQuantity} sản phẩm trong kho.");
                        }
                    }

                    HttpContext.Session.Set("Cart", cart);

                    if (!ModelState.IsValid)
                    {
                        await transaction.RollbackAsync();
                        PrepareCheckoutView(cart);
                        return View(order);
                    }

                    order.OrderDate = DateTime.Now;
                    order.Status = "Chờ xác nhận";
                    order.Note = order.Note?.Trim() ?? string.Empty;
                    order.TotalAmount = cart.Sum(item => item.Price * item.Quantity);

                    foreach (var item in cart)
                    {
                        var product = products[item.ProductId];
                        product.StockQuantity -= item.Quantity;

                        order.OrderDetails.Add(new OrderDetail
                        {
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            Price = product.Price
                        });
                    }

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    HttpContext.Session.Remove("Cart");
                    HttpContext.Session.SetInt32("LastOrderId", order.Id);

                    return RedirectToAction("CheckoutSuccess", new { id = order.Id });
                }
                catch (DbUpdateException)
                {
                    await transaction.RollbackAsync();
                    _context.ChangeTracker.Clear();
                    ModelState.AddModelError(string.Empty,
                        "Không thể tạo đơn hàng do dữ liệu vừa thay đổi. Vui lòng kiểm tra lại giỏ hàng.");
                }
            }

            await RefreshCartAsync(cart);
            PrepareCheckoutView(cart);
            return View(order);
        }

        // 3. Trang thông báo Đặt hàng thành công
        public IActionResult CheckoutSuccess(int id)
        {
            if (HttpContext.Session.GetInt32("LastOrderId") != id)
            {
                return RedirectToAction("Index", "Home");
            }

            // Truy vấn lấy đơn hàng kèm theo chi tiết sản phẩm
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        private async Task RefreshCartAsync(List<CartItem> cart)
        {
            var productIds = cart.Select(item => item.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(product => productIds.Contains(product.Id))
                .ToDictionaryAsync(product => product.Id);

            foreach (var item in cart)
            {
                if (products.TryGetValue(item.ProductId, out var product))
                {
                    item.ProductName = product.Name;
                    item.ImageUrl = product.ImageUrl;
                    item.Price = product.Price;
                    item.StockQuantity = product.StockQuantity;
                }
                else
                {
                    item.StockQuantity = 0;
                }
            }

            HttpContext.Session.Set("Cart", cart);
        }

        private void PrepareCheckoutView(List<CartItem> cart)
        {
            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(item => item.Price * item.Quantity);
        }
    }
}
