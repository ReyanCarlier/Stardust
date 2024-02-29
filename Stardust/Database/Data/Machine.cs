namespace Stardust.Database.Data
{
    public class Machine
    {
        public Machine() { }

        public int? Id { get; set; }
        public string? Hostname { get; set; }
        public string? MacAddress { get; set; }
        public string? LastIp { get; set; }
        public string? Vendor { get; set; }
        public bool? Protected { get; set; }
        public DateTime? LastScan { get; set; }

        public Machine(int? id, string? hostname, string? macAddress, string? lastIp, string? vendor, bool? @protected, DateTime? lastScan)
        {
            Id = id;
            Hostname = hostname;
            MacAddress = macAddress;
            LastIp = lastIp;
            Vendor = vendor;
            Protected = @protected;
            LastScan = lastScan;
        }
    }
}
