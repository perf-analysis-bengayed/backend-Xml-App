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
   


   // Lancer la suppression automatique après 5 minutes
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




// public class WyscoutParsingStrategy : IXmlParsingStrategy
// {
//     public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
//     {
//         switch (element.Value.Trim())
//         {
//             case "Aerial duels":
//                 xmlContentBuilder.AppendLine($"{element.Name}/Aerial duels");
//                 xmlContentBuilder.AppendLine($"{element.Name}/Duel");
//                 xmlContentBuilder.AppendLine($"{element.Name}/Aérien");
//                 break;
//             case "Aggressiveness":
//                 xmlContentBuilder.AppendLine("Aerial Aggressiveness1 /{element.Value}");
//                 xmlContentBuilder.AppendLine("Aerial Aggressiveness2 /{element.Value}");
//                 break;
//             default:
//                 xmlContentBuilder.AppendLine($"{element.Name} /{element.Value}");
//                 break;
//         }
//     }
// }

// public class InStatParsingStrategy : IXmlParsingStrategy
// {
//     private readonly List<string> playerNames = new List<string>();
//     private readonly List<string> outputLines = new List<string>();
//     private readonly HashSet<string> teamNames = new HashSet<string>();


//     public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
//     {
//         switch (element.Name.LocalName.ToUpper())
//         {
//             case "ALL_INSTANCES":
//                 ParseAllInstances(element, xmlContentBuilder);
//                 break;
//             case "ROWS":
//                 ParseRows(element, xmlContentBuilder);
//                 break;
//             case "SORT_INFO":
//                 ParseSortInfo(element, xmlContentBuilder);
//                 break;
//             // default:
//             //     ParseInstanceElement(element, xmlContentBuilder);
//             //     break;
//         }
//     }

// private void ParseAllInstances(XElement element, StringBuilder xmlContentBuilder)
// {
//     foreach (var instance in element.Elements("instance"))
//     {
//         string start = instance.Element("start")?.Value.Trim() ?? "N/A";
//         string end = instance.Element("end")?.Value.Trim() ?? "N/A";
//         string code = instance.Element("code")?.Value.Trim() ?? "Unknown";
//         string posX = instance.Element("pos_x")?.Value.Trim() ?? "N/A";
//         string posY = instance.Element("pos_y")?.Value.Trim() ?? "N/A";

    
//         if (IsPlayerCode(code))
//         {
//             string[] codeParts = code.Split(new[] { '.' }, 2);
//             string playerNumber = codeParts[0].Trim();
//             string playerName = codeParts[1].Trim();

//             if (!playerNames.Contains(playerName))
//             {
//                 playerNames.Add(playerName);
//             }
       
          
//             string team = "";
//             string action = "N/A";
//             string half = "N/A";
//             foreach (var label in instance.Elements("label"))
//             {
//                 var group = label.Element("group")?.Value.Trim();
//                 var text = label.Element("text")?.Value.Trim();
//                 if (group == "Team")
//                 {
//                     team = text;
//                     if (!string.IsNullOrWhiteSpace(team))
//                     {
//                         teamNames.Add(team);
//                     }
//                 }
//                 else if (group == "Action")
//                 {
//                     action = text;
//                 }
//                 else if (group == "Half")
//                 {
//                     half = text;
//                 }
//             }

//             string outputLine = $"joueur {playerName} numero {playerNumber} Team {team} Action {action} start {start} end {end} et pos_x {posX} pos_y {posY} Half {half}";
//             xmlContentBuilder.AppendLine(outputLine);
//         }
//         else
//         {
//             string team = "";
//             string action = code;

//             if (IsTeamActionCode(code))
//             {
//                 string[] codeParts = code.Split(new[] { ' ' }, 2);
//                 team = codeParts[0].Trim();
//                 action = codeParts.Length > 1 ? codeParts[1].Trim() : "Unknown Action";
              
//             }

       
//             xmlContentBuilder.AppendLine($"{team} {action} start {start} end {end}");
//         }
//     }
// }
//        private void ParseRows(XElement element, StringBuilder xmlContentBuilder)
//     {
//         foreach (var row in element.Elements("row"))
//         {
//             string code = row.Element("code")?.Value.Trim() ?? "Unknown";
//             string r = row.Element("R")?.Value.Trim() ?? "0";
//             string g = row.Element("G")?.Value.Trim() ?? "0";
//             string b = row.Element("B")?.Value.Trim() ?? "0";

          

//             xmlContentBuilder.AppendLine($"{code} R {r}, G {g}, B {b}");
//         }
//     }

//     private void ParseSortInfo(XElement element, StringBuilder xmlContentBuilder)
//     {
//         string sortType = element.Element("sort_type")?.Value.Trim() ?? "N/A";
//         xmlContentBuilder.AppendLine($"{sortType}");
//     }

    
//     private bool IsPlayerCode(string code)
//     {
//         if (string.IsNullOrEmpty(code))
//             return false;

//         string[] parts = code.Split(new[] { '.' }, 2);
//         if (parts.Length != 2)
//             return false;

//         return int.TryParse(parts[0].Trim(), out _) && !string.IsNullOrWhiteSpace(parts[1].Trim());
//     }

//     private bool IsTeamActionCode(string code)
//     {
//         if (string.IsNullOrEmpty(code))
//             return false;

//         string[] parts = code.Split(new[] { ' ' }, 2);
//         return parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[0]) && !string.IsNullOrWhiteSpace(parts[1]);
//     }

//     public void AppendFinalPlayerNames(StringBuilder xmlContentBuilder)
// {
//     foreach (var line in outputLines)
//     {
//         xmlContentBuilder.AppendLine(line);
//     }

//     if (playerNames.Any())
//     {
//         var playerCodes = playerNames.Select((name, index) => $"{name}").ToList();
//         xmlContentBuilder.AppendLine($"Liste des Joueurs: {string.Join(", ", playerCodes)}");
//     }

//     if (teamNames.Any())
//     {
//         xmlContentBuilder.AppendLine($"Équipe de match :{string.Join(" et ", teamNames)}");
//     }
    
//     }

//     public List<string> GetPlayerNames()
//     {
//         return new List<string>(playerNames); 
//     }

//     public List<string> GetTeamNames()
//     {
//         return new List<string>(teamNames); 
//     }
// }


}
