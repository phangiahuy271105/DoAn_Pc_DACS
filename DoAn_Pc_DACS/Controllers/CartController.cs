using Microsoft.AspNetCore.Mvc;
using DoAn_Pc_DACS.Data;
using DoAn_Pc_DACS.Models;
using DoAn_Pc_DACS.Helpers;
using System.Collections.Generic;
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
        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        // Xử lý nút Thêm vào giỏ
        [HttpPost]
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity; // Nếu đã có trong giỏ thì cộng dồn số lượng
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = quantity
                });
            }

            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng (+ / -) trực tiếp trong giỏ hàng
        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == id);
                if (item != null)
                {
                    item.Quantity = quantity > 0 ? quantity : 1;
                    HttpContext.Session.Set("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }

        // Xóa 1 sản phẩm khỏi giỏ
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
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart"); // Giỏ hàng trống thì đuổi về trang giỏ hàng
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

            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // MẤU CHỐT LÀ 3 DÒNG NÀY: Bỏ qua kiểm tra các trường không nhập từ form
            ModelState.Remove("OrderDetails");
            ModelState.Remove("Status");
            ModelState.Remove("Note");

            if (ModelState.IsValid)
            {
                // Bước 1: Lưu thông tin Order (Đơn hàng)
                order.OrderDate = DateTime.Now;
                order.TotalAmount = cart.Sum(item => item.Price * item.Quantity);
                order.Status = "Chờ xác nhận";

                
                if (order.Note == null)
                {
                    order.Note = ""; // Gán bằng chuỗi rỗng nếu khách không nhập gì
                }
               
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Lưu để sinh ID đơn hàng

                // Bước 2: Lưu chi tiết vào OrderDetails
                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId, // Lấy đúng ProductId theo CartItem
                        Quantity = item.Quantity,
                        Price = item.Price          // Lấy đúng Price
                    };
                    _context.OrderDetails.Add(orderDetail);
                }
                await _context.SaveChangesAsync();

                // Bước 3: Xóa giỏ hàng trong Session
                HttpContext.Session.Remove("Cart");

                return RedirectToAction("CheckoutSuccess", new { id = order.Id });
            }

            // Nếu người dùng nhập thiếu Tên, SĐT, Địa chỉ sẽ bị văng về lại form
            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(item => item.Price * item.Quantity);
            return View(order);
        }

        // 3. Trang thông báo Đặt hàng thành công
        public IActionResult CheckoutSuccess(int id)
        {
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
    }
}