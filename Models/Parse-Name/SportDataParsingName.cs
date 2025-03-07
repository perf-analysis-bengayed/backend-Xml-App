using System.Text;
using System.Xml.Linq;



public class SportDataParsingName : IFileNameParsingStrategy
    {
        public MatchNameInfo ParseFileName(string fileName)
        {
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var parts = nameWithoutExtension.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
            {
                throw new ArgumentException($"Format de nom de fichier SportData invalide : {fileName}");
            }

            int scoreIndex = Array.FindIndex(parts, p => p.Contains("-"));
            if (scoreIndex == -1 || scoreIndex < 1 || scoreIndex >= parts.Length - 2)
            {
                throw new ArgumentException($"Score introuvable ou mal positionné dans le nom de fichier SportData : {fileName}");
            }

            string score = parts[scoreIndex]; 

           
            int dateIndex = Array.FindIndex(parts, scoreIndex + 1, p => 
                p.Length >= 10 && p.Contains(".") && 
                DateTime.TryParseExact(p.Split(',')[0], "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out _));
            if (dateIndex == -1 || dateIndex <= scoreIndex)
            {
                throw new ArgumentException($"Date introuvable ou mal positionnée dans le nom de fichier SportData : {fileName}");
            }

            string date = parts[dateIndex].Split(',')[0]; 

           
            string homeTeam = string.Join(" ", parts.Take(scoreIndex)); 

            
            string awayTeam = string.Join(" ", parts.Skip(scoreIndex + 1).Take(dateIndex - scoreIndex - 1)); 

            return new MatchNameInfo
            {
                MatchDate = DateTime.ParseExact(date, "dd.MM.yyyy", null),
                HomeTeam = homeTeam,
                AwayTeam = awayTeam
            };
        }
    }