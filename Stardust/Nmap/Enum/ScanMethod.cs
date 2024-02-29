namespace Stardust.Nmap.Enums
{
    public enum ScanMethod
    {
        None,
        TCPStealth,
        TCPConnect,
        TCPAck,
        TCPWindow,
        UDP,
        TCPMaimon,
        TCPIdle,
        IPProtocol,
        TCPFTPBounce,
        NoPort,
        HostDiscovery,
        SimplePing,
        SYNStealth
    }
}
