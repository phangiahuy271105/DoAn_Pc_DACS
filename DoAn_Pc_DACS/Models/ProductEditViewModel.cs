#nullable disable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class ProductEditViewModel
    {
        [StringLength(20)]
        [RegularExpression("^(CPU|Mainboard|RAM|GPU|Storage|PSU|Case|Cooler)$", ErrorMessage = "Loại linh kiện không hợp lệ.")]
        public string ComponentType { get; set; }

        [StringLength(20)]
        [RegularExpression("^(LGA1700|LGA1851|LGA1200|LGA1151|AM4|AM5)$", ErrorMessage = "Socket không hợp lệ.")]
        public string BuildSocket { get; set; }

        [StringLength(20)]
        [RegularExpression("^(DDR3|DDR4|DDR5)$", ErrorMessage = "Chuẩn RAM không hợp lệ.")]
        public string BuildMemoryType { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(1, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc không được âm")]
        public decimal OldPrice { get; set; }

        [Range(0, 100, ErrorMessage = "Giảm giá phải từ 0 đến 100%")]
        public int Discount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho không được âm")]
        public int StockQuantity { get; set; }

        public IFormFile ImageFile { get; set; }

        public string ExistingImageUrl { get; set; }

        public List<IFormFile> GalleryFiles { get; set; }

        public List<ProductImage> ExistingGalleryImages { get; set; }

        public bool DeleteOldGallery { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        [StringLength(2000, ErrorMessage = "Mô tả tối đa 2000 ký tự")]
        public string Description { get; set; }

        [StringLength(5000, ErrorMessage = "Thông số kỹ thuật tối đa 5000 ký tự")]
        public string TechnicalSpecifications { get; set; }

        public List<int> RelatedProductIds { get; set; } = new();
        public IEnumerable<SelectListItem> AvailableRelatedProducts { get; set; } = new List<SelectListItem>();

        [StringLength(50)]
        public string Socket { get; set; }
        public int SocketSl { get; set; }
        public string SocketBh { get; set; }

        public string RamType { get; set; }
        public int RamSl { get; set; }
        public string RamBh { get; set; }

        public string FormFactor { get; set; }
        public int FormFactorSl { get; set; }
        public string FormFactorBh { get; set; }

        public int Wattage { get; set; }

        public string Mainboard { get; set; }
        public int MainboardSl { get; set; }
        public string MainboardBh { get; set; }

        public string Vga { get; set; }
        public int VgaSl { get; set; }
        public string VgaBh { get; set; }

        public string Storage { get; set; }
        public int StorageSl { get; set; }
        public string StorageBh { get; set; }

        public string PowerSupply { get; set; }
        public int PowerSupplySl { get; set; }
        public string PowerSupplyBh { get; set; }

        public string Cooler { get; set; }
        public int CoolerSl { get; set; }
        public string CoolerBh { get; set; }
    }
}
