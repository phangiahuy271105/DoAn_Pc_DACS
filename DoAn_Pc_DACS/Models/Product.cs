using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class Product
    {
        [StringLength(20)]
        [RegularExpression("^(CPU|Mainboard|RAM|GPU|Storage|PSU|Case|Cooler)$", ErrorMessage = "Loại linh kiện không hợp lệ.")]
        public string? ComponentType { get; set; }

        [StringLength(20)]
        [RegularExpression("^(LGA1700|LGA1851|LGA1200|LGA1151|AM4|AM5)$", ErrorMessage = "Socket không hợp lệ.")]
        public string? BuildSocket { get; set; }

        [StringLength(20)]
        [RegularExpression("^(DDR3|DDR4|DDR5)$", ErrorMessage = "Chuẩn RAM không hợp lệ.")]
        public string? BuildMemoryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int DiscountPercent => CalculateDiscount(Price, OldPrice);

        public static int CalculateDiscount(decimal price, decimal oldPrice) =>
            oldPrice > price && price > 0
                ? (int)Math.Round((oldPrice - price) / oldPrice * 100, MidpointRounding.AwayFromZero)
                : 0;

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Range(1, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
        public decimal Price { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc không được âm")]
        public decimal OldPrice { get; set; }
        [Range(0, 100, ErrorMessage = "Giảm giá phải từ 0 đến 100%")]
        public int Discount { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không được âm")]
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [StringLength(2000)]
        public string? Description { get; set; }
        [StringLength(5000)]
        public string? TechnicalSpecifications { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ComponentSpec? ComponentSpec { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<ProductRelation> RelatedProducts { get; set; } = new List<ProductRelation>();
        public ICollection<ProductRelation> RecommendedByProducts { get; set; } = new List<ProductRelation>();
    }
}
