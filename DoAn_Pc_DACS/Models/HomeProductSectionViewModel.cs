namespace DoAn_Pc_DACS.Models
{
    public class HomeProductSectionViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string SeeAllUrl { get; set; } = string.Empty;
        public IReadOnlyList<Product> Products { get; set; } = Array.Empty<Product>();
    }
}
