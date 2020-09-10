using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LiRACore.Osrm.Models
{
    [DataContract]
    public class Route
    {
        [DataMember(Name = "distance")]
        public double Distance { get; set; }

        [DataMember(Name = "duration")]
        public double Duration { get; set; }

        [DataMember(Name = "geometry")]
        public string GeometryStr { get; set; }

        public Location[] Geometry
        {
            get
            {
                if (string.IsNullOrEmpty(GeometryStr))
                {
                    return new Location[0];
                }

                return OsrmPolylineConverter.Decode(GeometryStr, 1E5)
                    .ToArray();
            }
        }

        [DataMember(Name = "legs")]
        public RouteLeg[] Legs { get; set; }

        /// <summary>
        /// Match. Confidence of the matching. float value between 0 and 1. 1 is very confident that the matching is correct.
        /// </summary>
        [DataMember(Name = "confidence")]
        public float? Confidence { get; set; }
    }
    [DataContract]
    public class RouteLeg
    {
        [DataMember(Name = "annotation")]
        public Annotation annotation { get; set; }

        [DataMember(Name = "distance")]
        public double Distance { get; set; }

        [DataMember(Name = "duration")]
        public double Duration { get; set; }

        [DataMember(Name = "steps")]
        public RouteStep[] Steps { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "weight")]
        public double weight { get; set; }
    }



    [DataContract]
    public class RouteStep
    {
        [DataMember(Name = "distance")]
        public double Distance { get; set; }

        [DataMember(Name = "duration")]
        public double Duration { get; set; }

        [DataMember(Name = "geometry")]
        public string GeometryStr { get; set; }

        public Location[] Geometry
        {
            get
            {
                if (string.IsNullOrEmpty(GeometryStr))
                {
                    return new Location[0];
                }

                return OsrmPolylineConverter.Decode(GeometryStr, 1E5)
                    .ToArray();
            }
        }

        [DataMember(Name = "maneuver")]
        public StepManeuver Maneuver { get; set; }

        [DataMember(Name = "mode")]
        public string Mode { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "@ref")]
        public string @ref { get; set; }

        [DataMember(Name = "weight")]
        public double weight { get; set; }

        [DataMember(Name = "driving_side")]
        public string driving_side { get; set; }

        [DataMember(Name = "intersections")]
        public List<Intersection> intersections { get; set; }
    }

    [DataContract]
    public class Intersection
    {
        [DataMember(Name = "@out")]
        public int @out { get; set; }

        [DataMember(Name = "entry")]
        public List<bool> entry { get; set; }

        [DataMember(Name = "bearings")]
        public List<int> bearings { get; set; }

        [DataMember(Name = "location")]
        public List<double> location { get; set; }

        [DataMember(Name = "lanes")]
        public List<Lane> lanes { get; set; }

        [DataMember(Name = "@in")]
        public int? @in { get; set; }
    }

    [DataContract]
    public class Lane
    {
        [DataMember(Name = "valid")]
        public bool valid { get; set; }

        [DataMember(Name = "indications")]
        public List<string> indications { get; set; }
    }



    [DataContract]
    public class Annotation
    {
        [DataMember(Name = "metadata")]
        public Metadata metadata { get; set; }

        [DataMember(Name = "nodes")]
        public List<object> nodes { get; set; }

        [DataMember(Name = "datasources")]
        public List<int> datasources { get; set; }

        [DataMember(Name = "weight")]
        public List<double> weight { get; set; }

        [DataMember(Name = "distance")]
        public List<double> distance { get; set; }

        [DataMember(Name = "duration")]
        public List<double> duration { get; set; }
    }

        [DataContract]
        public class Metadata
        {
            [DataMember(Name = "datasource_names")]
            public List<string> datasource_names { get; set; }
        }


    [DataContract]
    public class StepManeuver
    {
        /// <summary>
        /// The clockwise angle from true north to the direction of travel immediately after the maneuver.
        /// </summary>
        [DataMember(Name = "bearing_after")]
        protected int BearingAfter { get; set; }

        /// <summary>
        /// The clockwise angle from true north to the direction of travel immediately before the maneuver.
        /// </summary>
        [DataMember(Name = "bearing_before")]
        protected int BearingBefore { get; set; }

        /// <summary>
        /// An optional integer indicating number of the exit to take. The field exists for the following type field:
        /// </summary>
        [DataMember(Name = "exit")]
        protected int Exit { get; set; }

        [DataMember(Name = "location")]
        protected double[] LocationArr { get; set; }

        public Location Location
        {
            get
            {
                if (LocationArr == null)
                    return null;

                return new Location(LocationArr[0], LocationArr[1]);
            }
        }

        /// <summary>
        /// A string indicating the type of maneuver. new identifiers might be introduced without API change Types unknown to the client should be handled like the turn type, the existance of correct modifier values is guranteed.
        /// </summary>
        [DataMember(Name = "type")]
        protected string Type { get; set; }

        /// <summary>
        /// An optional string indicating the direction change of the maneuver.
        /// </summary>
        [DataMember(Name = "modifier")]
        protected string Modifier { get; set; }
    }
}
