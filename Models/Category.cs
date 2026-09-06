using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Slug { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}