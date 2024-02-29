namespace Stardust.Database.Data
{
    public class GlobalScan
    {
        public int Id { get; set; }
        public string? Initiated_By { get; set; }
        public string? Description { get; set; }
        public DateTime? Launched_At { get; set; }
        public DateTime? Ended_At { get; set; }
        public string? DevicesIds { get; set; }

        public GlobalScan() { }

        public GlobalScan(int id, string? name, string? description, DateTime? created_at, DateTime? updated_at, string? devicesIds)
        {
            Id = id;
            Initiated_By = name;
            Description = description;
            Launched_At = created_at;
            Ended_At = updated_at;
            DevicesIds = devicesIds;
        }
    }
}
