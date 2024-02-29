using Stardust.Nmap.Enums;
using System.ComponentModel.DataAnnotations;

namespace Stardust.Nmap.Class
{
    public class NmapScanPortsForm
    {
        [Required]
        public string IP { get; set; } = "";

        [Required]
        public ScanMethod ScanMethod { get; set; }

        [Required]
        public int StartingPort { get; set; }

        public int EndingPort { get; set; }
    }
}
