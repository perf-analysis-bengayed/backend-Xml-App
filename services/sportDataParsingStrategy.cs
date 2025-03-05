using System.Text;
using System.Xml.Linq;

public class SportDataParsingStrategy : IXmlParsingStrategy
{
    public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
    {
        if (element.Name.LocalName == "instance")
        {
            string start = element.Element("start")?.Value ?? "N/A";
            string end = element.Element("end")?.Value ?? "N/A";
            string code = element.Element("code")?.Value ?? "";

            var labels = element.Elements("label").ToDictionary(
                l => l.Element("group")?.Value,
                l => l.Element("text")?.Value);

            string action = labels.GetValueOrDefault("Action") ?? "N/A";
            string half = labels.GetValueOrDefault("Half") ?? "N/A";
            string posX = labels.GetValueOrDefault("pos_x") ?? "N/A";
            string posY = labels.GetValueOrDefault("pos_y") ?? "N/A";

            string team = "";
            string codeAction = "";
            if (!string.IsNullOrEmpty(code))
            {
            
                string[] codeParts = code.Split(new[] { " - " }, StringSplitOptions.None);
                if (codeParts.Length == 2)
                {
                    team = codeParts[0].Trim(); 
                    codeAction = codeParts[1].Trim(); 
                }
                else
                {
                    team = code; 
                }
            }

            string finalAction = action != "N/A" ? action : codeAction;

            if (!string.IsNullOrEmpty(team) && !string.IsNullOrEmpty(finalAction))
            {
                string outputLine = $"{team} {finalAction} start {start} end {end} Action {finalAction} Half {half} pos_x {posX} pos_y {posY}";
                xmlContentBuilder.AppendLine(outputLine);
            }
        }
    }
}