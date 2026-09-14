using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DoAn_Pc_DACS.Models;

public class OrderTrackingViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mã đơn hàng.")]
    [RegularExpression(@"^#?[0-9]{1,10}$", ErrorMessage = "Mã đơn hàng gồm các chữ số, ví dụ #123.")]
    public string OrderCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại đặt hàng.")]
    [StringLength(25, ErrorMessage = "Số điện thoại quá dài.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [BindNever]
    public Order? Result { get; set; }
}
