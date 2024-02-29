namespace Stardust.Database.Data
{
    public class Report
    {
        public Report() { }

        public int? Id { get; set; }
        public string? Content { get; set; }
        public string? OpenPorts { get; set; }
        public int? Type { get; set; }
        public string? ClosedPorts { get; set; }
        public string? FilteredPorts { get; set; }
        public int? MachineId { get; set; }
        public string? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public Report(int id, string content, int type, string openedports, string closedports, string filteredports, int machineid, DateTime createdat, string? userId = "")
        {
            Id = id;
            Type = type;
            Content = content;
            OpenPorts = openedports;
            ClosedPorts = closedports;
            FilteredPorts = filteredports;
            MachineId = machineid;
            CreatedAt = createdat;
            UserId = userId;
        }
    }
}
