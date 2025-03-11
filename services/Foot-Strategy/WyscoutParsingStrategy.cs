

using System.Text;
using System.Xml.Linq;

public class WyscoutParsingStrategy : IXmlParsingStrategy
{
    public void AppendFinalPlayerNames(StringBuilder xmlContentBuilder)
    {
        throw new NotImplementedException();
    }

    public List<string> GetPlayerNames()
    {
        throw new NotImplementedException();
    }

    public List<string> GetTeamNames()
    {
        throw new NotImplementedException();
    }

    public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
    {
        switch (element.Value.Trim())
        {
            case "Aerial duels":
                xmlContentBuilder.AppendLine($"{element.Name}/Aerial duels");
                xmlContentBuilder.AppendLine($"{element.Name}/Duel");
                xmlContentBuilder.AppendLine($"{element.Name}/Aérien");
                break;
            case "Aggressiveness":
                xmlContentBuilder.AppendLine("Aerial Aggressiveness1 /{element.Value}");
                xmlContentBuilder.AppendLine("Aerial Aggressiveness2 /{element.Value}");
                break;
            default:
                xmlContentBuilder.AppendLine($"{element.Name} /{element.Value}");
                break;
        }
    }
}
