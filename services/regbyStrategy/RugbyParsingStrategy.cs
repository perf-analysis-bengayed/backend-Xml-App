using System.Text;
using System.Xml.Linq;


public class RugbyParsingStrategy : IXmlParsingStrategy
{
    private readonly List<ActionRow> actionRows = new List<ActionRow>();
    private GameInfo gameInfo;
    private readonly List<StatEvent> statEvents = new List<StatEvent>();
        private readonly ParseActionRow parseActionRow = new ParseActionRow();
    private readonly List<Player> players = new List<Player>();
    public List<ActionRow> ActionRows => actionRows;
    public GameInfo GameInfo => gameInfo;
    public List<Player> Players => players;
    public List<StatEvent> StatEvents => statEvents;
    public void ParseElement(XElement element, StringBuilder outputBuilder)
    {
        switch (element.Name.LocalName.ToUpper())
        {
            case "ACTIONROW":
                ActionRow row = parseActionRow.ParseActionRowElement(element);
                actionRows.Add(row);
                AppendActionRowToBuilder(row, outputBuilder);
                break;

            case "GAME":
              GameInfo game = parseActionRow.ParseGameElement(element);
                gameInfo = game; 
                AppendGameToBuilder(game, outputBuilder);
                break;

            case "PLAYER":
                Player player = parseActionRow.ParsePlayerElement(element);
                players.Add(player); 
                parseActionRow.AppendPlayerToBuilder(player, outputBuilder); 
                break;
            case "STATEVENT":  
                StatEvent statEvent = parseActionRow.ParseStatEventElement(element);
                if (statEvent != null)
                {
                    statEvents.Add(statEvent);
                    AppendStatEventToBuilder(statEvent, outputBuilder);
                }
                break;
        }
    }
private void AppendStatEventToBuilder(StatEvent statEvent, StringBuilder outputBuilder)
    {
        var statEventData = new
        {
            TE_IDX = statEvent.TE_IDX,
            QL_IDX = statEvent.QL_IDX,
            FieldX = statEvent.FieldX,
            FieldY = statEvent.FieldY,
            TeamID = statEvent.TeamID,
            PlayerID = statEvent.PlayerID,
            VidRef = statEvent.VidRef,
            Stop = statEvent.Stop,
            StatLevel0 = statEvent.StatLevel0,
            StatLevel1 = statEvent.StatLevel1,
            StatVal = statEvent.StatVal,
            RefereeID = statEvent.RefereeID,
            Period = statEvent.Period,
            TackleCount = statEvent.TackleCount,
            SetCount = statEvent.SetCount
        };

        string outputLine = $"TE_IDX={statEventData.TE_IDX} " +
            $"QL_IDX={statEventData.QL_IDX} " +
            $"FieldX={statEventData.FieldX} " +
            $"FieldY={statEventData.FieldY} " +
            $"TeamID={statEventData.TeamID} " +
            $"PlayerID={statEventData.PlayerID} " +
            $"VidRef={statEventData.VidRef} " +
            (statEventData.Stop.HasValue ? $"Stop={statEventData.Stop} " : "") +
            $"StatLevel0=\"{statEventData.StatLevel0}\" " +
            (statEventData.StatLevel1 != null ? $"StatLevel1=\"{statEventData.StatLevel1}\" " : "") +
            $"StatVal={statEventData.StatVal} " +
            $"RefereeID={statEventData.RefereeID} " +
            $"Period={statEventData.Period} " +
            $"TackleCount={statEventData.TackleCount} " +
            $"SetCount={statEventData.SetCount}";

        outputBuilder.AppendLine(outputLine);
    }
    private void AppendActionRowToBuilder(ActionRow row, StringBuilder outputBuilder)
    {
        
        var actionRowData = new
        {
            Id = row.ID,
            FxId = row.FXID,
            PlId = row.PLID,
            TeamId = row.TeamId,
            PsTimestamp = row.PsTimestamp,
            PsEndstamp = row.PsEndstamp,
            MatchTime = row.MatchTime,
            PsId = row.PsID,
            Period = row.Period,
            XCoord = row.XCoord,
            YCoord = row.YCoord,
            XCoordEnd = row.XCoordEnd,
            YCoordEnd = row.YCoordEnd,
            Action = row.Action,
            ActionType = row.ActionType,
            ActionResult = row.ActionResult,
            Qualifier3 = row.Qualifier3,
            Qualifier4 = row.Qualifier4,
            Qualifier5 = row.Qualifier5,
            Metres = row.Metres,
            PlayNum = row.PlayNum,
            SetNum = row.SetNum,
            SequenceId = row.SequenceId,
            PlayerAdvantage = row.PlayerAdvantage,
            ScoreAdvantage = row.ScoreAdvantage,
            Flag = row.Flag,
            Advantage = row.Advantage,
            AssocPlayer = row.AssocPlayer
        };

        string outputLine = $"ID=\"{actionRowData.Id}\" " +
            $"FXID=\"{actionRowData.FxId}\" " +
            $"PLID=\"{actionRowData.PlId}\" " +
            $"team_id=\"{actionRowData.TeamId}\" " +
            $"ps_timestamp=\"{actionRowData.PsTimestamp}\" " +
            $"ps_endstamp=\"{actionRowData.PsEndstamp}\" " +
            $"MatchTime=\"{actionRowData.MatchTime}\" " +
            $"psID=\"{actionRowData.PsId}\" " +
            $"period=\"{actionRowData.Period}\" " +
            $"x_coord=\"{actionRowData.XCoord}\" " +
            $"y_coord=\"{actionRowData.YCoord}\" " +
            $"x_coord_end=\"{actionRowData.XCoordEnd}\" " +
            $"y_coord_end=\"{actionRowData.YCoordEnd}\" " +
            $"action=\"{actionRowData.Action}\" " +
            $"ActionType=\"{actionRowData.ActionType}\" " +
            $"Actionresult=\"{actionRowData.ActionResult}\" " +
            $"qualifier3=\"{actionRowData.Qualifier3}\" " +
            $"qualifier4=\"{actionRowData.Qualifier4}\" " +
            $"qualifier5=\"{actionRowData.Qualifier5}\" " +
            $"Metres=\"{actionRowData.Metres}\" " +
            $"PlayNum=\"{actionRowData.PlayNum}\" " +
            $"SetNum=\"{actionRowData.SetNum}\" " +
            $"sequence_id=\"{actionRowData.SequenceId}\" " +
            $"player_advantage=\"{actionRowData.PlayerAdvantage}\" " +
            $"score_advantage=\"{actionRowData.ScoreAdvantage}\" " +
            $"flag=\"{actionRowData.Flag}\" " +
            $"advantage=\"{actionRowData.Advantage}\" " +
            $"assoc_player=\"{actionRowData.AssocPlayer}\"";

        outputBuilder.AppendLine(outputLine);
    }

