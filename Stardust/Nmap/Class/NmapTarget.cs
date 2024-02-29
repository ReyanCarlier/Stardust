using Stardust.Nmap.Enums;

namespace Stardust.Nmap.Class
{
    public class NmapTarget
    {
        public string IP { get; set; } = "";
        public ScanMethod ScanMethod { get; set; }
        public int StartingPort { get; set; } = -1;
        public int? EndingPort { get; set; }
    }
}
