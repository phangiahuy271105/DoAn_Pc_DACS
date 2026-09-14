using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class Product
    {
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
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ComponentSpec? ComponentSpec { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<ProductRelation> RelatedProducts { get; set; } = new List<ProductRelation>();
        public ICollection<ProductRelation> RecommendedByProducts { get; set; } = new List<ProductRelation>();
    }
}
