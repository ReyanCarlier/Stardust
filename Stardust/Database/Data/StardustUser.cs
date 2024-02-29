namespace Stardust.Database.Data
{
    public enum StardustUserRole
    {
        User,
        Admin,
        SuperAdmin
    }

    public class StardustUser
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? GraphUserId { get; set; }
        public string? Avatar { get; set; } = null;
        public StardustUserRole Role { get; set; }
        public DateTime? Created_at { get; set; }
        public string? JiraToken { get; set; }
        public string? JiraMail { get; set; }

        public StardustUser() { }

        public StardustUser(int id, string? name, string? email, string? avatar, StardustUserRole role, DateTime? created_at, string? graphUserId, string? jiraToken, string? jiraMail)
        {
            Id = id;
            Name = name;
            Email = email;
            Avatar = avatar;
            Role = role;
            Created_at = created_at;
            GraphUserId = graphUserId;
            JiraToken = jiraToken;
            JiraMail = jiraMail;
        }
    }
}
