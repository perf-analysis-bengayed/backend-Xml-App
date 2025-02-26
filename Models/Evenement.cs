public class Evenement
    {
        public string Type { get; set; }
        public List<Critere> Criteres { get; set; } = new List<Critere>();
    }

   

 

    public class MappingImportRequest
    {
        public string XmlContent { get; set; }
    }

    public class EvenementFournisseurRequest
    {
        public string Type { get; set; }
        public List<Critere> Criteres { get; set; } = new List<Critere>();
    }