namespace DoAn_Pc_DACS.Models;

public class PcBuilderViewModel
{
    public static readonly Dictionary<string, string> Slots = new()
    {
        ["CPU"] = "CPU", ["Mainboard"] = "Mainboard", ["RAM"] = "RAM",
        ["GPU"] = "Card đồ họa", ["Storage"] = "Ổ cứng", ["PSU"] = "Nguồn",
        ["Case"] = "Vỏ case", ["Cooler"] = "Tản nhiệt"
    };
    public Dictionary<string, int> Selection { get; set; } = new();
    public List<Product> Products { get; set; } = [];
    public bool AcknowledgeLimitations { get; set; }
}
