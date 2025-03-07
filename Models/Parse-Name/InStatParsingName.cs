using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

public class InStatParsingName : IFileNameParsingStrategy
{
    public MatchNameInfo ParseFileName(string fileName)
    {
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var parts = nameWithoutExtension.Split(new[] { "--" }, StringSplitOptions.None);

        if (parts.Length < 3)
        {
            throw new ArgumentException($"Invalid InStat filename format - insufficient parts: {fileName}");
        }

        string datePart = parts[0];

        string leaguePart = parts[2];

        
        var pattern = @"^Ligue-1(?<homeTeam>.+?)-(?<scoreHome>\d+)-(?<scoreAway>\d+)-(?<awayTeam>.+?)(\d{8}.*)$";
        var match = Regex.Match(leaguePart, pattern);

        if (!match.Success)
        {
            throw new ArgumentException($"Invalid InStat filename format: {fileName}");
        }

        string homeTeam = match.Groups["homeTeam"].Value;

        string awayTeamRaw = match.Groups["awayTeam"].Value;
        string awayTeam = CleanAwayTeam(awayTeamRaw);

        DateTime matchDate;
        try
        {
            matchDate = DateTime.ParseExact(datePart, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException($"Invalid date format in filename: {fileName}", ex);
        }

        return new MatchNameInfo
        {
            MatchDate = matchDate,
            HomeTeam = homeTeam,
            AwayTeam = awayTeam
        };
    }

    private string CleanAwayTeam(string awayTeamRaw)
    {
        // Remove any trailing digits (such as the extra match info date and IDs)
        int digitIndex = -1;
        for (int i = 0; i < awayTeamRaw.Length; i++)
        {
            if (char.IsDigit(awayTeamRaw[i]) && i > 2) // Avoid cutting too early (for cases like "US-")
            {
                digitIndex = i;
                break;
            }
        }
        string cleaned = digitIndex > 0 ? awayTeamRaw.Substring(0, digitIndex) : awayTeamRaw;
        return cleaned;
    }
}
