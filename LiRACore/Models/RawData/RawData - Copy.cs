using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LiRACore.Models.AutoPi
{
    //    event: The monitored event
    //•	@rec: timestamp of when event was received
    //•	@t: type of event
    //•	@tag: additional event data
    //•	@ts: timestamp from device
    //•	@uid: unit id of your device
    //•	@vid: vehicle id of the vehicle associated to your device
    //•	Any additional fields depending on the type of the event, and each field name will start with the type of the event, ie. if type is event.battery, each field will be called event.battery.NAME-OF-FIELD

    public class RawData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LiRA_Id { get; set; }

        public string T { get; set; }

        //public string primaryKey { get; set; }
        public string objectId { get; set; }
      
        public string token { get; set; }
        public DateTimeOffset ts { get; set; }
        public string ts_temp { get; set; }       
        public string tag { get; set; }        
        public float? lat { get; set; }
        public float? lon { get; set; }
        public string  message { get; set; }
        public bool isComputed { get; set; }
        public float? lat_MapMatched { get; set; }
        public float? lon_MapMatched { get; set; }
        public string wayPointName { get; set; }

        public string legSummary_GeoReferenced { get; set; }

        public string legDistance_GeoReferenced { get; set; }

        public string nodeId_GeoReferenced { get; set; }




    }
}
