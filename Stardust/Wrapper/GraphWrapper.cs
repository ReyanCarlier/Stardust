using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.Models;
using Newtonsoft.Json;
using Stardust.Nmap.Class;

namespace Stardust.GraphWrapper
{
    public class GraphWrapper
    {
        private readonly GraphServiceClient GraphClient;

        public GraphWrapper(GraphServiceClient graphClient)
        {
            GraphClient = graphClient;
        }
         
        private static string? ParsedMacAddress(string? macaddress)
        {
            string? parsedMacAddress = null;
            if (macaddress == null)
                return parsedMacAddress;
            if (macaddress.Length == 12)
                parsedMacAddress = macaddress[..2] + ":" + macaddress.Substring(2, 2) + ":" + macaddress.Substring(4, 2) + ":" + macaddress.Substring(6, 2) + ":" + macaddress.Substring(8, 2) + ":" + macaddress.Substring(10, 2);
            else if (macaddress.Length == 17)
                parsedMacAddress = macaddress;
            return parsedMacAddress;
        }
        public async Task<List<ManagedDevice?>> GetAllManagedDevices()
        {
            var devices = new List<ManagedDevice?>();
            try
            {
                var mdcr = await GraphClient.DeviceManagement.ManagedDevices.GetAsync();
                string mdcrJson = JsonConvert.SerializeObject(mdcr);
                var mdcrDynamic = JsonConvert.DeserializeObject<dynamic>(mdcrJson);
                if (mdcrDynamic == null)
                    return devices;
                if (mdcrDynamic.Value == null)
                    return devices;
                foreach (var device in mdcrDynamic.Value)
                {
                    ManagedDevice? md = new()
                    {
                        DeviceName = device.DeviceName,
                        UserId = device.UserId,
                        UserPrincipalName = device.UserPrincipalName,
                        WiFiMacAddress = ParsedMacAddress((string?)device.WiFiMacAddress),
                        EthernetMacAddress = ParsedMacAddress((string?)device.EthernetMacAddress),
                        TotalStorageSpaceInBytes = device.TotalStorageSpaceInBytes,
                        FreeStorageSpaceInBytes = device.FreeStorageSpaceInBytes,
                        AdditionalData = new Dictionary<string, object>
                        {
                            { "OnLocalNetwork", false },
                            { "LocalIP", "" }
                        }
                    };
                    devices.Add(md);
                }
                return devices;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return devices;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var allUsers = new List<User>();

            var result = await GraphClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Top = 999;
                requestConfiguration.QueryParameters.Select = new string[] { "id", "companyName", "displayName", "mail", "deviceKeys", "jobTitle" };
            });

            if (result == null || result.Value == null)
                return allUsers;

            foreach (var user in result.Value)
            {
                allUsers.Add(user);
            }

            return allUsers;
        }

        public async Task<User?> GetGraphUserByDeviceMacAddress(string? macaddress, DetectedDevice? detectedDevice = null)
        {
            if (macaddress == null || macaddress == "N/A")
                return null;
            try
            {
                var machine = await GetAllManagedDevices();
                foreach (var device in machine)
                {
                    if (device == null)
                        continue;
                    if (device.WiFiMacAddress == macaddress)
                    {
                        User user = new()
                        {
                            DisplayName = device.UserPrincipalName,
                            Id = device.UserId
                        };
                        if (detectedDevice != null)
                            detectedDevice.Hostname = device.DeviceName;
                        return user;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<ManagedDevice?>> GetGraphManagedDevicesByUserId(string? userId)
        {
            var devices = new List<ManagedDevice?>();
            if (userId == null)
                return devices;
            try
            {
                devices = await GetAllManagedDevices();
                devices = devices.Where(d => d?.UserId == userId).ToList();
            }
            catch { }

            return devices;
        }

        public async Task<User?> GetGraphUserById(string? id)
        {
            if (id == null)
                return null;
            User? user = null;
            try { user = await GraphClient.Users[id].GetAsync(); }
            catch { }

            return user;
        }

        public async Task<User?> GetGraphUserByDeviceName(string? deviceName, DetectedDevice? detectedDevice = null)
        {
            if (deviceName == null && detectedDevice == null)
                return null;
            try
            {
                var machine = await GetAllManagedDevices();
                foreach (var device in machine)
                {
                    if (device == null)
                        continue;
                    if (device.DeviceName == deviceName)
                        return await GetGraphUserById(device.UserId);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<List<SubscribedSku>> GetSubscribedSkus()
        {
            var result = new List<SubscribedSku>();
            try
            {
                var sscr = await GraphClient.SubscribedSkus.GetAsync();
                string sscrJson = JsonConvert.SerializeObject(sscr);
                var sscrDynamic = JsonConvert.DeserializeObject<dynamic>(sscrJson);
                if (sscrDynamic == null)
                    return result;
                if (sscrDynamic.Value == null)
                    return result;
                foreach (var Sku in sscrDynamic.Value)
                {
                    SubscribedSku? ss = new()
                    {
                        Id = Sku.Id,
                        SkuId = Sku.SkuId,
                        AccountName = Sku.AccountName,
                        SkuPartNumber = Sku.SkuPartNumber,
                        AppliesTo = Sku.AppliesTo,
                        CapabilityStatus = Sku.CapabilityStatus,
                        ConsumedUnits = Sku.ConsumedUnits,
                        PrepaidUnits = JsonConvert.DeserializeObject<LicenseUnitsDetail>(Sku.PrepaidUnits.ToString()),
                        ServicePlans = new List<ServicePlanInfo>()
                    };
                    result.Add(ss);
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return result;
            }
        }
    }
}
