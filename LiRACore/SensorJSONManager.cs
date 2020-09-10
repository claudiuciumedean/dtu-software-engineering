using LiRACore.Models;
using LiRACore.Models.AutoPi;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore
{
    class SensorJSONManager
    {



        void AutoPiSensorDataSchemaGenerate()
        {

            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(Trigger));
            //        {
            //            "type": "object",
            //           "properties": {
            //                "email": { "type": "string", "format": "email" }
            //            },
            //           "required": [ "email" ]
            //}









        }
    }
}
