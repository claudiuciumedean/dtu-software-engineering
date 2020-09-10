using LiRACore.Models.RawData;
using LiRACore.Osrm;
using LiRACore.Osrm.Models;
using LiRACore.Osrm.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiRACore.Services
{
    public class OsrmMapMatchingService: IMapMatchingService
    {
        // async Task<bool> InsertData(RawDataModel data);
       // public static string OsrmUrl = "http://router.project-osrm.org/";
        public static string OsrmUrl = "http://liradb.compute.dtu.dk:5000/";

        public string Url { get; set; }

        /// <summary>
        /// Version of the protocol implemented by the service.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Mode of transportation, is determined by the profile that is used to prepare the data
        /// </summary>
        public string Profile { get; set; }

        /// <summary>
        /// Timeout for web request. If not specified default value will be used.
        /// </summary>
        public int? Timeout { get; set; }

        protected readonly string RouteServiceName = "route";
        protected readonly string NearestServiceName = "nearest";
        protected readonly string MatchServiceName = "match";
        
        
        public OsrmMapMatchingService(string url, string version = "v1", string profile = "car")
        {
            Url = url;
            Version = version;
            Profile = profile;
        }
        public Osrm.Models.Responses.MatchResponse Match(MatchRequest requestParams)
        {
            return Send<MatchResponse>(MatchServiceName, requestParams);
        }

        protected T Send<T>(string service, BaseRequest request, int notry=5) //string coordinatesStr, List<Tuple<string, string>> urlParams)
        {
            var coordinatesStr = request.CoordinatesUrlPart;
            List<Tuple<string, string>> urlParams = request.UrlParams;
            var fullUrl = OsrmRequestBuilder.GetUrl(Url, service, Version, Profile, coordinatesStr, urlParams);
            string json = null;
            try
            {
                using (var client = new OsrmWebClient(Timeout))
                {
                    json = client.DownloadString(new Uri(fullUrl));
                }
                    
            }
            catch (Exception ex)
            {
                int sleep = 1;
                if (notry > 0)
                {

                    Thread.Sleep(sleep++ * 1000);
                    return (T)Send<T>(service, request, --notry);
                }
                 throw ex;
                //JsonConvert.DeserializeObject<T>("empty"); 
            }
            return JsonConvert.DeserializeObject<T>(json); ;
        }

        public Osrm.Models.Responses.MatchResponse Match(LiRACore.Osrm.Location[] locations)
        {
            var request = new LiRACore.Osrm.Models.MatchRequest()
            {
                Coordinates = locations
            };

            return Match(request);
        }
    }
    public class OsrmWebClient : WebClient
    {
        private readonly int? _specificTimeout;

        public OsrmWebClient(int? timeout = 30)
        {
            _specificTimeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            try
            {
              

                if (request != null && _specificTimeout.HasValue)
                    request.Timeout = _specificTimeout.Value;
            }
            catch (Exception ex)
            {

                throw;
              
            }

            return request;
        }
    }
}
