public class Team
{
    public int TeamID { get; set; }
    public string TeamName { get; set; }
    public List<Player> Players { get; set; } = new List<Player>();
}