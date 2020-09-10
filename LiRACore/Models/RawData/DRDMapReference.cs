using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LiRACore.Models.RawData;
using Microsoft.EntityFrameworkCore;

namespace LiRACore.Models.RawData
{
    [Owned]
    public class DRDMapReference
    {
        //Primary_Key
        //[Key]
        public Guid DRDMapReferenceId { get; set; }

        //[ForeignKey("Measurement")]
        //public int MeasurementRef_Id { get; set; }

        public float? lat_MapMatched { get; set; }
        public float? lon_MapMatched { get; set; }
        public string wayPointName { get; set; }
        public string legSummary_MapMatched { get; set; }
        public string legDistance_MapMatched { get; set; }
        public string nodeId_MapMatched { get; set; }

        public string offset { get; set; }
        public string lane { get; set; }
        public string direction { get; set; }

        //public Guid FK_RoadId { get; set; }

        public Guid FK_DRDMeasurementId { get; set; }


        //navigation property
        // public Measurement Measurement { get; set; }

        //navigation property
        // public Road Road { get; set; }

        public Guid FK_OSMWayPointId { get; set; }

        public Section Section { get; set; }

    }


}
