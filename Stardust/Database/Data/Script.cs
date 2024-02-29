namespace Stardust.Database.Data
{
    public enum ScriptDefault
    {
        True,
        False
    }

    public enum ScriptType
    {
        Intrusive,
        Safe
    }

    public enum ScriptCategory
    {
        Authentication,
        Broadcast,
        Brute,
        Discovery,
        Dos,
        Exploit,
        External,
        Fuzzer,
        Intrusive,
        Malware,
        Safe,
        Version,
        Vulnerability,
        Default,
    }
    public class Script
    {
        public Script() => ConcernedPorts = string.Empty;

        public int? Id { get; set; }
        public string? Name { get; set; }
        public ScriptCategory Category { get; set; }
        public ScriptDefault Default { get; set; }
        public ScriptType Type { get; set; }
        public string? Description { get; set; }
        public string ConcernedPorts { get; set; }
        public string? ScriptContent { get; set; }

        public Script(int? id, string name, ScriptCategory category, ScriptDefault scriptDefault, ScriptType type, string? description, string concernedports, string scriptContent)
        {
            Id = id;
            Name = name;
            Category = category;
            Default = scriptDefault;
            Type = type;
            Description = description;
            ConcernedPorts = concernedports;
            ScriptContent = scriptContent;
        }
    }
}
