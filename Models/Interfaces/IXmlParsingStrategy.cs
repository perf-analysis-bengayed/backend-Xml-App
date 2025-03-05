using System.Text;
using System.Xml.Linq;

public interface IXmlParsingStrategy
{
    void ParseElement(XElement element, StringBuilder xmlContentBuilder);
  
}
