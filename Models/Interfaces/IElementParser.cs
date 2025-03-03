using System.Text;
using System.Xml.Linq;

public interface IElementParser
{
    void Parse(XElement element, StringBuilder xmlContentBuilder, InStatParsingStrategy context);
}