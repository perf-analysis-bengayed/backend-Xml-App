public class GameInfo
{
    public int TemplateID { get; set; }
    public string? CompetitionName { get; set; }
    public string? City { get; set; }
    public string? Venue { get; set; }
    public int VenueID { get; set; }
    public int MatchDate { get; set; } 
    public double MatchTime { get; set; }
    public string? MatchTimeI { get; set; }
    public int TeamOneScore { get; set; }
    public int TeamTwoScore { get; set; }
    public int TeamOnLeftPeriod1 { get; set; }
    public int TeamOnLeftPeriod2 { get; set; }
    public int GameID { get; set; }
    public int Round { get; set; }
    public int NumberOfRunOnPlayers { get; set; }
    public RefereeInfo? RefereeInformation { get; set; }
    public VideoView? VideoView { get; set; }
    public List<Team> Teams { get; set; } = new List<Team>();
}