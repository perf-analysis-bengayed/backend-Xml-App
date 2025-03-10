using System.Text;
using System.Xml.Linq;


   public class ParseActionRow
{
    public ActionRow ParseActionRowElement(XElement element)
    {
        return new ActionRow
        {
            ID = int.Parse(element.Attribute("ID")?.Value ?? "0"),
            FXID = element.Attribute("FXID")?.Value ?? "",
            PLID = int.Parse(element.Attribute("PLID")?.Value ?? "0"),
            TeamId = int.Parse(element.Attribute("team_id")?.Value ?? "0"),
            PsTimestamp = double.Parse(element.Attribute("ps_timestamp")?.Value ?? "0.0"),
            PsEndstamp = double.Parse(element.Attribute("ps_endstamp")?.Value ?? "0.0"),
            MatchTime = int.Parse(element.Attribute("MatchTime")?.Value ?? "0"),
            PsID = int.Parse(element.Attribute("psID")?.Value ?? "0"),
            Period = int.Parse(element.Attribute("period")?.Value ?? "0"),
            XCoord = int.Parse(element.Attribute("x_coord")?.Value ?? "0"),
            YCoord = int.Parse(element.Attribute("y_coord")?.Value ?? "0"),
            XCoordEnd = int.Parse(element.Attribute("x_coord_end")?.Value ?? "0"),
            YCoordEnd = int.Parse(element.Attribute("y_coord_end")?.Value ?? "0"),
            Action = int.Parse(element.Attribute("action")?.Value ?? "0"),
            ActionType = int.Parse(element.Attribute("ActionType")?.Value ?? "0"),
            ActionResult = int.Parse(element.Attribute("Actionresult")?.Value ?? "0"),
            Qualifier3 = int.Parse(element.Attribute("qualifier3")?.Value ?? "0"),
            Qualifier4 = int.Parse(element.Attribute("qualifier4")?.Value ?? "0"),
            Qualifier5 = int.Parse(element.Attribute("qualifier5")?.Value ?? "0"),
            Metres = int.Parse(element.Attribute("Metres")?.Value ?? "0"),
            PlayNum = int.Parse(element.Attribute("PlayNum")?.Value ?? "0"),
            SetNum = int.Parse(element.Attribute("SetNum")?.Value ?? "0"),
            SequenceId = int.Parse(element.Attribute("sequence_id")?.Value ?? "0"),
            PlayerAdvantage = int.Parse(element.Attribute("player_advantage")?.Value ?? "0"),
            ScoreAdvantage = int.Parse(element.Attribute("score_advantage")?.Value ?? "0"),
            Flag = bool.Parse(element.Attribute("flag")?.Value ?? "False"),
            Advantage = int.Parse(element.Attribute("advantage")?.Value ?? "0"),
            AssocPlayer = int.Parse(element.Attribute("assoc_player")?.Value ?? "0")
        };
    }
}
