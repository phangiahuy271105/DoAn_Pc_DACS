using DoAn_Pc_DACS.Data;
using DoAn_Pc_DACS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace DoAn_Pc_DACS.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class OrderController(ApplicationDbContext context) : Controller
{
    [HttpGet]
    public IActionResult Track(string? code = null) => View(new OrderTrackingViewModel
    {
        OrderCode = code ?? string.Empty
    });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("order-tracking")]
    public async Task<IActionResult> Track(OrderTrackingViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var phone = Regex.Replace(model.PhoneNumber.Trim(), @"[\s.()-]", "");
        if (phone.StartsWith("+84")) phone = "0" + phone[3..];
        if (!Regex.IsMatch(phone, @"^0[0-9]{9,10}$"))
        {
            ModelState.AddModelError(nameof(model.PhoneNumber), "Số điện thoại không đúng định dạng.");
            return View(model);
        }

        if (int.TryParse(model.OrderCode.TrimStart('#'), out var id) && id > 0)
        {
            var internationalPhone = "+84" + phone[1..];
            model.Result = await context.Orders.AsNoTracking()
                .Where(order => order.Id == id &&
                    (order.PhoneNumber == phone || order.PhoneNumber == internationalPhone))
                .Include(order => order.OrderDetails)
                .ThenInclude(detail => detail.Product)
                .SingleOrDefaultAsync();
        }

        if (model.Result == null)
            ModelState.AddModelError(string.Empty, "Không tìm thấy đơn hàng khớp với mã đơn và số điện thoại. Vui lòng kiểm tra lại cả hai thông tin.");

        return View(model);
    }
}
