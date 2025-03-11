using System.Text;
using System.Xml.Linq;
using System.Globalization;
using System.Linq;

public class ParseActionRow
{
    public ActionRow ParseActionRowElement(XElement element)
    {
        return new ActionRow
        {
            ID = GetIntAttribute(element, "ID", 0),
            FXID = GetStringAttribute(element, "FXID", ""),
            PLID = GetIntAttribute(element, "PLID", 0),
            TeamId = GetIntAttribute(element, "team_id", 0),
            PsTimestamp = GetDoubleAttribute(element, "ps_timestamp", 0.0),
            PsEndstamp = GetDoubleAttribute(element, "ps_endstamp", 0.0),
            MatchTime = GetIntAttribute(element, "MatchTime", 0),
            PsID = GetIntAttribute(element, "psID", 0),
            Period = GetIntAttribute(element, "period", 0),
            XCoord = GetIntAttribute(element, "x_coord", 0),
            YCoord = GetIntAttribute(element, "y_coord", 0),
            XCoordEnd = GetIntAttribute(element, "x_coord_end", 0),
            YCoordEnd = GetIntAttribute(element, "y_coord_end", 0),
            Action = GetIntAttribute(element, "action", 0),
            ActionType = GetIntAttribute(element, "ActionType", 0),
            ActionResult = GetIntAttribute(element, "Actionresult", 0),
            Qualifier3 = GetIntAttribute(element, "qualifier3", 0),
            Qualifier4 = GetIntAttribute(element, "qualifier4", 0),
            Qualifier5 = GetIntAttribute(element, "qualifier5", 0),
            Metres = GetIntAttribute(element, "Metres", 0),
            PlayNum = GetIntAttribute(element, "PlayNum", 0),
            SetNum = GetIntAttribute(element, "SetNum", 0),
            SequenceId = GetIntAttribute(element, "sequence_id", 0),
            PlayerAdvantage = GetIntAttribute(element, "player_advantage", 0),
            ScoreAdvantage = GetIntAttribute(element, "score_advantage", 0),
            Flag = bool.TryParse(element.Attribute("flag")?.Value, out bool flag) ? flag : false,
            Advantage = GetIntAttribute(element, "advantage", 0),
            AssocPlayer = GetIntAttribute(element, "assoc_player", 0)
        };
    }

    public GameInfo ParseGameElement(XElement element)
    {
        var game = new GameInfo
        {
            TemplateID = GetIntElement(element, "TemplateID", 0),
            CompetitionName = GetStringElement(element, "CompetitionName", "Unknown"),
            City = GetStringElement(element, "City", "NA"),
            Venue = GetStringElement(element, "Venue", "Unknown"),
            VenueID = GetIntElement(element, "VenueID", 0),
            MatchDate = GetIntElement(element, "MatchDate", 0),
            MatchTime = GetDoubleElement(element, "MatchTime", 0.0),
            MatchTimeI = GetStringElement(element, "MatchTimeI", "Unknown"),
            TeamOneScore = GetIntElement(element, "TeamOneScore", 0),
            TeamTwoScore = GetIntElement(element, "TeamTwoScore", 0),
            TeamOnLeftPeriod1 = GetIntElement(element, "TeamOnLeftPeriod1", 0),
            TeamOnLeftPeriod2 = GetIntElement(element, "TeamOnLeftPeriod2", 0),
            GameID = GetIntElement(element, "GameID", 0),
            Round = GetIntElement(element, "Round", 0),
            NumberOfRunOnPlayers = GetIntElement(element, "NumberOfRunOnPlayers", 0),
            RefereeInformation = ParseRefereeInformation(element.Element("RefereeInformation") ?? new XElement("RefereeInformation")),
            VideoView = ParseVideoView(element.Element("VideoView") ?? new XElement("VideoView")),
            Teams = ParseTeams(element.Elements("Team"))
        };

        return game;
    }

   
    private int GetIntAttribute(XElement element, string name, int defaultValue) =>
        int.TryParse(element.Attribute(name)?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int result) ? result : defaultValue;

