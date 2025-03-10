using System.Text;
using System.Xml.Linq;
using System.Globalization;

public class RugbyParsingStrategy : IXmlParsingStrategy
{
    private readonly List<ActionRow> actionRows = new List<ActionRow>();
    private GameInfo gameInfo;
    private readonly ParseActionRow parseActionRow = new ParseActionRow();

    public List<ActionRow> ActionRows => actionRows;
    public GameInfo GameInfo => gameInfo;

    public void ParseElement(XElement element, StringBuilder outputBuilder)
    {
        if (element.Name.LocalName.ToUpper() == "ACTIONROW")
        {
            ActionRow row = parseActionRow.ParseActionRowElement(element);
            actionRows.Add(row);
            AppendActionRowToBuilder(row, outputBuilder);
        }
        else if (element.Name.LocalName.ToUpper() == "GAME")
        {
            gameInfo = ParseGameElement(element);
            AppendGameToBuilder(gameInfo, outputBuilder);
        }
    }

    private ActionRow ParseActionRowElement(XElement element)
    {
        return parseActionRow.ParseActionRowElement(element); // Delegate to existing ParseActionRow
    }

    private GameInfo ParseGameElement(XElement element)
    {
        var game = new GameInfo
        {
            TemplateID = int.TryParse(element.Element("TemplateID")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int templateId) ? templateId : 0,
            CompetitionName = element.Element("CompetitionName")?.Value ?? "Unknown",
            City = element.Element("City")?.Value ?? "NA",
            Venue = element.Element("Venue")?.Value ?? "Unknown",
            VenueID = int.TryParse(element.Element("VenueID")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int venueId) ? venueId : 0,
            MatchDate = int.TryParse(element.Element("MatchDate")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int matchDate) ? matchDate : 0,
            MatchTime = double.TryParse(element.Element("MatchTime")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double matchTime) ? matchTime : 0.0,
            MatchTimeI = element.Element("MatchTimeI")?.Value ?? "Unknown",
            TeamOneScore = int.TryParse(element.Element("TeamOneScore")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int teamOneScore) ? teamOneScore : 0,
            TeamTwoScore = int.TryParse(element.Element("TeamTwoScore")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int teamTwoScore) ? teamTwoScore : 0,
            TeamOnLeftPeriod1 = int.TryParse(element.Element("TeamOnLeftPeriod1")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int teamLeftP1) ? teamLeftP1 : 0,
            TeamOnLeftPeriod2 = int.TryParse(element.Element("TeamOnLeftPeriod2")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int teamLeftP2) ? teamLeftP2 : 0,
            GameID = int.TryParse(element.Element("GameID")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int gameId) ? gameId : 0,
            Round = int.TryParse(element.Element("Round")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int round) ? round : 0,
            NumberOfRunOnPlayers = int.TryParse(element.Element("NumberOfRunOnPlayers")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int players) ? players : 0
        };

        // Parse RefereeInformation
        var refereeInfoElement = element.Element("RefereeInformation");
        if (refereeInfoElement != null)
        {
            game.RefereeInformation = new RefereeInfo
            {
                Referee1 = refereeInfoElement.Element("Referee1")?.Value ?? "Unknown",
                Referee1ID = int.TryParse(refereeInfoElement.Elements("RefereeID").FirstOrDefault()?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int ref1Id) ? ref1Id : 0,
                AssistantReferee1 = refereeInfoElement.Element("AssistantReferee1")?.Value ?? "Unknown",
                AssistantReferee1ID = int.TryParse(refereeInfoElement.Elements("RefereeID").Skip(1).FirstOrDefault()?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int asst1Id) ? asst1Id : 0,
                VideoReferee1 = refereeInfoElement.Element("VideoReferee1")?.Value ?? "Unknown",
                VideoReferee1ID = int.TryParse(refereeInfoElement.Elements("RefereeID").Skip(2).FirstOrDefault()?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int vidId) ? vidId : 0,
                AssistantReferee2 = refereeInfoElement.Element("AssistantReferee2")?.Value ?? "Unknown",
                AssistantReferee2ID = int.TryParse(refereeInfoElement.Elements("RefereeID").Skip(3).FirstOrDefault()?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int asst2Id) ? asst2Id : 0
            };
        }

        // Parse VideoView
        var videoViewElement = element.Element("VideoView");
        if (videoViewElement != null)
        {
            game.VideoView = new VideoView
            {
                ViewID = int.TryParse(videoViewElement.Element("ViewID")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int viewId) ? viewId : 0,
                VideoFileName = videoViewElement.Element("VideoFileName")?.Value ?? "Unknown",
                VideoFPS = int.TryParse(videoViewElement.Element("VideoFPS")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int fps) ? fps : 0
            };
        }

        // Parse Teams and Players
        var teamElements = element.Elements("Team");
        foreach (var teamElement in teamElements)
        {
            var team = new Team
            {
                TeamID = int.TryParse(teamElement.Element("TeamID")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int teamId) ? teamId : 0,
                TeamName = teamElement.Element("TeamName")?.Value ?? "Unknown"
            };

            var playersElement = teamElement.Element("Players");
            if (playersElement != null)
            {
                foreach (var playerElement in playersElement.Elements("Player"))
                {
                    team.Players.Add(new Player
                    {
                        PlayerID = playerElement.Element("PlayerID")?.Value ?? "Unknown",
                        ShirtNum = playerElement.Element("ShirtNum")?.Value ?? "0",
                        PlayerName = playerElement.Element("PlayerName")?.Value ?? "Unknown",
                        PlayerRole = playerElement.Element("PlayerRole")?.Value ?? "0",
                        PositionNum = playerElement.Element("PositionNum")?.Value ?? "0"
                    });
                }
            }
            game.Teams.Add(team);
        }

        return game;
    }

