using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string Address { get; set; }

        public string Note { get; set; } // Ghi chú thêm của khách (nếu có)

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        // Trạng thái đơn: "Chờ xác nhận", "Đang giao", "Hoàn tất", "Đã hủy"
        public string Status { get; set; } = "Chờ xác nhận";

        // Liên kết 1-N với Chi tiết đơn hàng
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}