    private void AppendGameToBuilder(GameInfo game, StringBuilder outputBuilder)
    {
        
        var gameData = new
        {
            TemplateId = game.TemplateID,
            CompetitionName = game.CompetitionName?.ToLower(),
            City = game.City,
            Venue = game.Venue,
            VenueId = game.VenueID,
            MatchDate = game.MatchDate,
            MatchTime = game.MatchTime,
            MatchTimeI = game.MatchTimeI,
            TeamOneScore = game.TeamOneScore,
            TeamTwoScore = game.TeamTwoScore,
            TeamOnLeftPeriod1 = game.TeamOnLeftPeriod1,
            TeamOnLeftPeriod2 = game.TeamOnLeftPeriod2,
            GameId = game.GameID,
            Round = game.Round,
            NumberOfRunOnPlayers = game.NumberOfRunOnPlayers,
            RefereeInfo = game.RefereeInformation,
            VideoView = game.VideoView,
            Teams = game.Teams
        };

        outputBuilder.AppendLine($"TemplateID={gameData.TemplateId}");
        outputBuilder.AppendLine($"CompetitionName={gameData.CompetitionName}");
        outputBuilder.AppendLine($"city={gameData.City}");
        outputBuilder.AppendLine($"Venue={gameData.Venue}");
        outputBuilder.AppendLine($"VenueID={gameData.VenueId}");
        outputBuilder.AppendLine($"MatchDate={gameData.MatchDate}");
        outputBuilder.AppendLine($"MatchTime={gameData.MatchTime}");
        outputBuilder.AppendLine($"MatchTimeI={gameData.MatchTimeI}");
        outputBuilder.AppendLine($"TeamOneScore={gameData.TeamOneScore}");
        outputBuilder.AppendLine($"TeamTwoScore={gameData.TeamTwoScore}");
        outputBuilder.AppendLine($"TeamOnLeftPeriod1={gameData.TeamOnLeftPeriod1}");
        outputBuilder.AppendLine($"TeamOnLeftPeriod2={gameData.TeamOnLeftPeriod2}");
        outputBuilder.AppendLine($"GameID={gameData.GameId}");
        outputBuilder.AppendLine($"Round={gameData.Round}");
        outputBuilder.AppendLine($"NumberOfRunOnPlayers={gameData.NumberOfRunOnPlayers}");

        if (gameData.RefereeInfo != null)
        {
            outputBuilder.AppendLine($"Referee1={gameData.RefereeInfo.Referee1}");
            outputBuilder.AppendLine($"Referee1ID={gameData.RefereeInfo.Referee1ID}");
            outputBuilder.AppendLine($"AssistantReferee1={gameData.RefereeInfo.AssistantReferee1}");
            outputBuilder.AppendLine($"AssistantReferee1ID={gameData.RefereeInfo.AssistantReferee1ID}");
            outputBuilder.AppendLine($"VideoReferee1={gameData.RefereeInfo.VideoReferee1}");
            outputBuilder.AppendLine($"VideoReferee1ID={gameData.RefereeInfo.VideoReferee1ID}");
            outputBuilder.AppendLine($"AssistantReferee2={gameData.RefereeInfo.AssistantReferee2}");
            outputBuilder.AppendLine($"AssistantReferee2ID={gameData.RefereeInfo.AssistantReferee2ID}");
        }

        if (gameData.VideoView != null)
        {
            outputBuilder.AppendLine($"ViewID={gameData.VideoView.ViewID}");
            outputBuilder.AppendLine($"VideoFileName={gameData.VideoView.VideoFileName}");
            outputBuilder.AppendLine($"VideoFPS={gameData.VideoView.VideoFPS}");
        }

        foreach (var team in gameData.Teams)
        {
            outputBuilder.AppendLine($"TeamID={team.TeamID}");
            outputBuilder.AppendLine($"TeamName={team.TeamName}");
            foreach (var player in team.Players)
            {
                var playerData = new
                {
                    PlayerId = player.PlayerID,
                    ShirtNum = player.ShirtNum,
                    PlayerName = player.PlayerName,
                    PlayerRole = player.PlayerRole,
                    PositionNum = player.PositionNum
                };

                outputBuilder.AppendLine($"PlayerID={playerData.PlayerId}");
                outputBuilder.AppendLine($"ShirtNum={playerData.ShirtNum}");
                outputBuilder.AppendLine($"PlayerName={playerData.PlayerName}");
                outputBuilder.AppendLine($"PlayerRole={playerData.PlayerRole}");
                outputBuilder.AppendLine($"PositionNum={playerData.PositionNum}");
            }
        }
    }
}