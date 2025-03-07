using Microsoft.AspNetCore.Mvc;

namespace WebXmlApplication.Controllers;

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
public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files, [FromForm] DateTime? matchDate = null)
{
    try
    {
        if (files == null || !files.Any())
        {
            return BadRequest(new { Message = "Aucun fichier n'a été fourni." });
        }

        var matchInfo = new MatchInfo
        {
            MatchDate = matchDate ?? DateTime.MinValue
        };

        var parsedContents = await _xmlFileService.UploadXmlFilesAsync(files, matchInfo);

        // Get the match file content
        string matchFilePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "ficheMatch.txt");
        string matchFileContent = System.IO.File.Exists(matchFilePath) 
            ? await System.IO.File.ReadAllTextAsync(matchFilePath)
            : "Match file not found";

        // Generate the match file name for reference
        string matchFileName = parsedContents.Any()
            ? $"{matchInfo.HomeTeam ?? "Unknown"} vs {matchInfo.AwayTeam ?? "Unknown"} {matchInfo.MatchDate:dd.MM.yyyy}.fiche"
            : "UnknownMatch.fiche";

        return Ok(new
        {
            Message = "Fichiers XML traités avec succès",
            MatchFileName = matchFileName,
            MatchFileContent = matchFileContent,
            ParsedContents = parsedContents
        });
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { Message = ex.Message });
    }
    catch (FormatException ex)
    {
        return StatusCode(500, new
        {
            Message = "Erreur serveur lors du traitement des fichiers",
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { 
            Message = "Erreur serveur lors du traitement des fichiers", 
            Detail = ex.Message 
        });
    }
}}