using System.Text;
using System.Xml.Linq;

public class InStatParsingStrategy : IXmlParsingStrategy
{
    private readonly List<InstanceData> instances = new List<InstanceData>();
    private readonly RowsParser rowsParser = new RowsParser();
    private readonly ParseInstanceInStat  parseInstance = new ParseInstanceInStat();
private readonly Dictionary<string, HashSet<string>> teamPlayers = new Dictionary<string, HashSet<string>>();
    public List<InstanceData> Instances => instances;
    public List<RowData> Rows => rowsParser.Rows;

 public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
{
    switch (element.Name.LocalName.ToUpper())
    {
        case "INSTANCE":
            InstanceData instance = parseInstance.ParseInstanceInStat1(element);
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
        if (string.IsNullOrEmpty(instance.PlayerName) && instance.Team == null)
        {
            string outputLine = $"{instance.Code} start {instance.Start} end {instance.End}";
            xmlContentBuilder.AppendLine(outputLine);
        }
        else if (instance.Team != "N/A" && instance.Action != "N/A")
        {
            string outputLine = $"joueur {instance.PlayerNumber}. {instance.PlayerName} " +
                              $"start {instance.Start} end {instance.End} " +
                              $"team {instance.Team} Action {instance.Action} " +
                              $"Half {instance.Half} pos_x {instance.PosX} pos_y {instance.PosY}";
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
    foreach (var team in teamPlayers)
    {
        sb.Append($"Liste des joueurs équipe {team.Key} : {{");
        sb.Append(string.Join(", ", team.Value));
        sb.AppendLine("}");
    }
    return sb.ToString();
}
    
}
