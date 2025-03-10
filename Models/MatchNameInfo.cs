public class MatchNameInfo
{
    public DateTime MatchDate { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
     public List<string> HomeTeamPlayers { get; set; } = new List<string>();
    public List<string> AwayTeamPlayers { get; set; } = new List<string>();
    
}