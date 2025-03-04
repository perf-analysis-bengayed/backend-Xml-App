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




    }
    
    

      
    
