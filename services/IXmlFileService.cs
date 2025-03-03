
public interface IXmlFileService
{
    Task<List<string>> UploadXmlFilesAsync(List<IFormFile> files, MatchInfo matchInfo);
      
}
