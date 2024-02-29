namespace Stardust.Database.Data
{
    public enum TodoTaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        WaitingOnSomeoneElse,
        Deferred,
        Deleted
    }

    public enum TodoTaskEmergency
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class TaskTodo
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? DeviceId { get; set; }
        public string? UserId { get; set; }
        public int? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public int? ReportId { get; set; }
        public string? Description { get; set; }
        public TodoTaskStatus Status { get; set; }
        public TodoTaskEmergency Emergency { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }

        public TaskTodo()
        {
        }

        public TaskTodo(TaskTodo other)
        {
            Id = other.Id;
            Title = other.Title;
            DeviceId = other.DeviceId;
            UserId = other.UserId;
            AssignedToId = other.AssignedToId;
            AssignedToName = other.AssignedToName;
            ReportId = other.ReportId;
            Description = other.Description;
            Status = other.Status;
            Emergency = other.Emergency;
            DueDate = other.DueDate;
            CompletedDate = other.CompletedDate;
            CreatedDate = other.CreatedDate;
            LastUpdatedDate = other.LastUpdatedDate;
        }

        public TaskTodo(int id, string? title, int? deviceId, string? userId, int? assignedToId, string? assignedToName, int? reportId, string? description, TodoTaskStatus status, TodoTaskEmergency emergency, DateTime? dueDate, DateTime? completedDate, DateTime? createdDate, DateTime? lastUpdatedDate)
        {
            Id = id;
            Title = title;
            DeviceId = deviceId;
            UserId = userId;
            AssignedToId = assignedToId;
            AssignedToName = assignedToName;
            ReportId = reportId;
            Description = description;
            Status = status;
            Emergency = emergency;
            DueDate = dueDate;
            CompletedDate = completedDate;
            CreatedDate = createdDate;
            LastUpdatedDate = lastUpdatedDate;
        }
    }
}
