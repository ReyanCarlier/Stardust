using Stardust.Nmap.Enums;
using System.Net;
using System.Net.Sockets;

namespace Stardust.Nmap
{
    public class DevicesService
    {
        public static string GetLocalIP()
        {
            string? localIP = null;
            using (Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint?.Address.ToString();
            }
            if (localIP == null)
            {
                throw new Exception("GetLocalIP: This error isn't supposed to happen if you access the app in remote.");
            }
            return localIP;
        }

        List<string> ResultLines { get; set; } = new List<string>();
        List<string> ErrorLines { get; set; } = new List<string>();
        public DevicesService()
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

        public async Task RunNmapScanLocalNetworkAsync()
        {
            ErrorLines.Clear();
            ResultLines.Clear();
            try
            {
                NmapProcess process = new();
                string localIP = GetLocalIP();
                string localNetwork = "";
                string[] splitted = localIP.Split(".");
                for (int i = 0; i < splitted.Length - 1; i++)
                {
                    localNetwork += splitted[i] + ".";
                }
                localNetwork += "0/24";
                await process.ExecuteNmapProcess("", localNetwork, ScanMethod.SimplePing);
                ErrorLines = process.ErrorLines;
                ResultLines = process.ResultLines;
            }
            catch (Exception e)
            {
                Console.WriteLine($"RunNmapSinglePort - ERROR : {e.Message}");
            }
        }
    }
}
