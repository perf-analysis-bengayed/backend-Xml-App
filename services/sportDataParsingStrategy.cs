using System.Text;
using System.Xml.Linq;

public class SportDataParsingStrategy : IXmlParsingStrategy
{
    private readonly List<InstanceData> instances = new List<InstanceData>();
    private readonly RowsParser rowsParser = new RowsParser(); 

    public List<InstanceData> Instances => instances;
    public List<RowData> Rows => rowsParser.Rows; 

    public void ParseElement(XElement element, StringBuilder xmlContentBuilder)
    {
        switch (element.Name.LocalName.ToUpper())
        {
            case "INSTANCE":
                InstanceData instance = ParseInstance(element);
                instances.Add(instance);
                AppendInstanceToBuilder(instance, xmlContentBuilder);
                break;

            case "ROW": 
                rowsParser.Parse(element.Parent, xmlContentBuilder, null); 
                break;

            case "ALL_INSTANCES":
                foreach (var child in element.Elements())
                {
                    ParseElement(child, xmlContentBuilder);
                }
                break;

            case "ROWS":
                rowsParser.Parse(element, xmlContentBuilder, null); 
                break;
        }
    }

    private InstanceData ParseInstance(XElement element)
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
            string[] codeParts = instance.Code.Split(new[] { '.' }, 2);
            if (codeParts.Length == 2)
            {
                instance.PlayerNumber = codeParts[0].Trim();
                string namePart = codeParts[1].Trim();
                
                int actionIndex = namePart.IndexOf(" - ");
                int parenIndex = namePart.IndexOf("(");
                if (parenIndex > 0)
                {
                    instance.PlayerName = namePart.Substring(0, parenIndex).Trim();
                    if (actionIndex > parenIndex && instance.Action == "N/A")
                    {
                        instance.Action = namePart.Substring(actionIndex + 3).Trim();
                    }
                }
                else if (actionIndex > 0)
                {
                    instance.PlayerName = namePart.Substring(0, actionIndex).Trim();
                    if (instance.Action == "N/A")
                    {
                        instance.Action = namePart.Substring(actionIndex + 3).Trim();
                    }
                }
                else
                {
                    instance.PlayerName = namePart;
                }
            }
        }

        return instance;
    }

    private void AppendInstanceToBuilder(InstanceData instance, StringBuilder xmlContentBuilder)
    {
        if (instance.PlayerNumber != "N/A" && instance.PlayerName != "Unknown")
        {
            string outputLine = $"joueur {instance.PlayerName} numero {instance.PlayerNumber} " +
                              $"Team {instance.Team} Action {instance.Action} " +
                              $"start {instance.Start} end {instance.End} " +
                              $"et pos_x {instance.PosX} pos_y {instance.PosY} Half {instance.Half}";
            xmlContentBuilder.AppendLine(outputLine);
        }
    }
}