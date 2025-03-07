using System.Text;
using System.Xml.Linq;

public interface IXmlFileService
{
    Task<List<string>> UploadXmlFilesAsync(List<IFormFile> files, MatchInfo matchInfo);
}