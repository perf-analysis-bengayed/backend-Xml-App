using System.Text;
using System.Xml.Linq;

public class AllInstancesParser : IElementParser
{
    private readonly Func<string, bool> _isPlayerCode;
    private readonly Func<string, bool> _isTeamActionCode;

    public AllInstancesParser(Func<string, bool> isPlayerCode, Func<string, bool> isTeamActionCode)
    {
        _isPlayerCode = isPlayerCode;
        _isTeamActionCode = isTeamActionCode;
    }

    public void Parse(XElement element, StringBuilder xmlContentBuilder, InStatParsingStrategy context)
    {
        foreach (var instance in element.Elements("instance"))
        {
            string start = instance.Element("start")?.Value.Trim() ?? "N/A";
            string end = instance.Element("end")?.Value.Trim() ?? "N/A";
            string code = instance.Element("code")?.Value.Trim() ?? "Unknown";
            string posX = instance.Element("pos_x")?.Value.Trim() ?? "N/A";
            string posY = instance.Element("pos_y")?.Value.Trim() ?? "N/A";

            if (_isPlayerCode(code))
            {
                string[] codeParts = code.Split(new[] { '.' }, 2);
                string playerNumber = codeParts[0].Trim();
                string playerName = codeParts[1].Trim();

                if (!context.GetPlayerNames().Contains(playerName))
                {
                    context.GetPlayerNames().Add(playerName);
                }

                string team = "";
                string action = "N/A";
                string half = "N/A";
                foreach (var label in instance.Elements("label"))
                {
                    var group = label.Element("group")?.Value.Trim();
                    var text = label.Element("text")?.Value.Trim();
                    if (group == "Team")
                    {
                        team = text;
                        if (!string.IsNullOrWhiteSpace(team))
                        {
                            context.GetTeamNames().Add(team);
                        }
                    }
                    else if (group == "Action")
                    {
                        action = text;
                    }
                    else if (group == "Half")
                    {
                        half = text;
                    }
                }

                string outputLine = $"joueur {playerName} numero {playerNumber} Team {team} Action {action} start {start} end {end} et pos_x {posX} pos_y {posY} Half {half}";
                xmlContentBuilder.AppendLine(outputLine);
            }
            else
            {
                string team = "";
                string action = code;

                if (_isTeamActionCode(code))
                {
                    string[] codeParts = code.Split(new[] { ' ' }, 2);
                    team = codeParts[0].Trim();
                    action = codeParts.Length > 1 ? codeParts[1].Trim() : "Unknown Action";
                    if (!string.IsNullOrWhiteSpace(team))
                    {
                        context.GetTeamNames().Add(team);
                    }
                }

                xmlContentBuilder.AppendLine($"{team} {action} start {start} end {end}");
            }
        }
    }
}