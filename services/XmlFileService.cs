using System.Text;

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

    if (files.Count > 5)
    {
        throw new ArgumentException("Le nombre maximum de fichiers autorisés est de 5.");
    }

    List<string> filePaths = new List<string>();
    List<string> parsedContents = new List<string>();
    IXmlParsingStrategy? xmlStrategy = null;
    MatchNameInfo? extractedMatchNameInfo = null;

    // Étape 1 : Déterminer la stratégie une seule fois avant la boucle
    if (files.Any())
    {
        var firstFilePath = Path.Combine(_uploadPath, files[0].FileName);
        // Télécharger le premier fichier pour déterminer la stratégie
        using (var stream = new FileStream(firstFilePath, FileMode.Create))
        {
            await files[0].CopyToAsync(stream);
        }
        xmlStrategy = XmlParser.DetermineParsingStrategy(firstFilePath);
        filePaths.Add(firstFilePath);
    }

    if (xmlStrategy == null)
    {
        throw new ArgumentException("Impossible de déterminer la stratégie de parsing.");
    }

    // Créer une instance unique de XmlParser avec cette stratégie
    XmlParser parser = new XmlParser(xmlStrategy, _environment);

    // Étape 2 : Traiter tous les fichiers avec la même stratégie
    for (int i = 1; i < files.Count; i++) // Commencer à 1 car le premier fichier est déjà traité
    {
        var file = files[i];
        if (Path.GetExtension(file.FileName).ToLower() != ".xml")
        {
            throw new ArgumentException($"Le fichier {file.FileName} doit être de type .xml.");
        }

        var filePath = Path.Combine(_uploadPath, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Parser le fichier avec la même instance de stratégie
        string parsedContent = parser.ParseXmlFile(filePath);
        parsedContents.Add(parsedContent);
        filePaths.Add(filePath);
    }

    // Parser le premier fichier après avoir initialisé la stratégie
    string firstParsedContent = parser.ParseXmlFile(filePaths[0]);
    parsedContents.Add(firstParsedContent);

    // Déterminer les informations du match (si applicable)
    if (xmlStrategy is RugbyParsingStrategy)
    {
        extractedMatchNameInfo = null;
    }
    else
    {
        IFileNameParsingStrategy? nameStrategy = DetermineFileNameParsingStrategy(xmlStrategy);
        extractedMatchNameInfo = nameStrategy.ParseFileName(files[0].FileName);
    }

    var matchFilePath = Path.Combine(_uploadPath, "ficheMatch.txt");
    filePaths.Add(matchFilePath);

    MatchInfo? finalMatchInfo = null;
    if (xmlStrategy is RugbyParsingStrategy)
    {
        finalMatchInfo = null;
    }
    else
    {
        finalMatchInfo = new MatchInfo
        {
            MatchDate = extractedMatchNameInfo?.MatchDate ?? matchInfo.MatchDate,
            HomeTeam = extractedMatchNameInfo?.HomeTeam ?? matchInfo.HomeTeam,
            AwayTeam = extractedMatchNameInfo?.AwayTeam ?? matchInfo.AwayTeam,
            ParsedContents = parsedContents
        };
    }

    // Étape 3 : Créer le fichier de match avec la liste complète des joueurs
    CreateMatchFile(matchFilePath, finalMatchInfo, xmlStrategy, parsedContents);

    // Suppression différée des fichiers
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

    public IFileNameParsingStrategy? DetermineFileNameParsingStrategy(IXmlParsingStrategy xmlStrategy)
    {
        return xmlStrategy switch
        {
            WyscoutParsingStrategy => new WyscoutParsingName(),
            SportDataParsingStrategy => new SportDataParsingName(),
            InStatParsingStrategy => new InStatParsingName(),
            RugbyParsingStrategy => null,
            _ => throw new ArgumentException("Stratégie de parsing XML non reconnue.")
        };
    }

  private void CreateMatchFile(string filePath, MatchInfo matchInfo, IXmlParsingStrategy xmlStrategy, List<string> parsedContents)
    {
        using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            if (matchInfo != null)
            {
                writer.WriteLine("Match Information:");
                writer.WriteLine($"Match Date: {matchInfo.MatchDate.ToString("dd.MM.yyyy")}");
                writer.WriteLine($"Home Team: {matchInfo.HomeTeam}");
                writer.WriteLine($"Away Team: {matchInfo.AwayTeam}");
            }
            else
            {
                writer.WriteLine("Match Information: Not available (Rugby format)");
            }

            if (xmlStrategy is RugbyParsingStrategy rugbyStrategy)
            {
                if (rugbyStrategy.ActionRows.Count > 0)
                {
                    writer.WriteLine($"Total Action Rows: {rugbyStrategy.ActionRows.Count}");
                }
                else if (rugbyStrategy.GameInfo != null)
                {
                    writer.WriteLine("Game File Information Parsed:");
                }
            }
            else if (xmlStrategy is SportDataParsingStrategy sportDataStrategy)
            {
                writer.WriteLine(sportDataStrategy.GetTeamPlayersList());
            }
            else if (xmlStrategy is InStatParsingStrategy inStatStrategy)
            {
                writer.WriteLine(inStatStrategy.GetTeamPlayersList());
            }

            writer.WriteLine("Parsed Contents:");
            var contentsToWrite = matchInfo?.ParsedContents ?? parsedContents;
            foreach (var content in contentsToWrite)
            {
                writer.WriteLine(content);
            }
        }
    }
}