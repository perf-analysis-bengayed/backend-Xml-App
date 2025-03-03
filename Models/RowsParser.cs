using System.Text;
using System.Xml.Linq;

public class RowsParser : IElementParser
{


    
    public void Parse(XElement element, StringBuilder xmlContentBuilder, InStatParsingStrategy context)
    {
        foreach (var row in element.Elements("row"))
        {
            string code = row.Element("code")?.Value.Trim() ?? "Unknown";
            string r = row.Element("R")?.Value.Trim() ?? "0";
            string g = row.Element("G")?.Value.Trim() ?? "0";
            string b = row.Element("B")?.Value.Trim() ?? "0";

            xmlContentBuilder.AppendLine($"{code} R {r}, G {g}, B {b}");
        }
    }
}