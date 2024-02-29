namespace Stardust.Nmap.Class
{
    public class DetectedDevice
    {
        public string? LocalIP { get; set; }
        public string? MACAddress { get; set; }
        public string? Owner { get; set; }
        public string? Vendor { get; set; }
        public string? Hostname { get; set; }
        public Dictionary<int, string> OpenedPorts { get; set; } = new();
        public List<int> FilteredPorts { get; set; } = new();
        public Dictionary<int, string> OpenedFilteredPorts { get; set; } = new();
        public List<int> ClosedPorts { get; set; } = new();
        public bool? BehindFirewall { get; set; }
        public bool? IsOnline { get; set; }
    }
}
