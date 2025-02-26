using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebXmlApplication.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using OfficeOpenXml;
using System.Text;
namespace WebXmlApplication.Controllers;
using WebXmlApplication.Models;
 [ApiController]
    [Route("api/xml-import")]
    public class XmlImportController : ControllerBase
    {

    
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        public XmlImportController()
        {
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

[HttpPost("upload")]
public async Task<IActionResult> UploadXmlFiles([FromForm] List<IFormFile> files, [FromForm] MatchInfo matchInfo)
{
    if (files == null || files.Count == 0)
    {
        return BadRequest("Aucun fichier n'a été téléchargé.");
    }

    List<string> parsedContents = new List<string>();

    foreach (var file in files)
    {
        if (Path.GetExtension(file.FileName).ToLower() != ".xml")
        {
            return BadRequest($"Le fichier {file.FileName} doit être de type .xml.");
        }

        var uploadedFilePath = Path.Combine("uploads", file.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(uploadedFilePath));

        using (var stream = new FileStream(uploadedFilePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        string parsedContent = ParseXmlFile(uploadedFilePath);
        parsedContents.Add(parsedContent);
    }

    var matchFilePath = Path.Combine("uploads", "ficheMatch.xml");
    CreateMatchFile(matchFilePath, parsedContents, matchInfo);

    return Ok(new { ParsedContents = parsedContents });
}

//       [HttpPost("upload")]
// public async Task<IActionResult> UploadXmlFile([FromForm] IFormFile file, [FromForm] MatchInfo matchInfo)

// {
//     if (file == null || file.Length == 0)
//     {
//         return BadRequest("Aucun fichier n'a été téléchargé.");
//     }

//     if (Path.GetExtension(file.FileName).ToLower() != ".xml")
//     {
//         return BadRequest("Le fichier doit être de type .xml.");
//     }

//     var uploadedFilePath = Path.Combine("uploads", file.FileName);
//     Directory.CreateDirectory(Path.GetDirectoryName(uploadedFilePath));

//     using (var stream = new FileStream(uploadedFilePath, FileMode.Create))
//     {
//         await file.CopyToAsync(stream);
//     }

//         string parsedContent = ParseXmlFile(uploadedFilePath);

//         var matchFilePath = Path.Combine("uploads", "ficheMatch.xml");
//         CreateMatchFile(matchFilePath, parsedContent, matchInfo);

//         return Ok(new {  ParsedContent = parsedContent });
//     }

    // private void CreateMatchFile(string filePath,  List<string> parsedContents, MatchInfo matchInfo)
    // {
    //     var matchElement = new XElement("Match",
    //         new XElement("MatchDate", matchInfo.MatchDate.ToString("yyyy-MM-dd")),
    //         // new XElement("EquipeLocale", matchInfo.EquipeLocale),
    //         // new XElement("EquipeVisiteuse", matchInfo.EquipeVisiteuse),
    //         // new XElement("Competition", matchInfo.Competition),
    //         // new XElement("JourneeChampionnat", matchInfo.JourneeChampionnat),
    //         // new XElement("EquipeSuivie", matchInfo.EquipeSuivie),
    //         // new XElement("ListeJoueurs",
    //         //     new XElement("EquipeLocale", string.Join(", ", matchInfo.ListeJoueursEquipeLocale)),
    //         //     new XElement("EquipeVisiteuse", string.Join(", ", matchInfo.ListeJoueursEquipeVisiteuse))
    //         // ),
    //         // new XElement("FichiersVideo", string.Join(", ", matchInfo.FichiersVideo)),
    //         // new XElement("CoteTerrainPremierePeriode", matchInfo.CoteTerrainPremierePeriode),
    //         // new XElement("TimecodesDebutPeriodes", string.Join(", ", matchInfo.TimecodesDebutPeriodes)),
    //         new XElement("ParsedContent", parsedContent)
    //     );

    //     var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), matchElement);
    //     document.Save(filePath);
    // }

private void CreateMatchFile(string filePath, List<string> parsedContents, MatchInfo matchInfo)

{
    var matchElement = new XElement("Match",
        new XElement("MatchDate", matchInfo.MatchDate.ToString("yyyy-MM-dd")),
        // Ajoutez ici d'autres éléments de `matchInfo` si nécessaire
        new XElement("ParsedContents", new XElement("Contents", parsedContents.Select(pc => new XElement("Content", pc))))
    );

    var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), matchElement);
    document.Save(filePath);
}




private string ParseXmlFile(string filePath)
{
    StringBuilder xmlContentBuilder = new StringBuilder();
    try
    {
        XDocument xmlDocument = XDocument.Load(filePath);

        foreach (var element in xmlDocument.Descendants())
        {
            if (!string.IsNullOrWhiteSpace(element.Value))
            {

switch (element.Value)
                {
                    case "Aerial duels":
                        xmlContentBuilder.AppendLine($"{element.Name}/Aerial duels " );
                        xmlContentBuilder.AppendLine($"{element.Name}/Duel " );
                        xmlContentBuilder.AppendLine($"{element.Name}/Aérien " );
                        break;
                    case "Aggressiveness":
                        xmlContentBuilder.AppendLine("Aerial Aggressiveness1 /" + element.Value);
                        xmlContentBuilder.AppendLine("Aerial Aggressiveness2 /" + element.Value);
                        break;
                    default:
                        xmlContentBuilder.AppendLine($"{element.Name} /{element.Value}");
                        break;
                }  
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

    }
    
    

      
    
