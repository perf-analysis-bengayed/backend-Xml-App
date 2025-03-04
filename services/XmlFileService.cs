using System.Text;

using System.Xml.Linq;

public class XmlFileService : IXmlFileService
{
    private readonly string _uploadPath;

private readonly IWebHostEnvironment _environment; 

    public XmlFileService(IWebHostEnvironment environment)
    {
        _environment = environment; 
        _uploadPath = Path.Combine(environment.ContentRootPath, "uploads");

        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
    {
        throw new NotImplementedException();
    }

    public async Task<List<string>> UploadXmlFilesAsync(List<IFormFile> files, MatchInfo matchInfo)
    {
        
        if (files == null || files.Count == 0)
        {
            throw new ArgumentException("Aucun fichier n'a été téléchargé.");
        }
        List<string> filePaths = new List<string>();
        List<string> parsedContents = new List<string>();
        
        foreach (var file in files)
        {
            if (Path.GetExtension(file.FileName).ToLower() != ".xml")
            {
                throw new ArgumentException($"Le fichier {file.FileName} doit être de type .xml.");
            }

            var filePath = Path.Combine(_uploadPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            IXmlParsingStrategy strategy = XmlParser.DetermineParsingStrategy(filePath);
            XmlParser parser = new XmlParser(strategy, _environment);
            string parsedContent = parser.ParseXmlFile(filePath);
            parsedContents.Add(parsedContent);
            

        }

        var matchFilePath = Path.Combine(_uploadPath, "ficheMatch.xml");
        filePaths.Add(matchFilePath);
        CreateMatchFile(matchFilePath, parsedContents, matchInfo);
  
  
    _ = Task.Run(async () =>
    {
        await Task.Delay(TimeSpan.FromMinutes(5)); 
        foreach (var path in filePaths)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Console.WriteLine($"Fichier supprimé : {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression de {path} : {ex.Message}");
            }
        }
    });

    return parsedContents;
    }

 


    private void CreateMatchFile(string filePath, List<string> parsedContents, MatchInfo matchInfo)
    {
        var matchElement = new XElement("Match",
            new XElement("MatchDate", matchInfo.MatchDate.ToString("yyyy-MM-dd")),
            new XElement("ParsedContents", parsedContents)
            
        );

        var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), matchElement);
        document.Save(filePath);
    }






}
