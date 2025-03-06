using System.Text;
using System.Xml.Linq;

public class ParseInstanceSportData
{
 public InstanceData ParseInstanceSportData1(XElement element)
    {
        var instance = new InstanceData
        {
            Start = element.Element("start")?.Value.Trim() ?? "N/A",
            End = element.Element("end")?.Value.Trim() ?? "N/A",
            Code = element.Element("code")?.Value.Trim() ?? ""
        };

        instance.Labels = element.Elements("label").Select(l => new Label
        {
            Group = l.Element("group")?.Value.Trim(),
            Text = l.Element("text")?.Value.Trim()
        }).ToList();

        foreach (var label in instance.Labels)
        {
            switch (label.Group)
            {
                case "Team": instance.Team = label.Text ?? "N/A"; break;
                case "Action": instance.Action = label.Text ?? "N/A"; break;
                case "Half": instance.Half = label.Text ?? "N/A"; break;
                case "pos_x": instance.PosX = label.Text ?? "N/A"; break;
                case "pos_y": instance.PosY = label.Text ?? "N/A"; break;
            }
        }

        if (!string.IsNullOrEmpty(instance.Code))
        {
            string[] codeParts = instance.Code.Split(new[] { " - " }, StringSplitOptions.None);
            if (codeParts.Length == 2)
            {
                string prefixPart = codeParts[0].Trim();
                string actionPart = codeParts[1].Trim();

                string[] prefixSubParts = prefixPart.Split(new[] { '.' }, 2);
                if (prefixSubParts.Length == 2 && prefixSubParts[0].Trim().All(char.IsDigit))
                {
                    instance.PlayerNumber = prefixSubParts[0].Trim();
                    string nameWithId = prefixSubParts[1].Trim();
                    int parenIndex = nameWithId.IndexOf("(");
                    instance.PlayerName = parenIndex > 0 ? nameWithId.Substring(0, parenIndex).Trim() : nameWithId;
                    if (instance.Action == "N/A") instance.Action = actionPart;
                    if (instance.Team == "N/A") instance.Team = "Unknown";
                }
                else
                {
                    instance.Team = prefixPart;
                    if (instance.Action == "N/A") instance.Action = actionPart;
                }
            }
            else if (instance.Team == "N/A")
            {
                instance.Team = instance.Code;
            }
        }

        return instance;
    }


}