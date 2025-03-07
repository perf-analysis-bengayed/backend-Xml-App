public class MatchInfo
    {
    public DateTime MatchDate { get; set; }
    public string HomeTeam { get; set; }
    public string AwayTeam { get; set; }
   public List<string> ParsedContents { get; set; } = new List<string>();
    // public List<string> ListeJoueursEquipeLocale { get; set; }
    // public List<string> ListeJoueursEquipeVisiteuse { get; set; }
    // public List<string> FichiersVideo { get; set; }
    // public string CoteTerrainPremierePeriode { get; set; }
    // public List<string> TimecodesDebutPeriodes { get; set; }
    }