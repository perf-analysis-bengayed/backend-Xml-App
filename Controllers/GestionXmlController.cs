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

    

private readonly IXmlFileService _xmlFileService;

    public XmlImportController(IXmlFileService xmlFileService)
    {
        _xmlFileService = xmlFileService;
    }

  [HttpPost("upload")]
    public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files, [FromForm] DateTime matchDate)
    {
        try
        {
            var matchInfo = new MatchInfo { MatchDate = matchDate };
            var parsedContents = await _xmlFileService.UploadXmlFilesAsync(files, matchInfo);
            
            return Ok(new
            {
                Message = "Fichiers XML traités avec succès",
                ParsedContents = parsedContents
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Erreur serveur lors du traitement des fichiers", Detail = ex.Message });
        }
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


    }
    
    

      
    
