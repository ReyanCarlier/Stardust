using System.Text.Json.Serialization;

namespace Stardust.ClassJSON
{
    public class MicrosoftUser
    {
        [JsonPropertyName("@odata.context")]
        public string? OdataContext { get; set; }
        public string[]? BusinessPhones { get; set; }
        public string? DisplayName { get; set; }
        public string? GivenName { get; set; }
        public string? JobTitle { get; set; }
        public string? Mail { get; set; }
        public string? MobilePhone { get; set; }
        public string? OfficeLocation { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? Surname { get; set; }
        public string? UserPrincipalName { get; set; }
        public string? Id { get; set; }
        public string? Avatar { get; set; }
    }
}
