using System.Text;
using System.Xml.Linq;

public interface IXmlParsingStrategyName
{
    MatchNameInfo ExtractMatchInfoFromFileName(string fileName);

 
}