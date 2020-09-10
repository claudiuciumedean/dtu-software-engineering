using System;
using System.Collections.Generic;
using System.Text;
using LiRACore.Models.RawData;

namespace LiRACore.Models
{
    public class Section
    {
        public Guid OSMWayPointId { get; set; }

        public ICollection<Node> Nodes { get; set; }


        public Road Road { get; set; }

        public long FK_Road { get; set; }


        //public ICollection<MapReference> MapReferences { get; set; }

        //public ICollection<DRDMapReference> DRDMapReferences { get; set; }





        //public string legSummary { get; set; }
        //public string legDistance { get; set; }
        //public string nodeId { get; set; }

        //public string offset { get; set; }
        //public string offset { get; set; }
        //public string lane { get; set; }
        //public string direction { get; set; }

    }
}
