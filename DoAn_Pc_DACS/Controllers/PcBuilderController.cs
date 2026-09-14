using DoAn_Pc_DACS.Data;
using DoAn_Pc_DACS.Helpers;
using DoAn_Pc_DACS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Pc_DACS.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class PcBuilderController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = new PcBuilderViewModel
        {
            Selection = HttpContext.Session.Get<Dictionary<string, int>>("PcBuildSelection") ?? new()
        };
        await Populate(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(PcBuilderViewModel model)
    {
        model.Selection ??= new();
        await Populate(model);
        if (model.Selection.Count > 8 || model.Selection.Keys.Any(key => !PcBuilderViewModel.Slots.ContainsKey(key)))
            ModelState.AddModelError("", "Danh sách linh kiện không hợp lệ.");

        var parts = new Dictionary<string, Product>();
        foreach (var entry in model.Selection.Where(entry => entry.Value != 0))
        {
            var product = model.Products.FirstOrDefault(product => product.Id == entry.Value && product.ComponentType == entry.Key);
            if (product == null)
                ModelState.AddModelError("", "Có linh kiện đã hết hàng, bị xóa hoặc thay đổi phân loại. Vui lòng chọn lại.");
            else parts[entry.Key] = product;
        }
        if (parts.Count == 0) ModelState.AddModelError("", "Vui lòng chọn ít nhất một linh kiện.");
        foreach (var conflict in PcCompatibility.Conflicts(parts)) ModelState.AddModelError("", conflict);
        if (!model.AcknowledgeLimitations)
            ModelState.AddModelError("", "Vui lòng xác nhận phạm vi kiểm tra trước khi thêm vào giỏ.");

        var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? [];
        foreach (var product in parts.Values)
        {
            var quantity = cart.Where(item => item.ProductId == product.Id).Sum(item => (long)item.Quantity);
            if (quantity + 1 > product.StockQuantity)
                ModelState.AddModelError("", $"Không đủ tồn kho cho {product.Name}, kể cả số lượng đang có trong giỏ.");
        }
        if (!ModelState.IsValid) return View("Index", model);

        foreach (var product in parts.Values)
        {
            var item = cart.FirstOrDefault(item => item.ProductId == product.Id);
            if (item == null)
            {
                item = new CartItem { ProductId = product.Id };
                cart.Add(item);
            }
            item.Quantity++;
            item.ProductName = product.Name;
            item.ImageUrl = product.ImageUrl;
            item.Price = product.Price;
            item.StockQuantity = product.StockQuantity;
        }
        HttpContext.Session.Set("Cart", cart);
        HttpContext.Session.Set("PcBuildSelection", model.Selection);
        return RedirectToAction("Index", "Cart");
    }

    private async Task Populate(PcBuilderViewModel model)
    {
        var types = PcBuilderViewModel.Slots.Keys.ToArray();
        model.Products = await context.Products.AsNoTracking()
            .Where(product => product.Category.Slug == "linh-kien-may-tinh" &&
                product.ComponentType != null && types.Contains(product.ComponentType) && product.StockQuantity > 0)
            .OrderBy(product => product.Price).ThenBy(product => product.Name).ToListAsync();
    }
}
