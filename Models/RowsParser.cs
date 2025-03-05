using System.Text;
using System.Xml.Linq;

public class RowsParser : IElementParser
{
    
    public List<RowData> Rows { get; set; } = new List<RowData>();

   
   
    public void Parse(XElement element, StringBuilder xmlContentBuilder, InStatParsingStrategy context)
    {
        Rows.Clear();

        foreach (var row in element.Elements("row"))
        {
          
            var rowData = new RowData
            {
                Code = row.Element("code")?.Value.Trim() ?? "Unknown",
                R = row.Element("R")?.Value.Trim() ?? "0",
                G = row.Element("G")?.Value.Trim() ?? "0",
                B = row.Element("B")?.Value.Trim() ?? "0"
            };
            Rows.Add(rowData);

            xmlContentBuilder.AppendLine($"{rowData.Code} R {rowData.R}, G {rowData.G}, B {rowData.B}");
        }
    }


    
    
}