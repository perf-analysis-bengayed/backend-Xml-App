using System.Text;
using System.Xml.Linq;
using System.Globalization;
public class ParseActionRow
{
    public ActionRow ParseActionRowElement(XElement element)
    {
        return new ActionRow
        {
            ID = TryParseInt(element.Attribute("ID")?.Value, 0),
            FXID = element.Attribute("FXID")?.Value ?? "",
            PLID = TryParseInt(element.Attribute("PLID")?.Value, 0),
            TeamId = TryParseInt(element.Attribute("team_id")?.Value, 0),
            PsTimestamp = TryParseDouble(element.Attribute("ps_timestamp")?.Value, 0.0),
            PsEndstamp = TryParseDouble(element.Attribute("ps_endstamp")?.Value, 0.0),
            MatchTime = TryParseInt(element.Attribute("MatchTime")?.Value, 0),
            PsID = TryParseInt(element.Attribute("psID")?.Value, 0),
            Period = TryParseInt(element.Attribute("period")?.Value, 0),
            XCoord = TryParseInt(element.Attribute("x_coord")?.Value, 0),
            YCoord = TryParseInt(element.Attribute("y_coord")?.Value, 0),
            XCoordEnd = TryParseInt(element.Attribute("x_coord_end")?.Value, 0),
            YCoordEnd = TryParseInt(element.Attribute("y_coord_end")?.Value, 0),
            Action = TryParseInt(element.Attribute("action")?.Value, 0),
            ActionType = TryParseInt(element.Attribute("ActionType")?.Value, 0),
            ActionResult = TryParseInt(element.Attribute("Actionresult")?.Value, 0),
            Qualifier3 = TryParseInt(element.Attribute("qualifier3")?.Value, 0),
            Qualifier4 = TryParseInt(element.Attribute("qualifier4")?.Value, 0),
            Qualifier5 = TryParseInt(element.Attribute("qualifier5")?.Value, 0),
            Metres = TryParseInt(element.Attribute("Metres")?.Value, 0),
            PlayNum = TryParseInt(element.Attribute("PlayNum")?.Value, 0),
            SetNum = TryParseInt(element.Attribute("SetNum")?.Value, 0),
            SequenceId = TryParseInt(element.Attribute("sequence_id")?.Value, 0),
            PlayerAdvantage = TryParseInt(element.Attribute("player_advantage")?.Value, 0),
            ScoreAdvantage = TryParseInt(element.Attribute("score_advantage")?.Value, 0),
            Flag = bool.TryParse(element.Attribute("flag")?.Value, out bool flag) ? flag : false,
            Advantage = TryParseInt(element.Attribute("advantage")?.Value, 0),
            AssocPlayer = TryParseInt(element.Attribute("assoc_player")?.Value, 0)
        };
    }

    private int TryParseInt(string value, int defaultValue)
    {
        if (string.IsNullOrEmpty(value)) return defaultValue;
        if (value.Contains("+"))
        {
            var parts = value.Split('+');
            int sum = 0;
            foreach (var part in parts)
            {
                if (int.TryParse(part, NumberStyles.Any, CultureInfo.InvariantCulture, out int num))
                    sum += num;
            }
            return sum > 0 ? sum : defaultValue;
        }
        return int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int parsed) ? parsed : defaultValue;
    }

    private double TryParseDouble(string value, double defaultValue)
    {
        if (string.IsNullOrEmpty(value)) return defaultValue;
        if (value.Contains("+"))
        {
            string firstPart = value.Split('+')[0];
            return double.TryParse(firstPart, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : defaultValue;
        }
        return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed) ? parsed : defaultValue;
    }
}