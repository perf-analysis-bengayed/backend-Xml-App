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

    var versionElement = xmlDocument.Descendants("VERSION").FirstOrDefault();
    if (versionElement != null && versionElement.Value.Contains("WYSCOUT", StringComparison.OrdinalIgnoreCase))
    {
        return new WyscoutParsingStrategy();
    }

    var allInstancesElement = xmlDocument.Descendants("ALL_INSTANCES").FirstOrDefault();
    bool hasStructuralElements = allInstancesElement != null || 
                                 xmlDocument.Descendants("ROWS").Any() || 
                                 xmlDocument.Descendants("SORT_INFO").Any();

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