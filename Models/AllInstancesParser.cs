using System.Text;
using System.Xml.Linq;

public class AllInstancesParser : IElementParser
{
    private readonly Func<string, bool> _isPlayerCode;
    private readonly Func<string, bool> _isTeamActionCode;
    
    public List<InstanceData> Instances { get; set; } = new List<InstanceData>();

    public AllInstancesParser(Func<string, bool> isPlayerCode, Func<string, bool> isTeamActionCode)
    {
        _isPlayerCode = isPlayerCode;
        _isTeamActionCode = isTeamActionCode;
    }

    public void Parse(XElement element, StringBuilder xmlContentBuilder, InStatParsingStrategy context)
    {
        Instances.Clear();

        foreach (var instance in element.Elements("instance"))
        {
            var instanceData = new InstanceData
            {
                Start = instance.Element("start")?.Value.Trim() ?? "N/A",
                End = instance.Element("end")?.Value.Trim() ?? "N/A",
                Code = instance.Element("code")?.Value.Trim() ?? ""
            };

            // Parse labels (from ParseInstance)
            instanceData.Labels = instance.Elements("label").Select(l => new Label
            {
                Group = l.Element("group")?.Value.Trim(),
                Text = l.Element("text")?.Value.Trim()
            }).ToList();

            // Process labels (from ParseInstance)
            foreach (var label in instanceData.Labels)
            {
                switch (label.Group)
                {
                    case "Team": instanceData.Team = label.Text ?? "N/A"; break;
                    case "Action": instanceData.Action = label.Text ?? "N/A"; break;
                    case "Half": instanceData.Half = label.Text ?? "N/A"; break;
                    case "pos_x": instanceData.PosX = label.Text ?? "N/A"; break;
                    case "pos_y": instanceData.PosY = label.Text ?? "N/A"; break;
                }
            }

          
            if (!string.IsNullOrEmpty(instanceData.Code))
            {
                string[] codeParts = instanceData.Code.Split(new[] { '.' }, 2);
                if (codeParts.Length == 2)
                {
                    instanceData.PlayerNumber = codeParts[0].Trim();
                    string namePart = codeParts[1].Trim();
                    
                    int actionIndex = namePart.IndexOf(" - ");
                    int parenIndex = namePart.IndexOf("(");
                    if (parenIndex > 0)
                    {
                        instanceData.PlayerName = namePart.Substring(0, parenIndex).Trim();
                        if (actionIndex > parenIndex && instanceData.Action == "N/A")
                        {
                            instanceData.Action = namePart.Substring(actionIndex + 3).Trim();
                        }
                    }
                    else if (actionIndex > 0)
                    {
                        instanceData.PlayerName = namePart.Substring(0, actionIndex).Trim();
                        if (instanceData.Action == "N/A")
                        {
                            instanceData.Action = namePart.Substring(actionIndex + 3).Trim();
                        }
                    }
                    else
                    {
                        instanceData.PlayerName = namePart;
                    }
                }
            }

            Instances.Add(instanceData);

          
            if (_isPlayerCode(instanceData.Code))
            {
                string playerNumber = instanceData.PlayerNumber ?? "N/A";
                string playerName = instanceData.PlayerName ?? "Unknown";

                if (!context.GetPlayerNames().Contains(playerName))
                {
                    context.GetPlayerNames().Add(playerName);
                }

                if (!string.IsNullOrWhiteSpace(instanceData.Team))
                {
                    context.GetTeamNames().Add(instanceData.Team);
                }

                string outputLine = $"joueur {playerName} numero {playerNumber} Team {instanceData.Team} Action {instanceData.Action} start {instanceData.Start} end {instanceData.End} et pos_x {instanceData.PosX} pos_y {instanceData.PosY} Half {instanceData.Half}";
                xmlContentBuilder.AppendLine(outputLine);
            }
            else
            {
                string team = instanceData.Team;
                string action = instanceData.Action;

                if (_isTeamActionCode(instanceData.Code))
                {
                    string[] codeParts = instanceData.Code.Split(new[] { " - " }, 2, StringSplitOptions.None);
                    team = codeParts[0].Trim();
                    action = codeParts.Length > 1 ? codeParts[1].Trim() : "Unknown Action";
                    if (!string.IsNullOrWhiteSpace(team))
                    {
                        context.GetTeamNames().Add(team);
                    }
                }
                else
                {
                    team = "";
                    action = instanceData.Code;
                }

                var labelsOutput = new StringBuilder();
                foreach (var label in instanceData.Labels)
                {
                    if (!string.IsNullOrWhiteSpace(label.Group) && !string.IsNullOrWhiteSpace(label.Text))
                    {
                        labelsOutput.Append($"{label.Group} {label.Text} ");
                    }
                }

                string outputLine = $"{team} {action} start {instanceData.Start} end {instanceData.End} {labelsOutput.ToString().Trim()}";
                xmlContentBuilder.AppendLine(outputLine);
            }
        }
    }
}