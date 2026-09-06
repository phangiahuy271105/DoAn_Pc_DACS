#nullable disable
using System.ComponentModel.DataAnnotations;

namespace DoAn_Pc_DACS.Models
{
    public class ComponentSpec
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Socket { get; set; }
        public int SocketSl { get; set; } = 1;
        public string SocketBh { get; set; } = "36 Tháng";

        public string RamType { get; set; }
        public int RamSl { get; set; } = 1;
        public string RamBh { get; set; } = "36 Tháng";

        public string FormFactor { get; set; }
        public int FormFactorSl { get; set; } = 1;
        public string FormFactorBh { get; set; } = "12 Tháng";

        public int Wattage { get; set; }

        public string Mainboard { get; set; }
        public int MainboardSl { get; set; } = 1;
        public string MainboardBh { get; set; } = "36 Tháng";

        public string Vga { get; set; }
        public int VgaSl { get; set; } = 1;
        public string VgaBh { get; set; } = "36 Tháng";

        public string Storage { get; set; }
        public int StorageSl { get; set; } = 1;
        public string StorageBh { get; set; } = "36 Tháng";

        public string PowerSupply { get; set; }
        public int PowerSupplySl { get; set; } = 1;
        public string PowerSupplyBh { get; set; } = "36 Tháng";

        public string Cooler { get; set; }
        public int CoolerSl { get; set; } = 1;
        public string CoolerBh { get; set; } = "12 Tháng";
    }
}