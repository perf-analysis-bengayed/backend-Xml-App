using System.Text;
using System.Xml.Linq;

public class RugbyParsingStrategy : IXmlParsingStrategy
{
    private readonly List<ActionRow> actionRows = new List<ActionRow>();
    private readonly ParseActionRow parseActionRow = new ParseActionRow();
    
    public List<ActionRow> ActionRows => actionRows;

    public void ParseElement(XElement element, StringBuilder outputBuilder)
    {
        if (element.Name.LocalName.ToUpper() == "ACTIONROW")
        {
            ActionRow row = parseActionRow.ParseActionRowElement(element);
            actionRows.Add(row);
            AppendRowToBuilder(row, outputBuilder);
        }
    }

    private void AppendRowToBuilder(ActionRow row, StringBuilder outputBuilder)
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
}