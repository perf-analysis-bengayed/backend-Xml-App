using System.Text;
using System.Xml.Linq;

public class SortInfoParser : IElementParser
{
    public string? SortType { get; set; }

    public void Parse(XElement element, StringBuilder xmlContentBuilder, InStatParsingStrategy context)
    {
        SortType = element.Element("sort_type")?.Value.Trim() ?? "N/A";
        xmlContentBuilder.AppendLine($"{SortType}");
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(SortType))
        {
            return "No sort type information available";
        }
        
        return $"Sort Type: {SortType}";
    }
}