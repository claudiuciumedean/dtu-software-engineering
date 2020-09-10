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
    public class MeasurementType
    {

        public MeasurementType()
        {
           
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MeasurementTypeId { get; set; }


        // one to many relationship with Measurement class
        public ICollection<Measurement> Measurements { get; set; }

        //DRD
        public ICollection<DRDMeasurement> DRDMeasurements { get; set; }

        public string type { get; set; }
               
        // Creation History
        public DateTimeOffset Created_Date { get; set; }




    }
}
