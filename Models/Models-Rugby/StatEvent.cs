public class StatEvent
{
    public int TE_IDX { get; set; }
    public int QL_IDX { get; set; }
    public int FieldX { get; set; }
    public int FieldY { get; set; }
    public int TeamID { get; set; }
    public int PlayerID { get; set; }
    public int VidRef { get; set; }
    public int? Stop { get; set; }
    public string StatLevel0 { get; set; }
    public string StatLevel1 { get; set; }
    public double StatVal { get; set; }
    public int RefereeID { get; set; }
    public int Period { get; set; }
    public int TackleCount { get; set; }
    public int SetCount { get; set; }
    // Add VidFileOffSets if needed
    public VidFileOffset VidFileOffsets { get; set; }
}