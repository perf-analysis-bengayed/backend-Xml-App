using System.Text;
using System.Xml.Linq;

public class SportDataParsingStrategy : IXmlParsingStrategy
{
    private readonly List<InstanceData> instances = new List<InstanceData>();
    private readonly RowsParser rowsParser = new RowsParser();
    private readonly ParseInstanceSportData parseInstanceSportData = new ParseInstanceSportData();
    private readonly Dictionary<string, HashSet<string>> teamPlayers = new Dictionary<string, HashSet<string>>();
    
    public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
    {
        switch (element.Name.LocalName.ToUpper())
        {
            case "INSTANCE":
                InstanceData instance = parseInstanceSportData.ParseInstanceSportData1(element);
                instances.Add(instance);
                AppendInstanceToBuilder(instance, xmlContentBuilder);
                
                if (!string.IsNullOrEmpty(instance.Team) && instance.Team != "N/A" && 
                    !string.IsNullOrEmpty(instance.PlayerName))
                {
                    if (!teamPlayers.ContainsKey(instance.Team))
                    {
                        teamPlayers[instance.Team] = new HashSet<string>();
                    }
                    teamPlayers[instance.Team].Add($"{instance.PlayerNumber}. {instance.PlayerName}");
                }
                break;

            case "ROW":
                RowData row = rowsParser.ParseRow(element);
                rowsParser.Rows.Add(row);
                AppendRowToBuilder(row, xmlContentBuilder);
                break;
        }
    }

    private void AppendInstanceToBuilder(InstanceData instance, StringBuilder xmlContentBuilder)
    {
        if (instance.Team != "N/A" && instance.Action != "N/A")
        {
           
            bool isSpecificFormat = !string.IsNullOrEmpty(instance.PlayerName) && 
                                   !string.IsNullOrEmpty(instance.PlayerNumber) && 
                                   instance.Code.Contains(" - ") && 
                                   instance.Code.Contains("(") && instance.Code.Contains(")");

            string outputLine;
            if (isSpecificFormat)
            {
               
                outputLine = $"joueur {instance.PlayerNumber}. {instance.PlayerName} team {instance.Team} " +
                             $"action {instance.Action} half {instance.Half} " +
                             $"pos_x={instance.PosX} pos_y={instance.PosY} " +
                             $"start {instance.Start} end {instance.End}";
            }
            else
            {
                outputLine = !string.IsNullOrEmpty(instance.PlayerName) && !string.IsNullOrEmpty(instance.PlayerNumber)
                    ? $"joueur test {instance.PlayerName} numero {instance.PlayerNumber} team {instance.Team} " +
                      $"action {instance.Action} half {instance.Half} pos_x={instance.PosX} pos_y={instance.PosY} " +
                      $"start {instance.Start} end {instance.End}"
                    : $"teams {instance.Team} action {instance.Action} half {instance.Half} " +
                      $"pos_x={instance.PosX} pos_y={instance.PosY} start={instance.Start} end={instance.End}";
            }

            xmlContentBuilder.AppendLine(outputLine);
        }
    }

    private void AppendRowToBuilder(RowData row, StringBuilder xmlContentBuilder)
    {
        if (row.Code != "Unknown")
        {
            string outputLine = $"{row.Code} R {row.R}, G {row.G}, B {row.B}, SortOrder {row.SortOrder}";
            xmlContentBuilder.AppendLine(outputLine);
        }
    }

    public string GetTeamPlayersList()

    {
        StringBuilder sb = new StringBuilder();
          if (teamPlayers.Count == 0)
        {
            sb.AppendLine("Aucune équipe ou joueur détecté dans les données InStat.");
        }
        else
        {
        foreach (var team in teamPlayers)
        {
            sb.Append($"Liste des joueurs équipe {team.Key} : {{");
            sb.Append(string.Join(", ", team.Value));
            sb.AppendLine("}");
        }
     
    }
       return sb.ToString();
}}