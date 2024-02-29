using Stardust.Nmap.Enums;
using System.Net;

namespace Stardust.Nmap
{
    public class PortScanService
    {
        List<string> ResultLines { get; set; } = new List<string>();
        List<string> ErrorLines { get; set; } = new List<string>();
        string TargetIP { get; set; } = "";
        string TargetPorts { get; set; } = "";

        public PortScanService()
        {
            this.ErrorLines = new List<string>();
            this.ResultLines = new List<string>();
        }

        public List<string> GetResultLines()
        {
            return this.ResultLines;
        }

        public List<string> GetErrorLines()
        {
            return this.ErrorLines;
        }

        public async Task RunNmapScanSinglePortAsync(ScanMethod _ScanMethod, IPAddress _IPAddress, int _Port)
        {
            ErrorLines.Clear();
            ResultLines.Clear();
            try
            {

                TargetPorts = _Port != 0 ? $"-p {_Port}" : "";
                TargetIP = _IPAddress.ToString();
                NmapProcess process = new();
                await process.ExecuteNmapProcess(TargetPorts, TargetIP, _ScanMethod);
                ErrorLines = process.ErrorLines;
                ResultLines = process.ResultLines;
            }
            catch (Exception e)
            {
                Console.WriteLine($"RunNmapSinglePort - ERROR : {e.Message}");
            }
        }

        public async Task RunNmapScanPortsRangeAsync(ScanMethod _ScanMethod, IPAddress _IPAddress, int StartPort, int EndPort)
        {
            ErrorLines = new();
            ResultLines = new();
            try
            {
                TargetPorts = $"-p{StartPort}-{EndPort}";
                TargetIP = _IPAddress.ToString();
                NmapProcess process = new();
                await process.ExecuteNmapProcess(TargetPorts, TargetIP, _ScanMethod);
                ErrorLines = process.ErrorLines;
                ResultLines = process.ResultLines;
            }
            catch (Exception e)
            {
                Console.WriteLine($"RunNmapPortsRange - ERROR : {e.Message}");
            }
        }
    }
}
