using DoAn_Pc_DACS.Models;

namespace DoAn_Pc_DACS.Helpers;

public static class PcCompatibility
{
    public static List<string> Conflicts(IReadOnlyDictionary<string, Product> parts)
    {
        var result = new List<string>();
        Compare("CPU", "Mainboard", product => product.BuildSocket, "Socket CPU và Mainboard không khớp.");
        Compare("RAM", "Mainboard", product => product.BuildMemoryType, "Chuẩn RAM không khớp với Mainboard.");
        return result;

        void Compare(string first, string second, Func<Product, string?> value, string message)
        {
            if (parts.TryGetValue(first, out var a) && parts.TryGetValue(second, out var b) &&
                !string.IsNullOrWhiteSpace(value(a)) && !string.IsNullOrWhiteSpace(value(b)) &&
                !string.Equals(value(a), value(b), StringComparison.OrdinalIgnoreCase))
                result.Add(message);
        }
    }
}
