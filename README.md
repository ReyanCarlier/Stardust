# Stardust
### Monitor managed and unmanaged devices on your network, with Nmap, Microsoft Graph, in an Azure AD environment.

# I hereby disclaim any liability for misuse of Stardust. For educational use only. (See Warnings)

Implementation as part of a 6-month cybersecurity internship, 2 months of this internship were dedicated to Stardust development in solo.
Stardust is written in C#/ASP.NET & Blazor Server, and is accessible only if you have a Microsoft Azure account with sufficient authorizations.
This repository has been curated of every confidential informations, thus, it might need you to do some configurations to make it work.

An additionnal database made with SQLite is also used by Stardust to store devices, reports and scripts.
An empty database is also provided and distributed here.

## Warnings
- Make sure you are the owner of the network where you use Stardust or to have the explicit autorization of the owner to scan the network you're on. I 
- Stardust is functional but some implementations of Microsoft Defender were removed due to license reasons.
- `Atlassian.Jira.dll` library included in this project is a slightly modified version of [Atlassian.Net-SDK](https://github.com/kalhorim/Atlassian.Net-SDK) and keep its own license. (See `Dependencies/Atlassian.Jira.dll.LICENSE.md`)

## Requirements

- Nmap installed
- Windows environment
- .NET7 SDK
- NO VPN (Nmap won't work properly)
- Don't remove the footer and its content from `MainLayout.razor`

## Configuration

### Stardust.csproj
- Modify the tag `<UserSecretsId>` with your own.

### Modifiy appsettings.json

```
[...]
"AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "",       <--- Add your Azure AD domain
    "TenantId": "",     <--- Add your Azure AD tenant ID
    "ClientId": "",     <--- Add your Azure AD client ID
    "ClientSecret": "", <--- Add your Azure AD client secret
    "CallbackPath": "/signin-oidc"
},
[...]
"Nmap": {
    "ScriptFolder": "C:\\Program Files (x86)\\Nmap\\scripts\\" <--- Modify the path if needed (don't forget double \\)
},
[...]
"ConnectionStrings": {
    "Stardust": "Data Source=Database\\Stardust.db",    <--- Path to SQLite Database (default)
    "MySQL": ""                                         <--- MySQL connexion string
}
```

### Modify Wrapper/JiraClient.cs

```
public async Task<List<Issue>> GetIssuesAsync(string projectKey)
{
    [...]
    // Modify the status with the one you use on Jira
    var issues = await _jiraClient.Issues.GetIssuesFromJqlAsync($"project={projectKey} AND status NOT IN (Annulé, Canceled, [...])");
    [...]
}
```

