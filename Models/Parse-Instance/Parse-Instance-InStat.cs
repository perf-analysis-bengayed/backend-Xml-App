using System.Text;
using System.Xml.Linq;

public class ParseInstanceInStat 
{
   public InstanceData ParseInstanceInStat1(XElement element)
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
            }
        }

        instance.PosX = element.Element("pos_x")?.Value.Trim() ?? "N/A";
        instance.PosY = element.Element("pos_y")?.Value.Trim() ?? "N/A";

        if (!string.IsNullOrEmpty(instance.Code) && instance.Code.Contains("."))
        {
            instance.PlayerNumber = instance.Code.Split('.')[0].Trim();
            instance.PlayerName = instance.Code.Substring(instance.Code.IndexOf('.') + 1).Trim();
        }

        return instance;
    }

}