using System.Text;
using System.Xml.Linq;

public class SportDataParsingStrategy : IXmlParsingStrategy
{
    private readonly List<InstanceData> instances = new List<InstanceData>();
    private readonly RowsParser rowsParser = new RowsParser();
    private readonly ParseInstanceSportData  parseInstanceSportData = new ParseInstanceSportData();

    public List<InstanceData> Instances => instances;
    public List<RowData> Rows => rowsParser.Rows;

    public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
    {
        switch (element.Name.LocalName.ToUpper())
        {
            case "INSTANCE":
                InstanceData instance = parseInstanceSportData.ParseInstanceSportData1(element);
                instances.Add(instance);
                AppendInstanceToBuilder(instance, xmlContentBuilder);
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
            string outputLine = !string.IsNullOrEmpty(instance.PlayerName) && !string.IsNullOrEmpty(instance.PlayerNumber)
                ? $"joueur test {instance.PlayerName} numero {instance.PlayerNumber} team {instance.Team} " +
                  $"action {instance.Action} half {instance.Half} pos_x={instance.PosX} pos_y={instance.PosY} " +
                  $"start {instance.Start} end {instance.End}"
                : $"teams {instance.Team} action {instance.Action} half {instance.Half} " +
                  $"pos_x={instance.PosX} pos_y={instance.PosY} start={instance.Start} end={instance.End}";

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






}