using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;

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

            if (_parsingStrategy is InStatParsingStrategy inStatStrategy)
            {
                inStatStrategy.AppendFinalPlayerNames(xmlContentBuilder);
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

        if (xmlDocument.Descendants("ALL_INSTANCES").Any() || 
            xmlDocument.Descendants("ROWS").Any() || 
            xmlDocument.Descendants("SORT_INFO").Any())
        {
            return new InStatParsingStrategy();
        }

        return new InStatParsingStrategy();
    }
}