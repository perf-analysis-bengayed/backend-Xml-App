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
                Code = instance.Element("code")?.Value.Trim() ?? "Unknown",
                PosX = instance.Element("pos_x")?.Value.Trim() ?? "N/A",
                PosY = instance.Element("pos_y")?.Value.Trim() ?? "N/A"
            };

            string team = "N/A";
            string action = "N/A";
            string half = "N/A";
            var labelsOutput = new StringBuilder();

            foreach (var label in instance.Elements("label"))
            {
                var labelData = new Label
                {
                    Group = label.Element("group")?.Value.Trim(),
                    Text = label.Element("text")?.Value.Trim()
                };
                instanceData.Labels.Add(labelData);

                if (!string.IsNullOrWhiteSpace(labelData.Group) && !string.IsNullOrWhiteSpace(labelData.Text))
                {
                    labelsOutput.Append($"{labelData.Group} {labelData.Text} ");
                }

                if (labelData.Group == "Team")
                {
                    team = labelData.Text;
                    instanceData.Team = team;
                    if (!string.IsNullOrWhiteSpace(team))
                    {
                        context.GetTeamNames().Add(team);
                    }
                }
                else if (labelData.Group == "Action")
                {
                    action = labelData.Text;
                    instanceData.Action = action;
                }
                else if (labelData.Group == "Half")
                {
                    half = labelData.Text;
                    instanceData.Half = half;
                }
                else if (labelData.Group == "pos_x")
                {
                    instanceData.PosX = labelData.Text;
                }
                else if (labelData.Group == "pos_y")
                {
                    instanceData.PosY = labelData.Text;
                }
            }

            Instances.Add(instanceData);

            if (_isPlayerCode(instanceData.Code))
            {
                string playerNumber = "N/A";
                string playerName = "Unknown";

                string[] codeParts = instanceData.Code.Split(new[] { '.' }, 2);
                if (codeParts.Length == 2)
                {
                    playerNumber = codeParts[0].Trim();
                    playerName = codeParts[1].Trim();

                    if (playerName.Contains("("))
                    {
                        int idStartIndex = playerName.IndexOf("(");
                        int actionStartIndex = playerName.IndexOf(" - ");
                        if (actionStartIndex > idStartIndex)
                        {
                            playerName = playerName.Substring(0, idStartIndex).Trim();
                            if (action == "N/A" && actionStartIndex != -1)
                            {
                                action = playerName.Substring(actionStartIndex + 3).Trim();
                            }
                        }
                    }
                }

                if (!context.GetPlayerNames().Contains(playerName))
                {
                    context.GetPlayerNames().Add(playerName);
                }

                string outputLine = $"joueur {playerName} numero {playerNumber} Team {team} Action {action} start {instanceData.Start} end {instanceData.End} et pos_x {instanceData.PosX} pos_y {instanceData.PosY} Half {half}";
                xmlContentBuilder.AppendLine(outputLine);
            }
            else
            {
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

                string outputLine = $"{team} {action} start {instanceData.Start} end {instanceData.End} {labelsOutput.ToString().Trim()}";
                xmlContentBuilder.AppendLine(outputLine);
            }
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("AllInstancesParser:");
        sb.AppendLine($"Total Instances: {Instances.Count}");

        if (Instances.Count == 0)
        {
            sb.AppendLine("No instances available.");
            return sb.ToString();
        }

        foreach (var instance in Instances)
        {
            sb.AppendLine("---");

            if (_isPlayerCode(instance.Code))
            {
                string playerNumber = "N/A";
                string playerName = "Unknown";
                string action = instance.Action;

                string[] codeParts = instance.Code.Split(new[] { '.' }, 2);
                if (codeParts.Length == 2)
                {
                    playerNumber = codeParts[0].Trim();
                    playerName = codeParts[1].Trim();

                    if (playerName.Contains("("))
                    {
                        int idStartIndex = playerName.IndexOf("(");
                        int actionStartIndex = playerName.IndexOf(" - ");
                        if (actionStartIndex > idStartIndex)
                        {
                            playerName = playerName.Substring(0, idStartIndex).Trim();
                            if (action == "N/A" && actionStartIndex != -1)
                            {
                                action = playerName.Substring(actionStartIndex + 3).Trim();
                            }
                        }
                    }
                }

                sb.AppendLine($"joueur {playerName} numero {playerNumber} Team {instance.Team} Action {action} start {instance.Start} end {instance.End} et pos_x {instance.PosX} pos_y {instance.PosY} Half {instance.Half}");
            }
            else
            {
                string team = "";
                string action = instance.Code;
                var labelsOutput = new StringBuilder();

                foreach (var label in instance.Labels)
                {
                    if (!string.IsNullOrWhiteSpace(label.Group) && !string.IsNullOrWhiteSpace(label.Text))
                    {
                        labelsOutput.Append($"{label.Group} {label.Text} ");
                    }
                }

                if (_isTeamActionCode(instance.Code))
                {
                    string[] codeParts = instance.Code.Split(new[] { " - " }, 2, StringSplitOptions.None);
                    team = codeParts[0].Trim();
                    action = codeParts.Length > 1 ? codeParts[1].Trim() : "Unknown Action";
                }

                sb.AppendLine($"{team} {action} start {instance.Start} end {instance.End} {labelsOutput.ToString().Trim()}");
            }
        }

        return sb.ToString();
    }
}