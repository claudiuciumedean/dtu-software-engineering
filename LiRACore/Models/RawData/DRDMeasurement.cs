using LiRACore.Models.RawData;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LiRACore.Models.RawData
{
    //    event: The monitored event
    //•	@rec: timestamp of when event was received
    //•	@t: type of event
    //•	@tag: additional event data
    //•	@ts: timestamp from device
    //•	@uid: unit id of your device
    //•	@vid: vehicle id of the vehicle associated to your device
    //•	Any additional fields depending on the type of the event, and each field name will start with the type of the event, ie. if type is event.battery, each field will be called event.battery.NAME-OF-FIELD


    //[{"@vid": 36, 
    //  "@uid": "f8d815fe-ef73-6aad-063e-492ba05a6fd4", 
    //    "acc.xyz.pitch": -77.3104239653087, 
    //    "acc.xyz.roll": -127.41474148742192,
    //    "@ts": "2019-05-28T09:00:00.994595Z", 
    //    "@t": "acc.xyz", 
    //    "acc.xyz.z": -0.13,
    //    "acc.xyz.y": -0.17, 
    //    "acc.xyz.x": 0.95, 
    //    "@rec": "2019-05-28T10:38:52.946762Z"},
    public class DRDMeasurement
    {

        public DRDMeasurement()
        {
            DRDMapReferences = new DRDMapReference();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DRDMeasurementId { get; set; }

        //[ForeignKey("Trip")]
        //public int TripRef_Id { get; set; }
        //public Guid? UId { get; set; }
        //public long? VId { get; set; }
        public string TS_or_Distance { get; set; }
        //public DateTimeOffset? Rec { get; set; }
        public string T { get; set; }
        //public string ts_temp { get; set; }       
        //public string tag { get; set; }        
        public float? lat { get; set; }
        public float? lon { get; set; }
        public string  message { get; set; }
        public bool isComputed { get; set; }

        // Inverse navigation property of relationship with Trip
        public Trip Trip { get; set; }
        public Guid FK_Trip { get; set; }

        //zero-to-one relationship
        public DRDMapReference DRDMapReferences { get; set; }
        //public Guid FK_RoadReference { get; set; }

        // one-to-one relationship
        public int FK_MeasurementType { get; set; }
        public MeasurementType Measurement_Type { get; set; }

        // Creation History
        public DateTimeOffset Created_Date { get; set; }

        // last update date
        public DateTimeOffset Updated_Date { get; set; }




    }
}
