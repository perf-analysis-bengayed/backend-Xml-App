public class InstanceData
    {
        public string? Start { get; set; }
        public string? End { get; set; }
        public string? Code { get; set; }
        public string? PosX { get; set; }
        public string? PosY { get; set; }
        public List<Label> Labels { get; set; } = new List<Label>();
        public string? Team { get; set; }
        public string? Action { get; set; }
        public string? Half { get; set; }
    }