    private void AppendActionRowToBuilder(ActionRow row, StringBuilder outputBuilder)
    {
        string outputLine = $"ID=\"{row.ID}\" " +
            $"FXID=\"{row.FXID}\" " +
            $"PLID=\"{row.PLID}\" " +
            $"team_id=\"{row.TeamId}\" " +
            $"ps_timestamp=\"{row.PsTimestamp}\" " +
            $"ps_endstamp=\"{row.PsEndstamp}\" " +
            $"MatchTime=\"{row.MatchTime}\" " +
            $"psID=\"{row.PsID}\" " +
            $"period=\"{row.Period}\" " +
            $"x_coord=\"{row.XCoord}\" " +
            $"y_coord=\"{row.YCoord}\" " +
            $"x_coord_end=\"{row.XCoordEnd}\" " +
            $"y_coord_end=\"{row.YCoordEnd}\" " +
            $"action=\"{row.Action}\" " +
            $"ActionType=\"{row.ActionType}\" " +
            $"Actionresult=\"{row.ActionResult}\" " +
            $"qualifier3=\"{row.Qualifier3}\" " +
            $"qualifier4=\"{row.Qualifier4}\" " +
            $"qualifier5=\"{row.Qualifier5}\" " +
            $"Metres=\"{row.Metres}\" " +
            $"PlayNum=\"{row.PlayNum}\" " +
            $"SetNum=\"{row.SetNum}\" " +
            $"sequence_id=\"{row.SequenceId}\" " +
            $"player_advantage=\"{row.PlayerAdvantage}\" " +
            $"score_advantage=\"{row.ScoreAdvantage}\" " +
            $"flag=\"{row.Flag}\" " +
            $"advantage=\"{row.Advantage}\" " +
            $"assoc_player=\"{row.AssocPlayer}\"";
        
        outputBuilder.AppendLine(outputLine);
    }

    private void AppendGameToBuilder(GameInfo game, StringBuilder outputBuilder)
    {
        outputBuilder.AppendLine($"TemplateID={game.TemplateID}");
        outputBuilder.AppendLine($"CompetitionName={game.CompetitionName.ToLower()}");
        outputBuilder.AppendLine($"city={game.City}");
        outputBuilder.AppendLine($"Venue={game.Venue}");
        outputBuilder.AppendLine($"VenueID={game.VenueID}");
        outputBuilder.AppendLine($"MatchDate={game.MatchDate}");
        outputBuilder.AppendLine($"MatchTime={game.MatchTime}");
        outputBuilder.AppendLine($"MatchTimeI={game.MatchTimeI}");
        outputBuilder.AppendLine($"TeamOneScore={game.TeamOneScore}");
        outputBuilder.AppendLine($"TeamTwoScore={game.TeamTwoScore}");
        outputBuilder.AppendLine($"TeamOnLeftPeriod1={game.TeamOnLeftPeriod1}");
        outputBuilder.AppendLine($"TeamOnLeftPeriod2={game.TeamOnLeftPeriod2}");
        outputBuilder.AppendLine($"GameID={game.GameID}");
        outputBuilder.AppendLine($"Round={game.Round}");
        outputBuilder.AppendLine($"NumberOfRunOnPlayers={game.NumberOfRunOnPlayers}");

        if (game.RefereeInformation != null)
        {
            outputBuilder.AppendLine($"Referee1={game.RefereeInformation.Referee1}");
            outputBuilder.AppendLine($"Referee1ID={game.RefereeInformation.Referee1ID}");
            outputBuilder.AppendLine($"AssistantReferee1={game.RefereeInformation.AssistantReferee1}");
            outputBuilder.AppendLine($"AssistantReferee1ID={game.RefereeInformation.AssistantReferee1ID}");
            outputBuilder.AppendLine($"VideoReferee1={game.RefereeInformation.VideoReferee1}");
            outputBuilder.AppendLine($"VideoReferee1ID={game.RefereeInformation.VideoReferee1ID}");
            outputBuilder.AppendLine($"AssistantReferee2={game.RefereeInformation.AssistantReferee2}");
            outputBuilder.AppendLine($"AssistantReferee2ID={game.RefereeInformation.AssistantReferee2ID}");
        }

        if (game.VideoView != null)
        {
            outputBuilder.AppendLine($"ViewID={game.VideoView.ViewID}");
            outputBuilder.AppendLine($"VideoFileName={game.VideoView.VideoFileName}");
            outputBuilder.AppendLine($"VideoFPS={game.VideoView.VideoFPS}");
        }

        foreach (var team in game.Teams)
        {
            outputBuilder.AppendLine($"TeamID={team.TeamID}");
            outputBuilder.AppendLine($"TeamName={team.TeamName}");
            foreach (var player in team.Players)
            {
                outputBuilder.AppendLine($"PlayerID={player.PlayerID}");
                outputBuilder.AppendLine($"ShirtNum={player.ShirtNum}");
                outputBuilder.AppendLine($"PlayerName={player.PlayerName}");
                outputBuilder.AppendLine($"PlayerRole={player.PlayerRole}");
                outputBuilder.AppendLine($"PositionNum={player.PositionNum}");
            }
        }
    }
}