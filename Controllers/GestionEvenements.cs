using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;




 // Classe pour la correspondance d'un événement entre le fournisseur et TechFOOT
      public class MappingEvenement
    {
        public string EvenementFournisseur { get; set; }
        public string EvenementTechFOOT { get; set; }
        public List<MappingCritere> MappingCriteres { get; set; } = new List<MappingCritere>();
    }

    public class MappingCritere
    {
        public string CritereFournisseur { get; set; }
        public string CritereTechFOOT { get; set; }
    }



