using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models.RawData
{

    //"_source": {
    //     "@vid": 34,
    //     "@uid": "ee811a7b-9a23-9628-52c5-0136dd89f55a",
    //     "@ts": "2019-04-08T15:26:39.052359Z",
    //     "@t": "acc.xyz",
    //     "acc.xyz.z": 0.83,
    //     "acc.xyz.y": -0.48,
    //     "acc.xyz.x": 0.14,
    //     "@rec": "2019-04-08T15:26:50.846777Z"
    //}

    public class Trigger
    {
        [JsonProperty("vid")]
        public string vehicleID { get; set; }

        [JsonProperty("uid")]
        public string unitID { get; set; }

        [JsonProperty("ts")]
        public string timeStamp { get; set; }

        [JsonProperty("t")]
        public object Tag { get; set; }

        [JsonProperty("rec")]
        public string recordTimeStamp { get; set; }
  
    }
         
}
