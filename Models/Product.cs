using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public int Discount { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ComponentSpec ComponentSpec { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}