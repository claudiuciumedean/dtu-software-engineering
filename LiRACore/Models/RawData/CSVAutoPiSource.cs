using LiRACore.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.Models.RawData
{


    public class CSVSampleSource
    {

        [JsonProperty("@token", NullValueHandling = NullValueHandling.Ignore)]
        public long? Token { get; set; }

        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? _Id { get; set; }

        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public double? X { get; set; }

        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public double? Y { get; set; }

        [JsonProperty("z", NullValueHandling = NullValueHandling.Ignore)]
        public double? Z { get; set; }

    }

        public partial class CSVAutoPiSource
    {

        [JsonProperty("@token", NullValueHandling = NullValueHandling.Ignore)]
        public long? Token { get; set; }

        [JsonProperty("_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? _Id { get; set; }

        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public double? X { get; set; }

        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public double? Y { get; set; }

        [JsonProperty("z", NullValueHandling = NullValueHandling.Ignore)]
        public double? Z { get; set; }




        [JsonProperty("@vid", NullValueHandling = NullValueHandling.Ignore)]
        public long? Vid { get; set; }

        [JsonProperty("event.system.time.old", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? EventSystemTimeOld { get; set; }

        [JsonProperty("@tag", NullValueHandling = NullValueHandling.Ignore)]
        public string Tag { get; set; }

        [JsonProperty("@t", NullValueHandling = NullValueHandling.Ignore)]
        public string T { get; set; }

        [JsonProperty("event.system.time.source", NullValueHandling = NullValueHandling.Ignore)]
        public string EventSystemTimeSource { get; set; }

        [JsonProperty("@ts", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset Ts { get; set; }

        [JsonProperty("@uid", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Uid { get; set; }

        [JsonProperty("@rec", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Rec { get; set; }

        [JsonProperty("event.system.time.new", NullValueHandling = NullValueHandling.Ignore)]
        public string EventSystemTimeNew { get; set; }

        [JsonProperty("event.system.power.trigger", NullValueHandling = NullValueHandling.Ignore)]
        public string EventSystemPowerTrigger { get; set; }

        [JsonProperty("acc.xyz.z", NullValueHandling = NullValueHandling.Ignore)]
        public double? AccXyzZ { get; set; }

        [JsonProperty("acc.xyz.y", NullValueHandling = NullValueHandling.Ignore)]
        public double? AccXyzY { get; set; }

        [JsonProperty("acc.xyz.x", NullValueHandling = NullValueHandling.Ignore)]
        public double? AccXyzX { get; set; }

        [JsonProperty("event.vehicle.obd.protocol", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? EventVehicleObdProtocol { get; set; }

        [JsonProperty("event.vehicle.obd.autodetected", NullValueHandling = NullValueHandling.Ignore)]
        public bool? EventVehicleObdAutodetected { get; set; }

        [JsonProperty("obd.bat.voltage", NullValueHandling = NullValueHandling.Ignore)]
        public double? ObdBatVoltage { get; set; }

        [JsonProperty("obd.bat.state", NullValueHandling = NullValueHandling.Ignore)]
        public string ObdBatState { get; set; }

        [JsonProperty("obd.bat.level", NullValueHandling = NullValueHandling.Ignore)]
        public long? ObdBatLevel { get; set; }

        [JsonProperty("obd.speed.value", NullValueHandling = NullValueHandling.Ignore)]
        public long? ObdSpeedValue { get; set; }

        [JsonProperty("obd.speed.unit", NullValueHandling = NullValueHandling.Ignore)]
        public string ObdSpeedUnit { get; set; }

        [JsonProperty("obd.rpm.unit", NullValueHandling = NullValueHandling.Ignore)]
        public string ObdRpmUnit { get; set; }

        [JsonProperty("obd.rpm.value", NullValueHandling = NullValueHandling.Ignore)]
        public double? ObdRpmValue { get; set; }

        [JsonProperty("obd.engine_load.unit", NullValueHandling = NullValueHandling.Ignore)]
        public string ObdEngineLoadUnit { get; set; }

        [JsonProperty("obd.engine_load.value", NullValueHandling = NullValueHandling.Ignore)]
        public double? ObdEngineLoadValue { get; set; }

        [JsonProperty("obd.coolant_temp.value", NullValueHandling = NullValueHandling.Ignore)]
        public long? ObdCoolantTempValue { get; set; }

        [JsonProperty("obd.coolant_temp.unit", NullValueHandling = NullValueHandling.Ignore)]
        public string ObdCoolantTempUnit { get; set; }

        [JsonProperty("obd.intake_temp.value", NullValueHandling = NullValueHandling.Ignore)]
        public long? ObdIntakeTempValue { get; set; }

        [JsonProperty("obd.intake_temp.unit", NullValueHandling = NullValueHandling.Ignore)]
        public string ObdIntakeTempUnit { get; set; }

        [JsonProperty("obd.ambiant_air_temp.value", NullValueHandling = NullValueHandling.Ignore)]
        public long? ObdAmbiantAirTempValue { get; set; }

        [JsonProperty("obd.ambiant_air_temp.unit", NullValueHandling = NullValueHandling.Ignore)]
        public string ObdAmbiantAirTempUnit { get; set; }

        [JsonProperty("rpi.temp.cpu", NullValueHandling = NullValueHandling.Ignore)]
        public RpiTemp RpiTempCpu { get; set; }

        [JsonProperty("rpi.temp.gpu", NullValueHandling = NullValueHandling.Ignore)]
        public RpiTemp RpiTempGpu { get; set; }

        [JsonProperty("track.pos.utc", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? TrackPosUtc { get; set; }

        [JsonProperty("track.pos.cog", NullValueHandling = NullValueHandling.Ignore)]
        public double? TrackPosCog { get; set; }

        [JsonProperty("track.pos.nsat", NullValueHandling = NullValueHandling.Ignore)]
        public long? TrackPosNsat { get; set; }

        [JsonProperty("track.pos.alt", NullValueHandling = NullValueHandling.Ignore)]
        public double? TrackPosAlt { get; set; }

        [JsonProperty("track.pos.sog", NullValueHandling = NullValueHandling.Ignore)]
        public double? TrackPosSog { get; set; }

        [JsonProperty("track.pos.loc", NullValueHandling = NullValueHandling.Ignore)]
        public TrackPosLoc TrackPosLoc { get; set; }
    }

    //public partial class RpiTemp
    //{
    //    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    //    public double? Value { get; set; }

    //    [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
    //    public string Unit { get; set; }
    //}

    //public partial class TrackPosLoc
    //{
    //    [JsonProperty("lat", NullValueHandling = NullValueHandling.Ignore)]
    //    public float? Lat { get; set; }

    //    [JsonProperty("lon", NullValueHandling = NullValueHandling.Ignore)]
    //    public float? Lon { get; set; }
    //}




    public class myDynamicClassDataLine : System.Dynamic.DynamicObject
    {
        string[] _lineContent; // Actual line data
        List<string> _headers; // Associated headers (properties)

        public string this[string indexer]
        {
            get
            {
                string result = string.Empty;
                int index = _headers.IndexOf(indexer);

                if (index >= 0 && index < _lineContent.Length)
                    result = _lineContent[index];

                return result;
            }

        }
    }

}
