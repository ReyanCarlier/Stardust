using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph.Beta;
using Microsoft.Graph.Beta.Models;
using Newtonsoft.Json.Linq;

namespace Stardust.Wrapper
{
    public class WindowsDefenderWrapper
    {
        private readonly GraphServiceClient graphClient;

        public enum SecurityAlertCategory
        {
            DefenseEvasion,
            CredentialAccess,
            PrivilegeEscalation,
            Discovery,
            LateralMovement,
            Collection,
            Exfiltration,
            CommandAndControl,
            Impact,
            All
        }

        public WindowsDefenderWrapper(GraphServiceClient _graphClient)
        {
            graphClient = _graphClient;
        }

        public async Task<List<Alert>?> GetAllAlertsAsync()
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = new string[] { "id", "category", "description", "eventDateTime", "severity", "userStates", "recommendedActions", "fileStates", "hostStates" };
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }

        public async Task<List<Alert>?> GetHighSeverityAlertsAsync()
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = "Severity eq 'High'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }

        public async Task<List<Alert>?> GetMediumSeverityAlertsAsync()
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = "Severity eq 'Medium'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }

        public async Task<List<Alert>?> GetLowSeverityAlertsAsync()
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = "Severity eq 'Low'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }

        public async Task<List<Alert>?> GetInformationalSeverityAlertsAsync()
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = "Severity eq 'Informational'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }

        public async Task<List<Alert>?> GetAlertsByCategoryAsync(SecurityAlertCategory category)
        {
            string categoryString =
                category switch
                {
                    SecurityAlertCategory.DefenseEvasion => "Category eq'DefenseEvasion'",
                    SecurityAlertCategory.CredentialAccess => "Category eq 'CredentialAccess'",
                    SecurityAlertCategory.PrivilegeEscalation => "Category eq 'PrivilegeEscalation'",
                    SecurityAlertCategory.Discovery => "Category eq 'Discovery'",
                    SecurityAlertCategory.LateralMovement => "Category eq 'LateralMovement'",
                    SecurityAlertCategory.Collection => "Category eq 'Collection'",
                    SecurityAlertCategory.Exfiltration => "Category eq 'Exfiltration'",
                    SecurityAlertCategory.CommandAndControl => "Category eq 'CommandAndControl'",
                    SecurityAlertCategory.Impact => "Category eq 'Impact'",
                    SecurityAlertCategory.All => "",
                    _ => ""
                };

            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                if (categoryString != "")
                    requestConfiguration.QueryParameters.Filter = categoryString;
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }

        public async Task<List<Alert>?> GetAlertsByDeviceIdAsync(string? deviceId)
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = $"DeviceId eq '{deviceId}'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }
        public async Task<List<Alert>?> GetAlertsByDeviceNameAsync(string? deviceName)
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = $"DeviceName eq '{deviceName}'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }
        public async Task<List<Alert>?> GetAlertsByDeviceHostNameAsync(string? deviceHostName)
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = $"DeviceHostName eq '{deviceHostName}'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }
        public async Task<List<Alert>?> GetAlertsByUserPrincipalNameAsync(string? userPrincipalName)
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = $"UserPrincipalName eq '{userPrincipalName}'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }
        public async Task<List<Alert>?> GetAlertsByUserEmailAsync(string? userEmail)
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = $"UserEmail eq '{userEmail}'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }
        public async Task<List<Alert>?> GetAlertsByUserPrincipalNameAndDeviceHostNameAsync(string? userPrincipalName, string? deviceHostName)
        {
            var alertCollectionResponse = await graphClient.Security.Alerts.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = $"UserPrincipalName eq '{userPrincipalName}' and DeviceHostName eq '{deviceHostName}'";
                requestConfiguration.QueryParameters.Top = 999;
            });
            return alertCollectionResponse?.Value;
        }
    }
}
