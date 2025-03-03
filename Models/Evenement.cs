public class Event
{
    public int Id { get; set; }
    public string EventType { get; set; }
    public string Description { get; set; }
  
   
}

 

    public class MappingImportRequest
    {
        public string XmlContent { get; set; }
    }

    public class EvenementFournisseurRequest
    {
        public string Type { get; set; }
        
    }