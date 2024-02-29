using Stardust.Nmap.Enums;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Stardust.Nmap
{
    public class NmapProcess
    {
        public List<string> ResultLines { get; set; } = new List<string>();
        public List<string> ErrorLines { get; set; } = new List<string>();
        string Flags { get; set; } = "";

        private string ScanMethodHandler(ScanMethod scanMethod)
        {
            Flags = scanMethod switch
            {
                ScanMethod.TCPConnect => "-sS",
                ScanMethod.TCPStealth => "-sT",
                ScanMethod.SYNStealth => "-sN",
                ScanMethod.UDP => "-sU",
                ScanMethod.TCPAck => "-sA",
                ScanMethod.TCPWindow => "-sW",
                ScanMethod.TCPMaimon => "-sM",
                ScanMethod.TCPFTPBounce => "-F -Pn",
                ScanMethod.IPProtocol => "-sO",
                ScanMethod.TCPIdle => "-sI",
                ScanMethod.NoPort => "-sn",
                ScanMethod.None => "",
                ScanMethod.HostDiscovery => "-sL",
                ScanMethod.SimplePing => "-sn",
                _ => "-sT",
            };
            return Flags;
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outputLine)
        {
            try
            {
                if (outputLine.Data == null)
                    return;
                ResultLines.Add(outputLine.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void ErrorHandler(object sendingProcess, DataReceivedEventArgs errorLine)
        {
            try
            {
                if (errorLine.Data == null)
                    return;
                ErrorLines.Add(errorLine.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static IPAddress[] GetDnsAdresses(bool ip4Wanted, bool ip6Wanted)
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            HashSet<IPAddress> dnsAddresses = new();

            foreach (NetworkInterface networkInterface in interfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                    foreach (IPAddress forAddress in ipProperties.DnsAddresses)
                    {
                        if ((ip4Wanted && forAddress.AddressFamily == AddressFamily.InterNetwork) || (ip6Wanted && forAddress.AddressFamily == AddressFamily.InterNetworkV6))
                        {
                            _ = dnsAddresses.Add(forAddress);
                        }
                    }
                }
            }

            return dnsAddresses.ToArray();
        }

        public async Task ExecuteNmapProcess(string targetPorts, string targetIP, ScanMethod scanMethod)
        {
            ResultLines.Clear();
            ErrorLines.Clear();



            try
            {
                string? dnsservers = GetDnsAdresses(true, false)?[0].ToString();
                if (dnsservers == null)
                    dnsservers = "1.1.1.1";
                else
                {
                    string[] splittedTargetIP = targetIP.Split('.');
                    string[] splittedDNS = dnsservers.Split('.');
                    if (splittedTargetIP[0] == splittedDNS[0] && splittedTargetIP[1] == splittedDNS[1] && splittedTargetIP[2] == splittedDNS[2])
                        dnsservers = $"--dns-servers {GetDnsAdresses(true, false)?[0].ToString()}";
                    else
                        dnsservers = $"--dns-servers 1.1.1.1";
                }

                await Task.Run(() =>
                {
                    Process process = new();
                    process.StartInfo.FileName = "nmap";
                    process.StartInfo.Arguments = $"{ScanMethodHandler(scanMethod)} {targetPorts} {targetIP} {dnsservers} -oX - -T5";
                    Console.WriteLine($"{process.StartInfo.Arguments}");
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                    process.ErrorDataReceived += new DataReceivedEventHandler(ErrorHandler);
                    _ = process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"NmapProcess - ERROR : {e}");
            }
        }
    }
}
