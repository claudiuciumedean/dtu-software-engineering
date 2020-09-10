﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiRACore.Osrm.Models
{
    public class MatchRequest : BaseRequest
    {
        protected const string DefaultGeometries = "polyline";
        //protected const string DefaultOverview = "simplified";
        protected const string DefaultOverview = "full";

        protected const bool DefaultAnnotations = true;
        protected const bool DefaultSteps = true;

        public MatchRequest()
        {
            Geometries = DefaultGeometries;
            Overview = DefaultOverview;
            Timestamps = new int[0];
            Annotations = DefaultAnnotations;
            Steps = DefaultSteps;
        }

        public bool Annotations { get; set; }

        /// <summary>
        /// Return route steps for each route
        /// true, false (default)
        /// </summary>
        public bool Steps { get; set; }

        /// <summary>
        /// Returned route geometry format (influences overview and per step)
        /// polyline (default), geojson
        /// </summary>
        public string Geometries { get; set; }

        /// <summary>
        /// Add overview geometry either full, simplified according to highest zoom level it could be display on, or not at all.
        /// simplified (default), full, false
        /// </summary>
        public string Overview { get; set; }

        /// <summary>
        /// Timestamp of the input location.
        /// simplified (default), full, false
        /// </summary>
        public int[] Timestamps { get; set; }

        public override List<Tuple<string, string>> UrlParams
        {
            get
            {
                var urlParams = new List<Tuple<string, string>>(BaseUrlParams);

                urlParams
                    .AddBoolParameter("steps", Steps, false)
                    .AddStringParameter("geometries", Geometries, () => Geometries != DefaultGeometries)
                    .AddStringParameter("overview", Overview, () => Overview != DefaultOverview)
                    .AddParams("timestamps", Timestamps.Select(x => x.ToString()).ToArray())                  
                    .AddBoolParameter("annotations", Annotations, false);

                //    .AddStringParameter("z", Zoom.ToString(), () => Zoom != DefaultZoom)
                //    .AddBoolParameter("alt", Alternative, true)
                //    .AddBoolParameter("geometry", Geometry, true)
                //    .AddBoolParameter("compression", Compression, true)
                //    .AddBoolParameter("uturns", UTurns, false)
                //    .AddBoolParameter("u", UTurnAtTheVia, false)
                //    .AddStringParameter("hint", Hint)
                //    .AddStringParameter("checksum", Checksum);

                return urlParams;
            }
        }
    }
}