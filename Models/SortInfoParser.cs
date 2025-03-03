using System.Text;
using System.Xml.Linq;

public class SortInfoParser : IElementParser
{
    public void Parse(XElement element, StringBuilder xmlContentBuilder, InStatParsingStrategy context)
    {
        string sortType = element.Element("sort_type")?.Value.Trim() ?? "N/A";
        xmlContentBuilder.AppendLine($"{sortType}");
    }

   
}

