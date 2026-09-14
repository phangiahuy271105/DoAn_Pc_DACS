using DoAn_Pc_DACS.Helpers;
using DoAn_Pc_DACS.Models;

var parts = new Dictionary<string, Product>
{
    ["CPU"] = new() { BuildSocket = "AM5" },
    ["Mainboard"] = new() { BuildSocket = "AM5", BuildMemoryType = "DDR5" },
    ["RAM"] = new() { BuildMemoryType = "DDR5" }
};
Check("Matching socket and RAM", 0);
parts["CPU"].BuildSocket = "LGA1700";
Check("Socket mismatch", 1);
parts["RAM"].BuildMemoryType = "DDR4";
Check("Two independent conflicts", 2);
parts["CPU"].BuildSocket = null;
Check("Unknown socket does not hide RAM conflict", 1);
parts["RAM"].BuildMemoryType = null;
Check("Unknown metadata is not a known conflict", 0);
parts.Clear();
Check("Partial selection", 0);
Console.WriteLine("All 6 compatibility checks passed.");

void Check(string name, int expected)
{
    var actual = PcCompatibility.Conflicts(parts).Count;
    if (actual != expected) throw new Exception($"{name}: expected {expected}, got {actual}");
}
