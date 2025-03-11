using System.Text;
using System.Xml.Linq;
public class RowsParser : IElementParser
{
    public List<RowData> Rows { get; set; } = new List<RowData>();

    public void ParseRow(XElement element, StringBuilder xmlContentBuilder )
    {
        Rows.Clear();

        foreach (var row in element.Elements("row"))
        {
            RowData rowData = ParseRow(row);
            Rows.Add(rowData);
            AppendRowToBuilder(rowData, xmlContentBuilder);
        }
    }

    public RowData ParseRow(XElement row)
    {
        return new RowData
        {
            Code = row.Element("code")?.Value.Trim() ?? "Unknown",
            R = row.Element("R")?.Value.Trim() ?? "0",
            G = row.Element("G")?.Value.Trim() ?? "0",
            B = row.Element("B")?.Value.Trim() ?? "0",
            SortOrder = row.Element("sort_order")?.Value.Trim() ?? "N/A"
        };
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