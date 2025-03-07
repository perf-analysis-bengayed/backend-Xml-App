using System.Text;
using System.Xml.Linq;
public class WyscoutParsingName : IFileNameParsingStrategy


{
    public MatchNameInfo ParseFileName(string fileName)
    {
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var parts = nameWithoutExtension.Split("--");
        if (parts.Length >= 3)
        {
            string date = parts[0]; // 08-10-2022
            string teamScorePart = parts[2]; // Ligue-1Chebba-3-0-Bizertin20230605-43291-ae55q0
            var matchParts = teamScorePart.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            string homeTeam = matchParts[0].Replace("Ligue-1", "").Replace("Ligue-", ""); // Chebba
            string awayTeam = matchParts[3].Split('2')[0]; // Bizertin

            return new MatchNameInfo
            {
                MatchDate = DateTime.ParseExact(date, "dd-MM-yyyy", null),
                HomeTeam = homeTeam,
                AwayTeam = awayTeam
            };
        }
        throw new ArgumentException($"Format de nom de fichier WYSCOUT invalide : {fileName}");
    }
}

