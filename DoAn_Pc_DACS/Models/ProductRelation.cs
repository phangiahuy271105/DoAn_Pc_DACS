using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class ProductRelation
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int RelatedProductId { get; set; }
        public Product RelatedProduct { get; set; } = null!;

        [Range(0, 100)]
        public int DisplayOrder { get; set; }
    }
}