    private double GetDoubleAttribute(XElement element, string name, double defaultValue) =>
        double.TryParse(element.Attribute(name)?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : defaultValue;

    private string GetStringAttribute(XElement element, string name, string defaultValue) =>
        element.Attribute(name)?.Value ?? defaultValue;

    private int GetIntElement(XElement element, string name, int defaultValue) =>
        int.TryParse(element.Element(name)?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int result) ? result : defaultValue;

    private double GetDoubleElement(XElement element, string name, double defaultValue) =>
        double.TryParse(element.Element(name)?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : defaultValue;

    private string GetStringElement(XElement element, string name, string defaultValue) =>
        element.Element(name)?.Value ?? defaultValue;

    
 private RefereeInfo ParseRefereeInformation(XElement element)
{
    if (element == null)
    {
        return new RefereeInfo
        {
            Referee1 = "Unknown",
            Referee1ID = 0,
            AssistantReferee1 = "Unknown",
            AssistantReferee1ID = 0,
            VideoReferee1 = "Unknown",
            VideoReferee1ID = 0,
            AssistantReferee2 = "Unknown",
            AssistantReferee2ID = 0
        };
    }

    var refereeIds = element.Elements("RefereeID").ToList();

    return new RefereeInfo
    {
        Referee1 = GetStringElement(element, "Referee1", "Unknown"),
        Referee1ID = GetIntElement(element, "RefereeID", 0),
        AssistantReferee1 = GetStringElement(element, "AssistantReferee1", "Unknown"),
        AssistantReferee1ID = refereeIds.Count > 1 ? GetIntElement(refereeIds[1], "RefereeID", 0) : 0,
        VideoReferee1 = GetStringElement(element, "VideoReferee1", "Unknown"),
        VideoReferee1ID = refereeIds.Count > 2 ? GetIntElement(refereeIds[2], "RefereeID", 0) : 0,
        AssistantReferee2 = GetStringElement(element, "AssistantReferee2", "Unknown"),
        AssistantReferee2ID = refereeIds.Count > 3 ? GetIntElement(refereeIds[3], "RefereeID", 0) : 0
    };
}

    private VideoView ParseVideoView(XElement element)
    {
        if (element == null) return null;

        return new VideoView
        {
            ViewID = GetIntElement(element, "ViewID", 0),
            VideoFileName = GetStringElement(element, "VideoFileName", "Unknown"),
            VideoFPS = GetIntElement(element, "VideoFPS", 0)
        };
    }

 private List<Team> ParseTeams(IEnumerable<XElement> teamElements)
    {
        var teams = new List<Team>();
        foreach (var teamElement in teamElements)
        {
            var team = new Team
            {
                TeamID = GetIntElement(teamElement, "TeamID", 0),
                TeamName = GetStringElement(teamElement, "TeamName", "Unknown")
            };

            var playersElement = teamElement.Element("Players");
            if (playersElement != null)
            {
                team.Players.AddRange(playersElement.Elements("Player").Select(p => ParsePlayerElement(p)));
            }
            teams.Add(team);
        }
        return teams;
    }
public Player ParsePlayerElement(XElement element)
    {
       var player = new Player
    {
        PlayerID = GetStringElement(element, "PlayerID", "Unknown"),
        ShirtNum = GetStringElement(element, "ShirtNum", "0"),
        PlayerName = GetStringElement(element, "PlayerName", "Unknown"),
        PlayerRole = GetStringElement(element, "PlayerRole", "0"),
        PositionNum = GetStringElement(element, "PositionNum", "0")
    };

    // Check if the player has meaningful data; if not, return null
    if (player.PlayerID == "Unknown" && player.PlayerName == "Unknown" && 
        player.ShirtNum == "0" && player.PlayerRole == "0" && player.PositionNum == "0")
    {
        return null; // Skip this player
    }

    return player;
    }

    public void AppendPlayerToBuilder(Player player, StringBuilder outputBuilder)
    {
        string outputLine = $"player id {player.PlayerID} ShirtNum {player.ShirtNum} " +
                           $"PlayerName {player.PlayerName} PlayerRole {player.PlayerRole} " +
                           $"PositionNum {player.PositionNum}";
        outputBuilder.AppendLine(outputLine);
    }

public StatEvent ParseStatEventElement(XElement element)
    {
        if (element == null) return null;

        var statEvent = new StatEvent
        {
            TE_IDX = GetIntElement(element, "TE_IDX", 0),
            QL_IDX = GetIntElement(element, "QL_IDX", 0),
            FieldX = GetIntElement(element, "FieldX", 0),
            FieldY = GetIntElement(element, "FieldY", 0),
            TeamID = GetIntElement(element, "TeamID", 0),
            PlayerID = GetIntElement(element, "PlayerID", 0),
            VidRef = GetIntElement(element, "VidRef", 0),
            Stop = element.Element("Stop") != null ? GetIntElement(element, "Stop", 0) : (int?)null,
            StatLevel0 = GetStringElement(element, "StatLevel0", ""),
            StatLevel1 = GetStringElement(element, "StatLevel1", null),
            StatVal = GetDoubleElement(element, "StatVal", 0.0),
            RefereeID = GetIntElement(element, "RefereeID", 0),
            Period = GetIntElement(element, "Period", 0),
            TackleCount = GetIntElement(element, "TackleCount", 0),
            SetCount = GetIntElement(element, "SetCount", 0)
        };

        var vidFileOffsets = element.Element("VidFileOffSets");
        if (vidFileOffsets != null)
        {
            statEvent.VidFileOffsets = new VidFileOffset
            {
                ViewNum = GetIntElement(vidFileOffsets, "ViewNum", 0),
                NewFileNum = GetIntElement(vidFileOffsets, "NewFileNum", 0),
                FrameOffSet = GetIntElement(vidFileOffsets, "FrameOffSet", 0)
            };
        }

        return statEvent;
    }
}   

