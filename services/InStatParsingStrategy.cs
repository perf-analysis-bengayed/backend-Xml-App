using System.Text;
using System.Xml.Linq;


public class InStatParsingStrategy : IXmlParsingStrategy
{
    private readonly List<string> playerNames = new List<string>();
    private readonly List<string> outputLines = new List<string>();
    private readonly HashSet<string> teamNames = new HashSet<string>();

    private readonly Dictionary<string, IElementParser> parsers;

    public InStatParsingStrategy()
    {
        parsers = new Dictionary<string, IElementParser>(StringComparer.OrdinalIgnoreCase)
        {
            { "ALL_INSTANCES", new AllInstancesParser(IsPlayerCode, IsTeamActionCode) },
            { "ROWS", new RowsParser() },
            { "SORT_INFO", new SortInfoParser() }
        };
    }


    public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
    {
        string elementName = element.Name.LocalName.ToUpper();
        if (parsers.TryGetValue(elementName, out var parser))
        {
            parser.Parse(element, xmlContentBuilder, this);
        }
    }

    private bool IsPlayerCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            return false;

        string[] parts = code.Split(new[] { '.' }, 2);
        if (parts.Length != 2)
            return false;

        return int.TryParse(parts[0].Trim(), out _) && !string.IsNullOrWhiteSpace(parts[1].Trim());
    }

    private bool IsTeamActionCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            return false;

        string[] parts = code.Split(new[] { ' ' }, 2);
        return parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[0]) && !string.IsNullOrWhiteSpace(parts[1]);
    }

    public void AppendFinalPlayerNames(StringBuilder xmlContentBuilder)
    {
        foreach (var line in outputLines)
        {
            xmlContentBuilder.AppendLine(line);
        }

        if (playerNames.Any())
        {
            var playerCodes = playerNames.Select((name, index) => $"{name}").ToList();
            xmlContentBuilder.AppendLine($"Liste des Joueurs: {string.Join(", ", playerCodes)}");
        }

        if (teamNames.Any())
        {
            xmlContentBuilder.AppendLine($"Équipe de match :{string.Join(" et ", teamNames)}");
        }
    }

    public List<string> GetPlayerNames()
    {
        return playerNames;
    }

    public List<string> GetTeamNames()
    {
        return new List<string>(teamNames);
    }
    
}