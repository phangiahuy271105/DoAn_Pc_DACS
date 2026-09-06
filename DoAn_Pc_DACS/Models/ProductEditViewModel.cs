#nullable disable
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        public decimal Price { get; set; }

        public decimal OldPrice { get; set; }

        public int Discount { get; set; }

        public IFormFile ImageFile { get; set; }

        public string ExistingImageUrl { get; set; }

        public List<IFormFile> GalleryFiles { get; set; }

        public List<ProductImage> ExistingGalleryImages { get; set; }

        public bool DeleteOldGallery { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

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