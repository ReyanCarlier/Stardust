using Atlassian.Jira;
using Newtonsoft.Json;
using RestSharp;

namespace Stardust.Wrapper
{
    public class JiraClient
    {
        public readonly Jira _jiraClient;

        public JiraClient(string jiraUrl, string? userEmail, string? apiToken)
        {
            _jiraClient = Jira.CreateRestClient(jiraUrl, userEmail, apiToken);
        }

        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                var user = await _jiraClient.Users.GetMyselfAsync();
                return user != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Issue>> GetIssuesAsync(string projectKey)
        {
            try
            {
                List<Issue> result = new();
                var issues = await _jiraClient.Issues.GetIssuesFromJqlAsync($"project={projectKey} AND status NOT IN (Annulé, Canceled, Closed, Completed, Declined, Done, EMPTY, Brouillon, Completed, Done)");
                foreach (var issue in issues)
                {
                    result.Add(issue);
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Issue>();
            }
        }

        public async Task<List<string>> GetAllProjectsKeysAsync()
        {
            var projects = await _jiraClient.Projects.GetProjectsAsync();
            return projects.Select(p => p.Key).ToList();
        }

        public class ObjectEntry
        {
            [JsonProperty("workspaceId")]
            public string WorkspaceId { get; set; }
        }

        public class Results
        {
            [JsonProperty("objectEntries")]
            public List<ObjectEntry> ObjectEntries { get; set; } = new();
        }

        public class WorkspaceResponse
        {
            public int Size { get; set; }
            public int Start { get; set; }
            public int Limit { get; set; }
            public bool IsLastPage { get; set; }
            public Links Links { get; set; } = new();
            public List<WorkspaceValue> Values { get; set; } = new();
        }

        public class Links
        {
            public string Self { get; set; } = "";
            public string Base { get; set; } = "";
            public string Context { get; set; } = "";
        }

        public class WorkspaceValue
        {
            [JsonProperty("workspaceId")]
            public string WorkspaceId { get; set; }
        }


        public async Task<string?> GetWorkspaceId()
        {
            try
            {
                var workspaceIdEndpoint = "/rest/servicedeskapi/assets/workspace";
                RestRequest restRequest = new(workspaceIdEndpoint, Method.Get);
                var workspaceIdResponse = await _jiraClient.RestClient.RestSharpClient.ExecuteAsync(restRequest);
                dynamic? response = JsonConvert.DeserializeObject<dynamic>(workspaceIdResponse.Content);

                if (response == null)
                    return null;

                return response?.values[0]?.workspaceId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<string?> GetAllAssets()
        {
            string? workspaceId = await GetWorkspaceId();
            if (workspaceId == null)
            {
                return null;
            }

            try
            {
                var assetsEndpoint = $"/gateway/api/jsm/assets/workspace/{workspaceId}/v1/object/aql";
                var data = new
                {
                    qlQuery = "objectTypeId = 45",
                    startAt = 0,
                    maxResults = 500,
                    includeAttributes = true
                };

                string jsonData = JsonConvert.SerializeObject(data);
                RestRequest request = new(assetsEndpoint, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", jsonData, ParameterType.RequestBody);

                var assetsResponse = await _jiraClient.RestClient.ExecuteRequestAsync(request);

                if (assetsResponse.IsSuccessful)
                {
                    return assetsResponse.Content;
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve assets: {assetsResponse.ErrorMessage}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return null;
            }
        }


        public async Task<Issue?> CreateIssueAsync(string projectKey, string issueType, string summary, string description)
        {
            var issue = new Issue(_jiraClient, projectKey, issueType)
            {
                Summary = summary,
                Description = description
            };
            try
            {
                await issue.SaveChangesAsync();
                return issue;
            }
            catch
            {
                return null;
            }
        }
        public async Task<Issue?> GetIssueAsync(string issueKey)
        {
            try
            {
                var issue = await _jiraClient.Issues.GetIssueAsync(issueKey);
                return issue;
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> AddCommentToIssueAsync(string issueKey, string comment)
        {
            try
            {
                var issue = await _jiraClient.Issues.GetIssueAsync(issueKey);
                await issue.AddCommentAsync(comment);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
