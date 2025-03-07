using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.IO; // Ensure this is included for StreamWriter

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

    public async Task<List<string>> UploadXmlFilesAsync(List<IFormFile> files, MatchInfo matchInfo)
    {
        if (files == null || files.Count == 0)
        {
            throw new ArgumentException("Aucun fichier n'a été téléchargé.");
        }

        List<string> filePaths = new List<string>();
        List<string> parsedContents = new List<string>();

        MatchNameInfo extractedMatchNameInfo = null;

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

            IXmlParsingStrategy xmlStrategy = XmlParser.DetermineParsingStrategy(filePath);
            IFileNameParsingStrategy nameStrategy = DetermineFileNameParsingStrategy(xmlStrategy);
            extractedMatchNameInfo = nameStrategy.ParseFileName(file.FileName);

            XmlParser parser = new XmlParser(xmlStrategy, _environment);
            string parsedContent = parser.ParseXmlFile(filePath);
            parsedContents.Add(parsedContent);

            filePaths.Add(filePath);
        }

        var matchFilePath = Path.Combine(_uploadPath, "ficheMatch.txt"); // Changed to .txt
        filePaths.Add(matchFilePath);

        var finalMatchInfo = new MatchInfo
        {
            MatchDate = extractedMatchNameInfo?.MatchDate ?? matchInfo.MatchDate,
            HomeTeam = extractedMatchNameInfo?.HomeTeam ?? matchInfo.HomeTeam,
            AwayTeam = extractedMatchNameInfo?.AwayTeam ?? matchInfo.AwayTeam,
            ParsedContents = parsedContents
        };

        CreateMatchFile(matchFilePath, finalMatchInfo);

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

    public IFileNameParsingStrategy DetermineFileNameParsingStrategy(IXmlParsingStrategy xmlStrategy)
    {
        return xmlStrategy switch
        {
            WyscoutParsingStrategy => new WyscoutParsingName(),
            SportDataParsingStrategy => new SportDataParsingName(),
            InStatParsingStrategy => new InStatParsingName(),
            _ => throw new ArgumentException("Stratégie de parsing XML non reconnue.")
        };
    }

    private void CreateMatchFile(string filePath, MatchInfo matchInfo)
    {
        // Writing to a .txt file instead of XML
        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            writer.WriteLine("Match Information:");
            writer.WriteLine($"Match Date: {matchInfo.MatchDate.ToString("dd.MM.yyyy")}");
            writer.WriteLine($"Home Team: {matchInfo.HomeTeam}");
            writer.WriteLine($"Away Team: {matchInfo.AwayTeam}");
            writer.WriteLine("Parsed Contents:");
            foreach (var content in matchInfo.ParsedContents)
            {
                writer.WriteLine(content);
            }
        }
    }
}