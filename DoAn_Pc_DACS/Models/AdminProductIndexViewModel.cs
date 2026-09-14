using System.Collections.Generic;

namespace DoAn_Pc_DACS.Models
{
    public class AdminProductIndexViewModel
    {
        public IReadOnlyList<Product> Products { get; set; } = new List<Product>();
        public IReadOnlyDictionary<string, int> GroupCounts { get; set; } = new Dictionary<string, int>();
        public string CurrentGroup { get; set; } = "all";
        public string SearchTerm { get; set; } = string.Empty;
    }
}
