using System.Text;
using System.Xml.Linq;

public class XmlParser : XmlFileService
{
    private readonly IXmlParsingStrategy _parsingStrategy;

    public XmlParser(IXmlParsingStrategy parsingStrategy, IWebHostEnvironment environment) 
        : base(environment) 
    {
        _parsingStrategy = parsingStrategy;
    }

    public string ParseXmlFile(string filePath)
    {
        StringBuilder xmlContentBuilder = new StringBuilder();
        try
        {
            XDocument xmlDocument = XDocument.Load(filePath);
            
            foreach (var element in xmlDocument.Descendants())
            {
                if (!string.IsNullOrWhiteSpace(element.Value.Trim()))
                {
                    _parsingStrategy.ParseElement(element, xmlContentBuilder);
                }
            }


            if (xmlContentBuilder.Length == 0)
            {
                xmlContentBuilder.AppendLine("Aucun élément avec contenu trouvé dans le fichier XML.");
            }
        }
        catch (Exception ex)
        {
            return $"Erreur lors du traitement du fichier XML: {ex.Message}";
        }

        return xmlContentBuilder.ToString();
    }

 public static IXmlParsingStrategy DetermineParsingStrategy(string filePath)
    {
        XDocument xmlDocument = XDocument.Load(filePath);

        // Check for OPTAFEED (Rugby) format first
        var optafeedElement = xmlDocument.Descendants("OPTAFEED").FirstOrDefault();
        if (optafeedElement != null)
        {
            bool hasRugbyElements = xmlDocument.Descendants("ActionRow").Any() ||
                                  xmlDocument.Descendants("MatchData").Any();
            if (hasRugbyElements)
            {
                return new RugbyParsingStrategy();
            }
        }

        var gameElement = xmlDocument.Descendants("GameFile").FirstOrDefault() ?? 
                         xmlDocument.Descendants("Game").FirstOrDefault();
        
        if (gameElement != null)
        {
            bool isRugby = xmlDocument.Descendants("CompetitionName")
                            .Any(x => x.Value.Contains("TOP 14", StringComparison.OrdinalIgnoreCase)) ||
                          xmlDocument.Descendants("NumberOfRunOnPlayers").Any() ||
                          xmlDocument.Descendants("RefereeInformation").Any();
            
            if (isRugby)
            {
                return new RugbyParsingStrategy();
            }
        }

        return DetermineFootballParsingStrategy(xmlDocument);
    }

private static IXmlParsingStrategy DetermineFootballParsingStrategy(XDocument xmlDocument)
{
 
    var versionElement = xmlDocument.Descendants("VERSION").FirstOrDefault();
    if (versionElement != null && versionElement.Value.Contains("WYSCOUT", StringComparison.OrdinalIgnoreCase))
    {
        return new WyscoutParsingStrategy();
    }

    var allInstancesElement = xmlDocument.Descendants("ALL_INSTANCES").FirstOrDefault();
    bool hasStructuralElements = allInstancesElement != null || 
                                xmlDocument.Descendants("ROWS").Any() || 
                                xmlDocument.Descendants("SORT_INFO").Any() ||
                                xmlDocument.Descendants("OPTAFEED").Any();

    if (hasStructuralElements)
    {
        if (allInstancesElement != null)
        {
            var instances = allInstancesElement.Elements("instance");
            bool hasLabelPos = instances.Any(i => i.Elements("label").Any(l => 
                l.Element("group")?.Value == "pos_x" || l.Element("group")?.Value == "pos_y"));

            if (hasLabelPos)
            {
                return new SportDataParsingStrategy();
            }
        }
        return new InStatParsingStrategy();
    }

    throw new InvalidOperationException("Type non valide");
}

}