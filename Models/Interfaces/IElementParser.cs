using System.Text;
using System.Xml.Linq;

public interface IElementParser
{
    void ParseRow(XElement element, StringBuilder xmlContentBuilder);
}