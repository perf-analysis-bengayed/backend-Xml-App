public interface IFileNameParsingStrategy
{
    MatchNameInfo ParseFileName(string fileName);
}