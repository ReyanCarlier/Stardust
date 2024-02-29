using Stardust.Nmap.Enum;

namespace Stardust.Nmap.Class
{
    public class ScannedPort
    {
        public string? Port { get; set; } = "";
        public PortStatus Status { get; set; } = PortStatus.Filtered;
        public string? DetectedUtility { get; set; } = "";
    }
}
