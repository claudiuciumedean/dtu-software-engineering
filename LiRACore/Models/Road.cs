using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models
{
    public class Road
    {
       // public Guid LiRA_Road_Id { get; set; }
        public long Road_Id { get; set; }
        public string RoadName { get; set; }
                
        public ICollection<Section> Sections { get; set; }

        //public string legSummary { get; set; }
        //public string legDistance { get; set; }
        //public string nodeId { get; set; }

        //public string offset { get; set; }
        //public string lane { get; set; }
        //public string direction { get; set; }

    }
}
