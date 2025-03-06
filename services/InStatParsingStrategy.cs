using System.Text;
using System.Xml.Linq;

public class InStatParsingStrategy : IXmlParsingStrategy
{
    private readonly List<InstanceData> instances = new List<InstanceData>();
    private readonly RowsParser rowsParser = new RowsParser();
    private readonly ParseInstanceInStat  parseInstance = new ParseInstanceInStat();

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
}
