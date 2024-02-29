using Microsoft.Graph.Beta.Models;
using Stardust.Database.Data;
using Stardust.Nmap.Class;
using Stardust.Nmap;
using Stardust.Database.CRUD;
using System.Xml;
using Stardust.Nmap.Enums;
using System.Net;

namespace Stardust.Pages.Admin
{
    public class BackgroundGlobalScanService
    {
        private GraphWrapper.GraphWrapper? _graphClient;
        private StardustCRUD? _stardustCRUD;
        private CancellationTokenSource? _cts;
        private IPAddress? iPAddress;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PortScanService PortScanService = new();

        public BackgroundGlobalScanService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _cts = new CancellationTokenSource();
        }

        List<ManagedDevice?> ManagedDevices = new();
        public List<ManagedDevice?> ScannableDevices = new();
        public ManagedDevice? CurrentlyScannedDevice = new();
        DevicesService DevicesService = new();
        GlobalScan GlobalScan = new();
        List<DetectedDevice> DetectedDevices = new();
        
        public bool _generateTasks = false;
        List<string> ResultLines { get; set; } = new();
        List<string> ErrorLines { get; set; } = new();
        XmlDocument? ScanReport { get; set; } = null;
        DetectedDevice DetectedDevice { get; set; } = new();
        string userId { get; set; } = "";
        public int _counter = 0;

        public async void StartBackgroundTask(string? userId, bool _generateTasks = false)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("userId cannot be null or empty", nameof(userId));
            this._generateTasks = _generateTasks;
            this.userId = userId;

            if (Program.GlobalScanPending)
                return;

