using Microsoft.AspNetCore.Mvc;
using DoAn_Pc_DACS.Data;
using DoAn_Pc_DACS.Models;
using DoAn_Pc_DACS.Helpers;
using System.Collections.Generic;
using System.Linq;

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
    }
}