            _cts = new CancellationTokenSource();
            await ExecuteBackgroundTask(_cts.Token);
        }

        public int GetBackgroundTaskCounter()
        {
            return ScannableDevices.Count;
        }

        public async void StopBackgroundTask()
        {
            GlobalScan.Ended_At = DateTime.Now;
            await _stardustCRUD.UpdateGlobalScanAsync(GlobalScan);
            Console.WriteLine("Stopping Global Scan...");
            _cts?.Cancel();
            Program.GlobalScanPending = false;
            Console.WriteLine("Global Scan stopped!");
        }

        private void ParseScan(XmlDocument? ScanReport)
        {
            if (ScanReport == null)
                return;
            if (DetectedDevices != null)
                DetectedDevices.Clear();
            else
                DetectedDevices = new();
            XmlNodeList HostsNodeList = ScanReport.GetElementsByTagName("host");

            foreach (XmlNode hostNode in HostsNodeList)
            {
                var ipNode = hostNode.SelectSingleNode("address");
                var macNode = ipNode?.NextSibling;
                var hostnameNode = hostNode.SelectSingleNode("hostnames/hostname")?.Attributes?["name"];

                var detectedDevice = new DetectedDevice
                {
                    LocalIP = ipNode?.Attributes?["addr"]?.Value,
                    MACAddress = macNode?.Attributes?["addr"]?.Value,
                    Vendor = macNode?.Attributes?["vendor"]?.Value,
                    Hostname = hostnameNode?.Value ?? "N/A"
                };
                if (detectedDevice.MACAddress != null)
                    detectedDevice.MACAddress = detectedDevice.MACAddress.ToUpper();
                else
                    continue;
                if (detectedDevice.Hostname != "host.docker.internal")
                    DetectedDevices.Add(detectedDevice);
            }
        }

        private async Task GetAllDevices()
        {
            ManagedDevices = await _graphClient.GetAllManagedDevices();

            await DevicesService.RunNmapScanLocalNetworkAsync();
            List<string> text = DevicesService.GetResultLines();
            XmlDocument? ScanReport = new();
            ScanReport.LoadXml(string.Join(string.Empty, text));
            ParseScan(ScanReport);

            foreach (ManagedDevice? managedDevice in ManagedDevices)
            {
                if (managedDevice == null)
                    continue;
                foreach (DetectedDevice? detectedDevice in DetectedDevices)
                {
                    if (detectedDevice == null)
                        continue;
                    if (detectedDevice.MACAddress == managedDevice.WiFiMacAddress || detectedDevice.MACAddress == managedDevice.EthernetMacAddress)
                    {
                        managedDevice.AdditionalData["OnLocalNetwork"] = true;
                        managedDevice.AdditionalData["LocalIP"] = detectedDevice.LocalIP;
                        break;
                    }
                }
            }
            ScannableDevices.Clear();
            foreach (ManagedDevice? managedDevice in ManagedDevices)
            {
                if (managedDevice == null)
                    continue;
                if (managedDevice.AdditionalData.ContainsKey("OnLocalNetwork") && (bool)(managedDevice.AdditionalData["OnLocalNetwork"]) == true)
                    ScannableDevices.Add(managedDevice);
            }
        }

        private void ParseFiltered()
        {
            XmlNodeList? portNodes = ScanReport?.SelectNodes("//port");
            if (portNodes != null)
            {
                foreach (XmlNode portNode in portNodes)
                {
                    string? portState = portNode?.SelectSingleNode("state/@state")?.InnerText;
                    if (portState == "open|filtered")
                    {
                        string? portId = portNode?.Attributes?["portid"]?.Value;
                        string? serviceName = portNode?.SelectSingleNode("service/@name")?.InnerText;
                        if (serviceName == null)
                            serviceName = "N/A";
                        DetectedDevice.OpenedFilteredPorts.Add(Convert.ToInt32(portId), serviceName);
                    }
                    else if (portState == "open")
                    {
                        string? portId = portNode?.Attributes?["portid"]?.Value;
                        string? serviceName = portNode?.SelectSingleNode("service/@name")?.InnerText;
                        if (serviceName == null)
                            serviceName = "N/A";
                        DetectedDevice.OpenedPorts.Add(Convert.ToInt32(portId), serviceName);
                        if (DetectedDevice.OpenedFilteredPorts.ContainsKey(Convert.ToInt32(portId)))
                            DetectedDevice.OpenedFilteredPorts.Remove(Convert.ToInt32(portId));
                    }
                    else if (portState == "filtered")
                    {
                        string? portId = portNode?.Attributes?["portid"]?.Value;
                        DetectedDevice.FilteredPorts.Add(Convert.ToInt32(portId));
                    }
                }
            }
        }

        private void ParseClosed()
        {
            XmlNodeList? stateNodes = ScanReport?.SelectNodes("//extraports");
            if (stateNodes == null)
                return;

            foreach (XmlNode stateNode in stateNodes)
            {
                string? state = stateNode.Attributes?["state"]?.Value;

                if (string.IsNullOrEmpty(state))
                    continue;

                XmlNode? extraReasonsNode = stateNode.SelectSingleNode("extrareasons");
                string? closedPortRanges = extraReasonsNode?.Attributes?["ports"]?.Value;

                if (string.IsNullOrEmpty(closedPortRanges))
                    continue;

                string[]? ranges = closedPortRanges.Split(',');
                foreach (var range in ranges)
                {
                    if (range.Contains("-"))
                    {
                        string[] tokens = range.Split('-');
                        int start = int.Parse(tokens[0]);
                        int end = int.Parse(tokens[1]);
                        for (int i = start; i <= end; i++)
                        {
                            AddPortToDetectedDevice(state, i);
                        }
                    }
                    else
                    {
                        AddPortToDetectedDevice(state, int.Parse(range));
                    }
                }
            }
        }

        private void AddPortToDetectedDevice(string? state, int port)
        {
            if (state == "closed")
            {
                DetectedDevice.ClosedPorts.Add(port);
            }
            else if (state == "filtered")
            {
                DetectedDevice.FilteredPorts.Add(port);
            }
        }

        private async Task ParseOpenedFromFiltered()
        {
            if (iPAddress == null)
                return;
            foreach (var port in DetectedDevice.OpenedFilteredPorts)
            {
                await PortScanService.RunNmapScanSinglePortAsync(ScanMethod.TCPConnect, iPAddress, port.Key);
                ResultLines.Clear();
                ErrorLines.Clear();
                ResultLines = PortScanService.GetResultLines();
                ErrorLines = PortScanService.GetErrorLines();

                string rawXML = string.Join(string.Empty, ResultLines);
                if (rawXML == "")
                    return;
                ScanReport ??= new();
                ScanReport.LoadXml(rawXML);
                ParseFiltered();
            }
        }

        private async Task<int> ScanDevice(string ipstring, string? hostname)
        {
            try
            {
                iPAddress = IPAddress.Parse(ipstring);

                await PortScanService.RunNmapScanSinglePortAsync(ScanMethod.NoPort, iPAddress, 0);

                ResultLines.Clear();
                ErrorLines.Clear();
                ResultLines = PortScanService.GetResultLines();
                ErrorLines = PortScanService.GetErrorLines();
                string rawXML = string.Join(string.Empty, ResultLines);
                if (rawXML == "")
                    return -1;
                if (ScanReport == null)
                    ScanReport = new();
                ScanReport.LoadXml(rawXML);

                XmlNode? hostnameNode = ScanReport?.SelectSingleNode("//host/hostnames/hostname")?.Attributes?["name"];
                XmlNode? macNode = ScanReport?.SelectSingleNode("//host/address")?.NextSibling?.Attributes?["addr"];
                XmlNode? vendorNode = ScanReport?.SelectSingleNode("//host/address")?.NextSibling?.Attributes?["vendor"];
                DetectedDevice.Hostname = hostnameNode?.Value != null ? hostname : "N/A";
                DetectedDevice.MACAddress = macNode?.Value != null ? macNode.Value : "N/A";
                DetectedDevice.Vendor = vendorNode?.Value != null ? vendorNode.Value : "N/A";
                if (DetectedDevice?.MACAddress == null || DetectedDevice?.MACAddress == "N/A")
                    return -1;
                await PortScanService.RunNmapScanSinglePortAsync(ScanMethod.None, iPAddress, 0);
                ResultLines.Clear();
                ErrorLines.Clear();
                ResultLines = PortScanService.GetResultLines();
                ErrorLines = PortScanService.GetErrorLines();
                rawXML = string.Join(string.Empty, ResultLines);
                if (rawXML == "")
                    return -1;
                ScanReport ??= new();
                ScanReport.LoadXml(rawXML);
                DetectedDevice.ClosedPorts.Clear();
                DetectedDevice.OpenedPorts.Clear();
                DetectedDevice.FilteredPorts.Clear();
                ParseFiltered();
                ParseClosed();

                await ParseOpenedFromFiltered();

                List<Machine>? machines = await _stardustCRUD.GetMachinesAsync();

                Machine? machine = await _stardustCRUD.GetMachineByHostnameAsync(hostname);
                List<Database.Data.Report> reports = await _stardustCRUD.GetReportsAsync();
                DateTime datetime = DateTime.Now;
                if (machine == null)
                {
                    machine = new Machine
                    {
                        Id = machines.Count + 1,
                        Hostname = hostname,
                        MacAddress = DetectedDevice?.MACAddress,
                        Vendor = DetectedDevice?.Vendor,
                        LastIp = ipstring,
                        LastScan = datetime
                    };
                    await _stardustCRUD.CreateMachineAsync(machine);
                }
                else
                {
                    machine.Hostname = hostname;
                    machine.MacAddress = DetectedDevice?.MACAddress;
                    machine.LastIp = ipstring;
                    machine.Vendor = DetectedDevice?.Vendor;
                    machine.LastScan = datetime;
                    await _stardustCRUD.UpdateMachineAsync(machine);
                }
                Database.Data.Report report = new(reports.Count + 1, type: 1, content: "", closedports: string.Join(",", DetectedDevice.ClosedPorts), filteredports: string.Join(",", DetectedDevice.FilteredPorts), openedports: string.Join(",", DetectedDevice.OpenedPorts.Keys), machineid: (int)machine.Id, createdat: datetime, userId: userId);

                await _stardustCRUD.CreateReportAsync(report);
                return reports.Count + 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (-1);
        }

        private async Task ExecuteBackgroundTask(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    _graphClient = scope.ServiceProvider.GetRequiredService<GraphWrapper.GraphWrapper>();
                    _stardustCRUD = scope.ServiceProvider.GetRequiredService<StardustCRUD>();
                    List<GlobalScan> globalScans = await _stardustCRUD.GetAllGlobalScansAsync();
                    GlobalScan = new()
                    {
                        Initiated_By = userId,
                        Id = globalScans.Count + 1
                    };
                    await _stardustCRUD.AddGlobalScanAsync(GlobalScan);
                    Program.GlobalScanPending = true;

                    await GetAllDevices();

                    if (ScannableDevices.Count == 0)
                        continue;

                    _counter = 0;

                    Random random = new();
                    ScannableDevices = ScannableDevices.OrderBy(x => random.Next()).ToList();

                    foreach (var managedDevice in ScannableDevices)
                    {
                        if (managedDevice == null || !managedDevice.AdditionalData.ContainsKey("LocalIP"))
                            continue;
                        CurrentlyScannedDevice = managedDevice;
                        _counter++;

                        var localIp = managedDevice.AdditionalData["LocalIP"] as string;

                        if (string.IsNullOrEmpty(localIp))
                            continue;

                        int reportId = await ScanDevice(localIp, managedDevice?.DeviceName);
                        if (_generateTasks && DetectedDevice.OpenedPorts.Count > 0)
                        {
                            Machine? machine = await _stardustCRUD.GetMachineByHostnameAsync(managedDevice?.DeviceName);
                            if (machine == null)
                                continue;
                            List<TaskTodo>? taskTodos = await _stardustCRUD.GetAllTasksTodoAsync();
                            TaskTodo task = new()
                            {
                                Id = taskTodos != null ? taskTodos.Count + 1 : 1,
                                Title = $"Global Scan - Opened Ports",
                                Description = "",
                                UserId = managedDevice?.UserId,
                                Status = TodoTaskStatus.NotStarted,
                                Emergency = TodoTaskEmergency.Medium,
                                CreatedDate = DateTime.Now,
                                LastUpdatedDate = DateTime.Now,
                                DeviceId = machine.Id,
                                ReportId = reportId,
                                DueDate = DateTime.Now.AddDays(7),
                            };
                            List<int> openPorts = new();
                            foreach (var port in DetectedDevice.OpenedPorts)
                            {
                                openPorts.Add(port.Key);
                                task.Description += $"- PORT [{port.Key}] OPEN | SERVICE [{port.Value}]\n";
                            }
                            bool duplicate = false;
                            if (taskTodos != null)
                            {
                                foreach (var existingTasks in taskTodos)
                                {
                                    if (existingTasks.DeviceId == machine.Id && existingTasks.Description == task.Description)
                                    {
                                        duplicate = true;
                                        break;
                                    }
                                }
                            }
                            if (duplicate)
                                continue;
                            try
                            {
                                await _stardustCRUD.AddTaskTodoAsync(task);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Couldn't create task for {managedDevice?.DeviceName} {ex.Message}");
                            }
                        }
                        if (token.IsCancellationRequested)
                            break;
                    }

                    _counter = 0;
                    GlobalScan.Ended_At = DateTime.Now;
                    await _stardustCRUD.UpdateGlobalScanAsync(GlobalScan);
                    Program.GlobalScanPending = false;

                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
