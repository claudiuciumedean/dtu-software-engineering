using LiRACore.Models.RawData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Text;
using MongoDB.Bson;
using System.Linq;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Servers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.IO;
using LiRACore.Services.AutoPi;
using System.Globalization;
using static LiRACore.Services.AutoPi.SensorReader;
using System.Collections;
using LiRACore.Services;
using LiRACore.Osrm;
using LiRACore.Osrm.Models.Responses;   
using LiRACore.Osrm.Models;
using System.IO;
using LiRACore.Models.RawData;
using LiRACore.Models.RawData.List;
using LiRACore.Models.RawData;
using LiRACore.Models.Repository;
using LiRACore.Models.DataManager;
using LiRACore.Models;
using LiRACore.Helpers;
using LiRACore.Models.RawData.Raw;

namespace LiRACore
{
    public class RawDataPipeLine 
    {
        // // The Azure Cosmos DB endpoint for running this sample.
        //// private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];
        // private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

        // // The primary key for the Azure Cosmos account.
        //// private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];
        // private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];


        // The Cosmos client instance
        // public static MongoClient mongoClient;
        private readonly MongoClient _mongoClient;
       
        private List<string> eventList = new List<string>();
        private int mapMatch_BatchSize = 100;
        private InterpolationType Interpolation_Type = InterpolationType.Linear;
        private int Read_TrackPosition_Size = 1;

        private static List<Measurement> nonInter_list = new List<Measurement>();
        private static ICollection<Measurement> inter_list = new List<Measurement>(3);

        private static SizedQueue<Measurement> LastVisited_TrackPos = new SizedQueue<Measurement>(3);
        private IRawDataService dataStore;
        private IMapMatchingService _mapMatchingService;
        //private readonly IDataRepository<Measurement> _dataRepository;
        private readonly IDataRepository<Device> _deviceRepository;
        private readonly IDataRepository<Trip> _tripRepository;
        private readonly IDataRepository<MeasurementType> _measurementTypeRepository;
        private readonly MeasurementManager _measurementRepository;

        public RawDataPipeLine(IRawDataService dataStore, IMapMatchingService mapMatchingService)
        {
            this.dataStore = dataStore;
            this._mapMatchingService = mapMatchingService;
            
        }

        public RawDataPipeLine(IMapMatchingService mapMatchingService, IDataRepository<Device> deviceRepository, IDataRepository<Trip> tripRepository, IDataRepository<MeasurementType> measurementTypeRepository, MeasurementManager measurementRepository, InterpolationType interpolationType)

        {
            this._mapMatchingService = mapMatchingService;

            this._deviceRepository = deviceRepository;

            this._tripRepository = tripRepository;

            this._measurementTypeRepository = measurementTypeRepository;

            this._measurementRepository = measurementRepository;

            this.Interpolation_Type = interpolationType;


            switch (this.Interpolation_Type)
            {
                case InterpolationType.Quadratic:
                    Read_TrackPosition_Size = 3;
                    break;
                case InterpolationType.Cubic:
                    Read_TrackPosition_Size = 5;
                    break;
                default:
                    Read_TrackPosition_Size = 1;
                    break;
            }


        }



        public RawDataPipeLine(MongoClient mongoClient, IMapMatchingService mapMatchingService, IDataRepository<Device> deviceRepository, IDataRepository<Trip> tripRepository, IDataRepository<MeasurementType>  measurementTypeRepository, MeasurementManager measurementRepository, InterpolationType interpolationType)

          {
            this._mongoClient = mongoClient;

            this._mapMatchingService = mapMatchingService;

            this._deviceRepository = deviceRepository;

            this._tripRepository = tripRepository;

            this._measurementTypeRepository = measurementTypeRepository;

            this._measurementRepository = measurementRepository;

            this.Interpolation_Type = interpolationType;


            switch (this.Interpolation_Type)
            {
               
                case InterpolationType.Quadratic:
                    Read_TrackPosition_Size = 3;
                    break;
                case InterpolationType.Cubic:
                    Read_TrackPosition_Size = 5;
                    break;
                default:
                    Read_TrackPosition_Size = 1;
                    break;
            }


        }

        public  bool IsClusterConnected
        {
            get
            {
               try
                {
                   
                    var databases = _mongoClient.ListDatabases();
                    databases.MoveNextAsync(); // Force MongoDB to connect to the database.

                    return _mongoClient.Cluster.Description.State == ClusterState.Connected; 


                }
                catch (Exception  ex)
                {
                    throw ex;
                }                             

           }

        }

        public bool IsServerConnceted
        {
            get
            {
                try
                {

                    var databases = _mongoClient.ListDatabases();
                    databases.MoveNextAsync(); // Force MongoDB to connect to the database.

                    return _mongoClient.Cluster.Description.Servers.Single().State == ServerState.Connected;


                }
                catch (Exception ex)
                {
                    throw ex;
                }

              
            }
        }




        //public async Task StartGMPullingData()
        //{

        //    try
        //    {
        //        IMongoDatabase mongoDataBase;
        //        List<BsonDocument> devices_List;
        //        IMongoCollection<BsonDocument> track_pos_collection;
        //        List<string> collectionNames;
        //        FindOptions<BsonDocument> options;

        //        PreReading(out mongoDataBase, out devices_List, out track_pos_collection, out collectionNames, out options);

        //        //read by batch cosidering the filtering
        //        Queue eventQueue = new Queue();


        //        // var batch = 0;
        //        foreach (var device in devices_List)
        //        {
        //            var filterBuilder = Builders<BsonDocument>.Filter;
        //            var filter_Device = filterBuilder.Eq("@token", device.GetElement("tokens").Value.AsBsonDocument.GetElement(0).Name);







        //            using (IAsyncCursor<BsonDocument> cursor = await track_pos_collection.FindAsync(filter_Device, options))
        //            {

        //                var batch = 0;

        //                while (await cursor.MoveNextAsync())
        //                {

        //                    IEnumerable<BsonDocument> batch_TrachPostions = cursor.Current;


        //                    batch++;



        //                    Console.WriteLine($"Batch: {batch}");
        //                    // Here we have retreived 3 numbers of track positions 
        //                    for (int i = 0; i < batch_TrachPostions.Count(); i++)

        //                    {
        //                        //Creat GPS points
        //                        Interpolated GPS_point = Create_GPS_Point(batch_TrachPostions, i);

        //                        //Adding first actual point
        //                        SensorReader.allPoints.Add(GPS_point);
        //                        eventQueue.Enqueue(GPS_point);
        //                    }


        //                    while ()

        //                        // Cleaning the data
        //                        if (nonInterpolate_list.Find(x => x.ts == GPS_point.ts) != null)
        //                        {
        //                            nonInter_list.Add(GPS_point);
        //                            SensorReader.nonInterpolate_list.Add(GPS_point);
        //                        }-
        //                }



        //                if (nonInter_list.Count >= 3)

        //                // Collect events which require interpolation
        //                // if (SensorReader.nonInterpolate_list.Count > 1)
        //                // if (i < batch_TrachPostions.Count() - 1)
        //                {

        //                    Pull_NonGPS_Points(mongoDataBase, collectionNames, filterBuilder, filter_Device, GPS_point);

        //                }


        //                else
        //                {
        //                    //Adding first actual point
        //                    //SensorReader.allPoints.Add(GPS_point);
        //                }





        //                foreach (Interpolated p in allPoints)
        //                {
        //                    Console.Write(p.isComputed.ToString() + "  ");
        //                    Console.Write(allPoints.IndexOf(p).ToString() + "  ");
        //                    Console.Write(p.objectId.ToString() + "  ");
        //                    Console.WriteLine(p.ts.ToString());
        //                }



        //            }



        //            Console.WriteLine($"Total Batch: { batch}");

        //            // For every batch, we do the interpolation 
        //            SensorReader.interpolationType = InterpolationType.Quadratic;
        //            SensorReader.Interpolate(SensorReader.interpolationType);

        //            foreach (Interpolated p in allPoints)
        //            {
        //                Console.Write(p.isComputed.ToString() + "  ");
        //                Console.Write(allPoints.IndexOf(p).ToString() + "  ");
        //                Console.Write(p.objectId.ToString() + "  ");
        //                Console.Write(p.lat.ToString() + "  ");
        //                Console.Write(p.lon.ToString() + "  ");
        //                Console.WriteLine(p.ts.ToString());

        //            }
        //            SensorReader.Print(SensorReader.interpolationType);

        //        }






        //        //foreach (string eventCollection in collectionNames)
        //        //    {
        //        //        var collection = mongoDataBase.GetCollection<BsonDocument>(eventCollection);

        //        //        //read by batch cosidering the filtering
        //        //        using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter_Device, options))
        //        //        {

        //        //            var batch = 0;
        //        //            while (await cursor.MoveNextAsync())
        //        //            {
        //        //                IEnumerable<BsonDocument> documents = cursor.Current;
        //        //                batch++;

        //        //                Console.WriteLine($"Batch: {batch}");

        //        //                foreach (BsonDocument document in documents)
        //        //                {
        //        //                    Console.WriteLine(document);
        //        //                    Console.WriteLine();
        //        //                }
        //        //            }

        //        //            Console.WriteLine($"Total Batch: { batch}");
        //        //        }


        //        //        collection.GetType();
        //        //        //var data = collection.Find(x => (   (Convert.ToDateTime( x.GetElement("@ts").Value) < Convert.ToDateTime(track_pos_List[i].GetElement("@ts").Value)) )) //&& (x.GetElement(1).Value < track_pos_List[i+1].GetElement("@ts").Value) ) )
        //        //        //    .ForEachAsync(x => Console.WriteLine(x.GetElement("@ts").Value));



        //        //        //var data = collection.Find(x => (((x.GetElement("token").Value)) == (track_pos_List[i].GetElement("@token").Value))); //&& (x.GetElement(1).Value < track_pos_List[i+1].GetElement("@ts").Value) ) )
        //        //        //   .ForEachAsync(x => Console.WriteLine(x.GetElement("@token").Value));





        //        //    }


        //        //}




        //        //for (int i=0; i< track_pos_List.Count; i++ )
        //        //{

        //        //    filter_Device = filter_Device & filterBuilder.Lte("@ts", "token e227bddc-574d-40ba-91cc-1803f638065e");


        //        //    foreach (string eventCollection in collectionNames)
        //        //    {
        //        //        var collection = mongoDataBase.GetCollection<BsonDocument>(eventCollection);

        //        //        //read by batch cosidering the filtering
        //        //        using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter_Device, options))
        //        //        {

        //        //            var batch = 0;
        //        //            while (await cursor.MoveNextAsync())
        //        //            {
        //        //                IEnumerable<BsonDocument> documents = cursor.Current;
        //        //                batch++;

        //        //                Console.WriteLine($"Batch: {batch}");

        //        //                foreach (BsonDocument document in documents)
        //        //                {
        //        //                    Console.WriteLine(document);
        //        //                    Console.WriteLine();
        //        //                }
        //        //            }

        //        //            Console.WriteLine($"Total Batch: { batch}");
        //        //        }


        //        //        collection.GetType();
        //        //        //var data = collection.Find(x => (   (Convert.ToDateTime( x.GetElement("@ts").Value) < Convert.ToDateTime(track_pos_List[i].GetElement("@ts").Value)) )) //&& (x.GetElement(1).Value < track_pos_List[i+1].GetElement("@ts").Value) ) )
        //        //        //    .ForEachAsync(x => Console.WriteLine(x.GetElement("@ts").Value));



        //        //        //var data = collection.Find(x => (((x.GetElement("token").Value)) == (track_pos_List[i].GetElement("@token").Value))); //&& (x.GetElement(1).Value < track_pos_List[i+1].GetElement("@ts").Value) ) )
        //        //            //   .ForEachAsync(x => Console.WriteLine(x.GetElement("@token").Value));





        //        //    }


        //        //}



        //    }
        //    }

        public async Task Start_GM_RawData_Pipline()
        {

           
                IMongoDatabase mongoDataBase;
                List<BsonDocument> devices_List;
                IMongoCollection<BsonDocument> track_pos_collection;
                List<string> collectionNames;
                FindOptions<BsonDocument> options;

                //  STEP 1: collect collection names and devices list
                PreReading(out mongoDataBase, out devices_List, out track_pos_collection, out collectionNames, out options);

                //Queue batches data due to interpolation purposes
                Queue eventQueue = new Queue();


                 var batch = 0;
                // STEP 2: Reading by batch
                foreach (var device in devices_List)
                {
                    // Reading batches per device
                    var filterBuilder = Builders<BsonDocument>.Filter;
                    var filter_Device = filterBuilder.Eq("@token", device.GetElement("tokens").Value.AsBsonDocument.GetElement(0).Name);
                    
                    
                    using (IAsyncCursor<BsonDocument> cursor = await track_pos_collection.FindAsync(filter_Device, options))
                    {

                       // var batch = 0;

                        while (await cursor.MoveNextAsync())
                        {

                                IEnumerable<BsonDocument> batch_TrachPostions = cursor.Current;
                                batch++;

                                Console.WriteLine($"Batch: {batch}");


                                // STEP 3: Queue GPS events of the current batch
                                CreateGPSEvents_AddQueue(eventQueue, batch_TrachPostions);

                                // STEP 4: Cleanning the data due to the fact that we might receive GPS events with the same tiemstamp but different object Ids
                                //         which deteriorate the interpolation algorithm, although we save them in DB later on
                                CleanData(eventQueue);

                                // STEP 5: Send for interpolation if we have enough actual GPS points
                                if (nonInterpolate_list.Count >= 3)
                                {

                                       Pull_NonGPS_Events(mongoDataBase, collectionNames, filterBuilder, filter_Device);

                                        SensorReader.interpolationType = InterpolationType.Linear;
                                        SensorReader.Interpolate(SensorReader.interpolationType);
                                        SensorReader.Print(SensorReader.interpolationType);


                                        Location[] mapMatchingArray = new Location[interpolate_list.Count];
                                        for (int i = 0; i < interpolate_list.Count; i++)
                                        {
                                                Location x = new Location((double)interpolate_list[i].lat, (double)interpolate_list[i].lon);

                                                if (mapMatchingArray[0] != null)
                                                {
                                                   IEnumerable<Location> z = mapMatchingArray.Where(y => y != null && y.Latitude == x.Latitude);
                                    
                                                    if (z.Count() < 1)
                                                    {
                                                        // Location point = new Location(Math.Round((double)interpolate_list[i].lat, 6), Math.Round((double)interpolate_list[i].lon, 6));
                                                        Location point = new Location((double)interpolate_list[i].lat, (double)interpolate_list[i].lon);

                                                        mapMatchingArray[i] = point;
                                                    }
                                                }

                                                else
                                                {
                                                    Location point = new Location((double)interpolate_list[i].lat, (double)interpolate_list[i].lon);
                                                    mapMatchingArray[i] = point;
                                                }

                                        }

                                        //var mapMatchingArray = new Location[] {
                                        //    new Location(52.517037, 13.388860),
                                        //    new Location(52.529407, 13.397634),
                                        //    new Location(52.523219, 13.428555)
                                        //     };

                                        // STEP 6: Map matching (Geo-referencing)
                                        _mapMatchingService.Match(mapMatchingArray);


                                        // STEP 6: Clear lists 
                                        interpolate_list.Clear();
                                                    // Keep the last point for the next round of interpolation
                                        nonInterpolate_list.RemoveRange(0, nonInterpolate_list.Count - 1);


                                }

                       

                               

                               // STEP 7: Save in DataBase



                    }






                    //if (nonInter_list.Count >= 3)

                    //// Collect events which require interpolation
                    //// if (SensorReader.nonInterpolate_list.Count > 1)
                    //// if (i < batch_TrachPostions.Count() - 1)
                    //{

                    //    Pull_NonGPS_Points(mongoDataBase, collectionNames, filterBuilder, filter_Device, GPS_point);

                    //}


                    //else
                    //{
                    //    //Adding first actual point
                    //    //SensorReader.allPoints.Add(GPS_point);
                    //}





                    //foreach (Interpolated p in allPoints)
                    //{
                    //    Console.Write(p.isComputed.ToString() + "  ");
                    //    Console.Write(allPoints.IndexOf(p).ToString() + "  ");
                    //    Console.Write(p.objectId.ToString() + "  ");
                    //    Console.WriteLine(p.ts.ToString());
                    //}



                }



                    //Console.WriteLine($"Total Batch: { batch}");

                    //// For every batch, we do the interpolation 
                    //SensorReader.interpolationType = InterpolationType.Quadratic;
                    //SensorReader.Interpolate(SensorReader.interpolationType);

                    //foreach (Interpolated p in allPoints)
                    //{
                    //    Console.Write(p.isComputed.ToString() + "  ");
                    //    Console.Write(allPoints.IndexOf(p).ToString() + "  ");
                    //    Console.Write(p.objectId.ToString() + "  ");
                    //    Console.Write(p.lat.ToString() + "  ");
                    //    Console.Write(p.lon.ToString() + "  ");
                    //    Console.WriteLine(p.ts.ToString());

                    //}
                    //SensorReader.Print(SensorReader.interpolationType);

                }






            //foreach (string eventCollection in collectionNames)
            //    {
            //        var collection = mongoDataBase.GetCollection<BsonDocument>(eventCollection);

            //        //read by batch cosidering the filtering
            //        using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter_Device, options))
            //        {

            //            var batch = 0;
            //            while (await cursor.MoveNextAsync())
            //            {
            //                IEnumerable<BsonDocument> documents = cursor.Current;
            //                batch++;

            //                Console.WriteLine($"Batch: {batch}");

            //                foreach (BsonDocument document in documents)
            //                {
            //                    Console.WriteLine(document);
            //                    Console.WriteLine();
            //                }
            //            }

            //            Console.WriteLine($"Total Batch: { batch}");
            //        }


            //        collection.GetType();
            //        //var data = collection.Find(x => (   (Convert.ToDateTime( x.GetElement("@ts").Value) < Convert.ToDateTime(track_pos_List[i].GetElement("@ts").Value)) )) //&& (x.GetElement(1).Value < track_pos_List[i+1].GetElement("@ts").Value) ) )
            //        //    .ForEachAsync(x => Console.WriteLine(x.GetElement("@ts").Value));



            //        //var data = collection.Find(x => (((x.GetElement("token").Value)) == (track_pos_List[i].GetElement("@token").Value))); //&& (x.GetElement(1).Value < track_pos_List[i+1].GetElement("@ts").Value) ) )
            //        //   .ForEachAsync(x => Console.WriteLine(x.GetElement("@token").Value));





            //    }


            //}




            //for (int i=0; i< track_pos_List.Count; i++ )
            //{

            //    filter_Device = filter_Device & filterBuilder.Lte("@ts", "token e227bddc-574d-40ba-91cc-1803f638065e");


            //    foreach (string eventCollection in collectionNames)
            //    {
            //        var collection = mongoDataBase.GetCollection<BsonDocument>(eventCollection);

            //        //read by batch cosidering the filtering
            //        using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(filter_Device, options))
            //        {

            //            var batch = 0;
            //            while (await cursor.MoveNextAsync())
            //            {
            //                IEnumerable<BsonDocument> documents = cursor.Current;
            //                batch++;

            //                Console.WriteLine($"Batch: {batch}");

            //                foreach (BsonDocument document in documents)
            //                {
            //                    Console.WriteLine(document);
            //                    Console.WriteLine();
            //                }
            //            }

            //            Console.WriteLine($"Total Batch: { batch}");
            //        }


            //        collection.GetType();
            //        //var data = collection.Find(x => (   (Convert.ToDateTime( x.GetElement("@ts").Value) < Convert.ToDateTime(track_pos_List[i].GetElement("@ts").Value)) )) //&& (x.GetElement(1).Value < track_pos_List[i+1].GetElement("@ts").Value) ) )
            //        //    .ForEachAsync(x => Console.WriteLine(x.GetElement("@ts").Value));



            //        //var data = collection.Find(x => (((x.GetElement("token").Value)) == (track_pos_List[i].GetElement("@token").Value))); //&& (x.GetElement(1).Value < track_pos_List[i+1].GetElement("@ts").Value) ) )
            //            //   .ForEachAsync(x => Console.WriteLine(x.GetElement("@token").Value));





            //    }


            //}




        }



        //public async Task Start_JSON_RawData_Pipline(List<AutoPi> sensorData)
        //{


        //    int eventNo = sensorData.Count;
        //    int FirstGPSIndex = -1;
        //    int LastNonGPSIndex = -1;
        //    int trackPosition_No = 0;
        //    int i = 0;

        //    while (i < eventNo)
        //    {
        //        RawData point = new RawData();
        //        point.LiRA_Id = Guid.NewGuid().ToString();
        //        point.UId = sensorData[i].Uid.ToString();
        //        point.VId = sensorData[i].Vid.ToString();
        //        point.TS = sensorData[i].Ts;
        //        point.T = sensorData[i].T;
        //        point.Rec = sensorData[i].Rec;
        //        point.message = Newtonsoft.Json.JsonConvert.SerializeObject(sensorData[i]);

        //        // Points with actual GPS coordinates
        //        if (sensorData[i].TrackPosLoc != null)
        //        {
        //            point.lat = sensorData[i].TrackPosLoc.Lat;
        //            point.lon = sensorData[i].TrackPosLoc.Lon;
        //            point.isComputed = false;


        //            trackPosition_No++;

        //            nonInterpolate_list.Add(point);

        //            if (FirstGPSIndex == -1)
        //            {
        //                FirstGPSIndex = i;

        //            }
        //            else
        //            {
        //                LastNonGPSIndex = i - 1;
        //            }
        //        }

        //        else if (FirstGPSIndex != -1)  //Pointes which require interpolation but we do not do backwards interpolation 
        //        {

        //            point.isComputed = true;

        //            interpolate_list.Add(point);

        //        }

        //        allPoints.Add(point);


        //    }


        //    //maybe temporary: removing points which their timestamp is after visiting the last actual GPS point 
        //    interpolate_list.RemoveAll(r => r.TS > nonInterpolate_list[nonInterpolate_list.Count - 1].TS);



        //    PrintAllPoint(PrintMode.Raw, InterpolationType.Non);



        //    interpolationType = InterpolationType.Cubic;

        //    Interpolate(interpolationType);




        //    PrintAllPoint(PrintMode.Interpolated, interpolationType);


        //    // Map matching
        //    Geo_Referencing(mapMatch_BatchSize);
        //    PrintAllPoint(PrintMode.MapMatched, interpolationType, _dataRepository);

        //    //PrintAllPoint(InterpolationType.Quadratic);


        //    //Save in database

        //}


        // "@ts": "2019-04-08T15:20:48.176357Z",
        //"@t": "acc.xyz",
        //"acc.xyz.z": 0.83,
        //"acc.xyz.y": -0.52,
        //"acc.xyz.x": 0.22,
        //"@rec": "2019-04-08T15:20:56.642819Z"

        //public async Task Start_JSON_RawData_Pipline(List<AutoPi_Json> sensorData)
      public async Task Start_JSON_RawData_Pipline(string sensorDataFileJSON)
        {

            var jsonReader = new JsonTextReader(new StreamReader(sensorDataFileJSON))
            {
                SupportMultipleContent = true // This is important!
            };

            var jsonSerializer = new JsonSerializer();


            //Create Device
            Device device = new Device();
            Trip trip = new Trip();
            bool IsFirstevent = false;
            Measurement FirstGPS_measurement = new Measurement();
            Measurement LastGPS_measurement = new Measurement();
            int trackPosition_No = 0;

            try
            {
                while (jsonReader.Read())
                {

                    AutoPi_Json autoPi = jsonSerializer.Deserialize<AutoPi_Json>(jsonReader);
                    // add device first

                    // Save first trip data
                    if (autoPi.StartPositionDisplay != null)
                    {


                        // Use GET method first for Device, if not, create 
                        //For measurement_type you should GET first
                        Guid deviceId = (Guid)autoPi.Device;
                        var _device = _deviceRepository.Get(deviceId);

                        if (_device != null && _device.DeviceId != Guid.Empty)
                        {

                            device = _device;
                            // we assume trip is new and will work on recovery of trip data at some other point :)
                            trip = _tripRepository.Get(deviceId, autoPi.Id);
                        }

                        else // Device does not exist
                        {
                            device.DeviceId = (Guid)autoPi.Device;
                            device.Created_Date = DateTime.Now;

                        }
                        // Use GET method first for Device, if not, create 
                        //For measurement_type you should GET first
                        Guid tripId = (Guid)autoPi.Id;
                        //device = _deviceRepository.Get(deviceId);

                        trip.Created_Date = DateTime.Now;
                        trip.TripId = autoPi.Id;
                        // trip.Device = autoPi.Device;
                        trip.DistanceKm = autoPi.DistanceKm;
                        trip.StartPositionLat = autoPi.StartPositionLat;
                        trip.StartPositionLng = autoPi.StartPositionLng;
                        trip.StartTimeUtc = autoPi.StartTimeUtc;
                        trip.EndPositionDisplay = Newtonsoft.Json.JsonConvert.SerializeObject(autoPi.EndPositionDisplay);
                        trip.StartPositionDisplay = Newtonsoft.Json.JsonConvert.SerializeObject(autoPi.StartPositionDisplay);
                        trip.EndPositionLat = autoPi.EndPositionLat;
                        trip.EndPositionLng = autoPi.EndPositionLng;
                        trip.EndTimeUtc = autoPi.EndTimeUtc;

                        //TODO: tag is not filled to some reasons
                       // trip.Tag = autoPi.Tag;
                        trip.Duration = autoPi.Duration;
                        trip.EndTimeUtc = autoPi.EndTimeUtc;



                        IsFirstevent = true;


                        device.Trips.Add(trip);

                        _deviceRepository.Add(device);


                        //create the first and last GPS locations from the Trip data
                        FirstGPS_measurement = CreateMeasurement(autoPi, Event_Type.FristGPS_Trip);
                        LastVisited_TrackPos.Enqueue(FirstGPS_measurement);

                        
                        allPoints.Add(FirstGPS_measurement);

                         
                        nonInterpolate_list.Add(FirstGPS_measurement);
                        trackPosition_No++;

                        LastGPS_measurement = CreateMeasurement(autoPi, Event_Type.LastGPS_Trip);
                    }
                    else
                    {

                        // First event after reading trip data
                        Measurement measurement = CreateMeasurement(autoPi);
                        allPoints.Add(measurement);


                        if (autoPi.T == "track.pos")
                        {
                            trackPosition_No++;
                     
                            //  it dequeue when is filled as it is sizedQueue, but it should be tested

                            LastVisited_TrackPos.Enqueue(measurement);

                            nonInterpolate_list.Add(measurement);

                        }
                        else
                        {
                            interpolate_list.Add(measurement);
                        }

                        if (trackPosition_No == Read_TrackPosition_Size)
                        {
                            //nonInterpolate_list = LastVisited_TrackPos.ToList();

                            BatchPreProcessing(device, trip, false);


                            nonInterpolate_list.Clear();
                            interpolate_list.Clear();
                            allPoints.Clear();
                           


                            // Keep the last visited track position to be used, and saved in the next round
                            nonInterpolate_list.Add(LastVisited_TrackPos.LastOrDefault());
                            allPoints.Add(LastVisited_TrackPos.LastOrDefault());
                            trackPosition_No = 1;
                        }
                        else
                        {

                        }
                        // For save
                        //trip.Measurements.Add(measurement);

                        //measurement.TripRef_Id =
                    }


                }




                //if (i == 0)
                //{
                //    k = i;
                //}
                //else
                //{
                //    if (i == nonInterpolate_list.Count - 1)
                //    {
                //        k = i - 2;
                //    }
                //    else
                //    {
                //        k = i - 1;
                //    }
                //}



                // if two track positions is read, and the last events has been read from the file, so we add the end track postion from trip event and send the last set to the for interpolation and map-matching
                if (trackPosition_No == 1 )
                {
                    //

                    // nonInterpolate_list should be size of one
                    Measurement m =  LastVisited_TrackPos.FirstOrDefault(x => x == nonInterpolate_list[nonInterpolate_list.Count -1]);
                    //1th only for the purpose of interpolation
                    nonInterpolate_list.Insert(0, LastVisited_TrackPos.FirstOrDefault(x => x == nonInterpolate_list[nonInterpolate_list.Count - 1]));

                    //3th
                    nonInterpolate_list.Add(LastGPS_measurement);
                    //Add to allPoint to be saved
                    allPoints.Add(LastGPS_measurement);

                    BatchPreProcessing(device, trip, true);

                    //TODO: allPoints.Clear(LastVisited_TrackPos);

                }


                if (trackPosition_No == 2)
                {

                    nonInterpolate_list.Add(LastGPS_measurement);                    
                    //Add to allPoint to be saved
                    allPoints.Add(LastGPS_measurement);

                    BatchPreProcessing(device, trip, true);

                    //TODO: allPoints.Clear(LastVisited_TrackPos);

                }


                //if (trackPosition_No == 0)
                //{
                //    //Create non_iterpolated list for the sake of interpolation, but we sholud make sure the track positions events are not saved double in the db.
                //    // we do not need the oldest trakc postion in the last three events
                //    LastVisited_TrackPos.Dequeue();

                //    //
                //    nonInterpolate_list.Add(LastVisited_TrackPos.Dequeue());
                //    nonInterpolate_list.Add(LastVisited_TrackPos.Dequeue());
                //    nonInterpolate_list.Add(LastGPS_measurement);

                //    allPoints.Add(LastGPS_measurement);
                //}







            }
            catch (Exception ex)
            {


                string x = string.Empty;

                int y = -1; 
                for (int i = 0; i < allPoints.Count - 1; i++) 
                { 
                    if (allPoints.ElementAt(i).MeasurementId.ToString() == x) 
                    {
                        y = i; 
                    } 
                }


                throw ex;
            }



            //////int eventNo = sensorData.Count;
            //////int FirstGPSIndex = -1;   
            //////int LastNonGPSIndex = -1;
            //////int trackPosition_No = 0;
            //////int i = 0;

            //////// Skip first Non-Track Position events
            //////try
            //////{
            //////    while (sensorData[i].T != "track.pos")
            //////    {
            //////        i++;
            //////    }

            //////}
            //////catch (Exception ex)
            //////{

            //////    throw;
            //////}


            //////////while (i < eventNo)
            //////////{
            //////////    Measurement point = new Measurement();
            //////////    point.LiRA_Id = Guid.NewGuid().ToString();
            //////////    point.UId = sensorData[i].Uid.ToString();
            //////////    point.VId = sensorData[i].Vid.ToString();
            //////////    point.TS = DateTimeOffset.Parse(sensorData[i].Ts.ToString("yyyy-MM-ddThh:mm:ss.ffffffZ"));
            //////////    point.T = sensorData[i].T;
            //////////    point.Rec = sensorData[i].Rec;
            //////////    point.message = Newtonsoft.Json.JsonConvert.SerializeObject(sensorData[i]);

            //////////    // Points with actual GPS coordinates
            //////////    if (sensorData[i].TrackPosLoc != null)
            //////////    {
            //////////        point.lat = sensorData[i].TrackPosLoc.Lat;
            //////////        point.lon = sensorData[i].TrackPosLoc.Lon;
            //////////        point.isComputed = false;


            //////////        trackPosition_No++;
            //////////        nonInterpolate_list.Add(point);

            //////////        if (FirstGPSIndex == -1)
            //////////        {
            //////////            FirstGPSIndex = i;
            //////////        }
            //////////        else
            //////////        {
            //////////            LastNonGPSIndex = i - 1;
            //////////        }
            //////////    }

            //////////    else if (FirstGPSIndex != -1)  //Pointes which require interpolation but we do not do backwards interpolation 
            //////////    {

            //////////        point.isComputed = true;
            //////////        interpolate_list.Add(point);

            //////////    }

            //////////    allPoints.Add(point);

            //////////    try
            //////////    {
            //////////        _dataRepository.Add(point);
            //////////    }
            //////////    catch (Exception ex)
            //////////    {

            //////////        throw ex;
            //////////    }

            //////////    i++;


                // If enough number of Track position events are received, the interpolation and map-matching will be in placed:

                //if (trackPosition_No == Read_TrackPosition_Size)
                //{


                //    // Interpolation
                //    Interpolate(Interpolation_Type);


                //    // Map matching
                //    Geo_Referencing(mapMatch_BatchSize);



                //}

         //////////   }


            ////maybe temporary: removing points which their timestamp is after visiting the last actual GPS point 
            //interpolate_list.RemoveAll(r => r.TS > nonInterpolate_list[nonInterpolate_list.Count - 1].TS);



            //PrintAllPoint(PrintMode.Raw, InterpolationType.Non);



            //interpolationType = InterpolationType.Cubic;






            //PrintAllPoint(PrintMode.Interpolated, interpolationType);


            //PrintAllPoint(PrintMode.MapMatched, interpolationType, _dataRepository);

            //PrintAllPoint(InterpolationType.Quadratic);



        }

        private void BatchPreProcessing(Device device, Trip trip, bool isLastSet)
        {

            // Interpolate & Map-matching
            Interpolate(interpolationType);

            // Map matching
            Geo_Referencing(mapMatch_BatchSize);


            if (!isLastSet)
            {
                // Do not save the last track postion in the Db now
                allPoints.Remove(allPoints.ElementAt(allPoints.Count - 1));
            }

           // save in db
            _measurementRepository.AddAll(allPoints, device.DeviceId, trip.TripId);
        }

        private  Measurement CreateMeasurement(AutoPi_Json autoPi, Event_Type GPS_Type = Event_Type.Others)
        {
            Measurement measurement = new Measurement();          
            MeasurementType mType = new MeasurementType();


            String requestMeasurementType = string.Empty;
           
            //Check if MeasurementType exists in MeasurmentType table, if not add it

            if (GPS_Type == Event_Type.FristGPS_Trip || GPS_Type == Event_Type.LastGPS_Trip)
            {
                requestMeasurementType = "track.pos";               
            }

            else
            {
                requestMeasurementType = autoPi.T;
            }

            var _mType = _measurementTypeRepository.GetByName(requestMeasurementType);
           
            if (_mType == null || _mType.type == string.Empty)
            {

                // if does not exist
                var lastMType = _measurementTypeRepository.GetLastItemId();

                if (lastMType != null)
                {
                    mType.MeasurementTypeId = lastMType.MeasurementTypeId + 1;
                }

                else
                {
                    mType.MeasurementTypeId = 1;
                 
                }

                 mType.type = requestMeasurementType;
                // Add and Save
                _measurementTypeRepository.Add(mType);


            }

            else
            {
                mType = _mType;
            }



            switch (GPS_Type)
            {

                case Event_Type.FristGPS_Trip:

                    measurement.Measurement_Type = mType;
                    measurement.T = mType.type;
                    measurement.isComputed = false;
                    measurement.lon = (float?)(Convert.ToDouble(autoPi.StartPositionLng));  
                    measurement.lat = (float?)(Convert.ToDouble(autoPi.StartPositionLat)); 
                   // measurement.TS = (DateTimeOffset) autoPi.StartTimeUtc;
                    measurement.TS_or_Distance = DateTimeOffset.Parse(autoPi.StartTimeUtc.ToString("yyyy-MM-ddThh:mm:ss.ffffffZ" ));
                    break;
                case Event_Type.LastGPS_Trip :
                    measurement.Measurement_Type = mType;
                    measurement.T = mType.type;
                    measurement.isComputed = false;
                    measurement.lon = (float?)(Convert.ToDouble(autoPi.EndPositionLng));
                    measurement.lat = (float?)(Convert.ToDouble(autoPi.EndPositionLat));
                   // measurement.TS = (DateTimeOffset)autoPi.EndTimeUtc;
                    measurement.TS_or_Distance = DateTimeOffset.Parse(autoPi.EndTimeUtc.ToString("yyyy-MM-ddThh:mm:ss.ffffffZ"));
                    break;
                default:
                    if (autoPi.T == "track.pos")
                    {
                        measurement.isComputed =  false;
                        measurement.lon = (float?)(Convert.ToDouble(autoPi.TrackPosLoc.Lon));
                        measurement.lat = (float?)(Convert.ToDouble(autoPi.TrackPosLoc.Lat));
                    }
                    else
                    {
                        measurement.isComputed = true;
                    }

                    measurement.Measurement_Type = mType;
                    measurement.T = mType.type;
                    measurement.TS_or_Distance = DateTimeOffset.Parse(autoPi.Ts.ToString("yyyy-MM-ddThh:mm:ss.ffffffZ"));

                    break;
            }


            //measurement.MeasurementId = Guid.NewGuid();
            measurement.Created_Date = DateTime.Now;
            //measurement.UId = autoPi.Uid;
            //measurement.VId = autoPi.Vid;
           // measurement.Rec = autoPi.Rec;
            measurement.Measurement_Type = mType;
            measurement.message = Newtonsoft.Json.JsonConvert.SerializeObject(autoPi);


          
            return measurement;
        }

        //public async Task Start_JSON_RawData_Pipline(List<AutoPi_Json> sensorData)
        //{

        //    int eventNo = sensorData.Count;
        //    int FirstGPSIndex = -1;
        //    int LastNonGPSIndex = -1;
        //    int trackPosition_No = 0;
        //    int i = 0;

        //    // Skip first Non-Track Position events
        //    try
        //    {
        //        while (sensorData[i].T != "track.pos")
        //        {
        //            i++;
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }


        //    while (i < eventNo)
        //    {
        //        Measurement point = new Measurement();
        //        point.LiRA_Id = Guid.NewGuid().ToString();
        //        point.UId = sensorData[i].Uid.ToString();
        //        point.VId = sensorData[i].Vid.ToString();
        //        point.TS = DateTimeOffset.Parse( sensorData[i].Ts.ToString("yyyy-MM-ddThh:mm:ss.ffffffZ"));
        //        point.T = sensorData[i].T;
        //        point.Rec = sensorData[i].Rec;
        //        point.message = Newtonsoft.Json.JsonConvert.SerializeObject(sensorData[i]);

        //        // Points with actual GPS coordinates
        //        if (sensorData[i].TrackPosLoc != null)
        //        {
        //            point.lat = sensorData[i].TrackPosLoc.Lat;
        //            point.lon = sensorData[i].TrackPosLoc.Lon;
        //            point.isComputed = false;


        //            trackPosition_No++;
        //            nonInterpolate_list.Add(point);

        //            if (FirstGPSIndex == -1)
        //            {
        //                FirstGPSIndex = i;
        //            }
        //            else
        //            {
        //                LastNonGPSIndex = i - 1;
        //            }
        //        }

        //        else if (FirstGPSIndex != -1)  //Pointes which require interpolation but we do not do backwards interpolation 
        //        {

        //            point.isComputed = true;
        //            interpolate_list.Add(point);

        //        }

        //        allPoints.Add(point);

        //        try
        //        {
        //            _dataRepository.Add(point);
        //        }
        //        catch (Exception ex)
        //        {

        //            throw ex;
        //        }

        //        i++;


        //        // If enough number of Track position events are received, the interpolation and map-matching will be in placed:

        //        //if (trackPosition_No == Read_TrackPosition_Size)
        //        //{


        //        //    // Interpolation
        //        //    Interpolate(Interpolation_Type);


        //        //    // Map matching
        //        //    Geo_Referencing(mapMatch_BatchSize);



        //        //}

        //    }


        //    ////maybe temporary: removing points which their timestamp is after visiting the last actual GPS point 
        //    //interpolate_list.RemoveAll(r => r.TS > nonInterpolate_list[nonInterpolate_list.Count - 1].TS);



        //    //PrintAllPoint(PrintMode.Raw, InterpolationType.Non);



        //    //interpolationType = InterpolationType.Cubic;






        //    //PrintAllPoint(PrintMode.Interpolated, interpolationType);


        //    //PrintAllPoint(PrintMode.MapMatched, interpolationType, _dataRepository);

        //    //PrintAllPoint(InterpolationType.Quadratic);



        //  }

        public async Task Start_JSON_RawData_Pipline(RawAutoPiSensor sensorData)
        {



            int eventNo = sensorData.Hits.HitsHits.Length;


            int FirstGPSIndex = -1;
            int LastNonGPSIndex = -1;


            try
            {
                for (int i = 0; i < eventNo; i++)
                {


                    Measurement point = new Measurement();
                    point.MeasurementId = Guid.NewGuid();
                    point.TS_or_Distance = sensorData.Hits.HitsHits[i].Source.Ts;
                    point.T = sensorData.Hits.HitsHits[i].Source.T;
                    point.message = Newtonsoft.Json.JsonConvert.SerializeObject(sensorData.Hits.HitsHits[i].Source);


                    // Points with actual GPS coordinates
                    if (sensorData.Hits.HitsHits[i].Source.TrackPosLoc != null)
                    {
                        point.lat = sensorData.Hits.HitsHits[i].Source.TrackPosLoc.Lat;
                        point.lon = sensorData.Hits.HitsHits[i].Source.TrackPosLoc.Lon;
                        point.isComputed = false;

                        nonInterpolate_list.Add(point);

                        if (FirstGPSIndex == -1)
                        {
                            FirstGPSIndex = i;

                        }
                        else
                        {
                            LastNonGPSIndex = i - 1;
                        }
                    }

                    else if (FirstGPSIndex != -1)  //Pointes which require interpolation but we do not do backwards interpolation 
                    {

                        point.isComputed = true;

                        interpolate_list.Add(point);

                    }

                    allPoints.Add(point);
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            //maybe temporary: removing points which their timestamp is after visiting the last actual GPS point 
            interpolate_list.RemoveAll(r => r.TS_or_Distance > nonInterpolate_list[nonInterpolate_list.Count - 1].TS_or_Distance);


            PrintAllPoint(PrintMode.Raw, InterpolationType.Non);



            interpolationType = InterpolationType.Cubic;
            Interpolate(interpolationType);
            PrintAllPoint(PrintMode.Interpolated, interpolationType);


            // Map matching
            Geo_Referencing(mapMatch_BatchSize);
            PrintAllPoint(PrintMode.MapMatched, interpolationType);

            //PrintAllPoint(InterpolationType.Quadratic);

        }



        List<MatchResponse> mapMatched_response = new List<MatchResponse>();

        private int mapMatch_counter = -1;

        private void Geo_Referencing(int mm_BatchSize)
        {

            mapMatch_counter = 0;
            //Location[] mapMatchingArray = new Location[interpolate_list.Count];
            List<Location> mapMatchingList = new List<Location>();

            int fromIndex = -1, toIndex = -1;
            MatchRequest match_Request = new MatchRequest();
            int s = 0;
            for (int i = 0; i < allPoints.Count(); i++)
            {


                if (allPoints.ElementAt(i).lat != null && allPoints.ElementAt(i).lon != null)
                {
                    if (mapMatchingList.Count() == 0) fromIndex = i;

                    Location x = new Location((double)allPoints.ElementAt(i).lat, (double)allPoints.ElementAt(i).lon);
                    mapMatchingList.Add(x);

                    s++;
                    // int d = Math.DivRem(i, 10, out s);
                    //for the quafficient of 100, and also the remainning points
                    if (    (s == mm_BatchSize)      ||       ( s < mm_BatchSize && i == allPoints.Count() - 1 && s > 1 )    )
                    {

                        try
                        {
                            string ex1 = "";
                            toIndex = i;
                            //mapMatchingArray = mapMatchingList.ToArray();

                            match_Request.Coordinates = mapMatchingList.ToArray(); ;
                            MatchResponse match_Response_Json;
                            try
                            {
                                match_Response_Json = _mapMatchingService.Match(match_Request.Coordinates);
                            }
                            catch (Exception ex)
                            {
                                match_Response_Json = null;
                                ex1 = ex.Message.ToString();
                            }


                            Geo_Alighnment(match_Response_Json, match_Request, fromIndex, toIndex, ex1);

                            mapMatchingList.Clear();

                            //if (match_Response_Json != null)
                            //{

                            //}

                            //else
                            //{
                            //    Console.WriteLine(fromIndex + toIndex );
                            //}

                            //we add next batch if json was null
                            s = 0;

                        }
                        catch (Exception ex)
                        {


                        }
                    }


                    //else if ()
                    //{ 

                    //}

                }

            }


        }

        //private void Geo_Referencing(int mm_BatchSize)
        //{

        //    mapMatch_counter = 0;
        //    //Location[] mapMatchingArray = new Location[interpolate_list.Count];
        //    List<Location> mapMatchingList = new List<Location>();
         
        //    int fromIndex = -1, toIndex = -1;
        //    MatchRequest match_Request = new MatchRequest();
        //    int s = 0;
        //    for (int i = 0; i < allPoints.Count(); i++)
        //    {
                

        //        if (allPoints[i].lat != null && allPoints[i].lon != null)
        //        {
        //            if (mapMatchingList.Count() == 0) fromIndex = i;
                    
        //            Location x = new Location((double)allPoints[i].lat, (double)allPoints[i].lon);
        //            mapMatchingList.Add(x);

        //            s++;
        //            // int d = Math.DivRem(i, 10, out s);
        //            if (s == mm_BatchSize)
        //            {

        //                try
        //                {
        //                    string ex1 = "";
        //                    toIndex = i;
        //                    //mapMatchingArray = mapMatchingList.ToArray();

        //                    match_Request.Coordinates = mapMatchingList.ToArray(); ;
        //                    MatchResponse match_Response_Json;
        //                    try
        //                    {
        //                        match_Response_Json = _mapMatchingService.Match(match_Request.Coordinates);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        match_Response_Json = null;
        //                         ex1 = ex.Message.ToString();
        //                    }

                            
        //                    Geo_Alighnment(match_Response_Json, match_Request, fromIndex, toIndex, ex1);

        //                    mapMatchingList.Clear();

        //                    if (match_Response_Json != null)
        //                    {
                                

                     
        //                    }

        //                    //else
        //                    //{
        //                    //    Console.WriteLine(fromIndex + toIndex );
        //                    //}

        //                    //we add next batch if json was null
        //                    s = 0;

        //                }
        //                catch (Exception ex)
        //                {

                         
        //                }
        //            }

        //        }

        //    }

  
        //}


        private void Geo_Alighnment(MatchResponse match_Response, MatchRequest match_Request, int fromIndex, int toIndex, string ex1 = "")
        {

            
            mapMatched_response.Add(match_Response);

            MapReference roadRef;

            if (match_Response != null)
            {
                try
                {
                    int legIndex = -1;
                    for (int i = 0; i < match_Response.Tracepoints.Count(); i++)
                    {

                        roadRef = new MapReference();

                        mapMatch_counter ++;

                        legIndex++;
                        roadRef.lat_MapMatched = (match_Response.Tracepoints[i] != null) ? (float)match_Response.Tracepoints[i].Location.Latitude : -1;
                        roadRef.lon_MapMatched = (match_Response.Tracepoints[i] != null) ? (float)match_Response.Tracepoints[i].Location.Longitude : -1;
                        roadRef.wayPointName = (match_Response.Tracepoints[i] != null) ? match_Response.Tracepoints[i].Name : "";
                        
                        
                        allPoints.ElementAt(fromIndex).MapReferences = roadRef;

                     //allPoints[fromIndex].legSummary_GeoReferenced = match_Response.Matchings[(int)match_Response.Tracepoints[i].MatchingsIndex].Legs[legIndex - 1].Summary; 
                     // allPoints[fromIndex].legDistance_GeoReferenced = match_Response.Matchings[(int)match_Response.Tracepoints[i].MatchingsIndex].Legs[legIndex - 1].Summary;
                     // allPoints[fromIndex].nodeId_GeoReferenced = 
                     // allPoints [fromIndex].roadName_GeoReferenced = match_Response.Tracepoints[i]. match_Response.Tracepoints[i].MatchingsIndex

                        fromIndex++;

                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }

            else
            {

                for(int k = fromIndex; k <= toIndex ; k++)
                {
                    string path = @"..\PullingRawDataService\bin\Bad1Request_MAPMatch_AllPoint" + ".txt";

                    try
                    {
                        // Create a file to write to.
                        //if (!File.Exists(path))
                        //{
                        //    File.Create(path);

                        //    //StreamWriter sw = new StreamWriter(path);
                        //}
                        //string filePath = @"c:\somefilename.txt";
                        using (StreamWriter sw = new StreamWriter(path, true))
                        {

                        //    using (StreamWriter sw = File.AppendText(path))
                        //{
                            mapMatch_counter++;

                            sw.Write(mapMatch_counter + ", ");

                            sw.Write(k + ", ");

                            sw.Write(fromIndex + " to ");
                            sw.Write(toIndex + ", ");
                            sw.Write(allPoints.ElementAt(k).lat.Value.ToString("G9") + " , ");
                            sw.Write(allPoints.ElementAt(k).lon.Value.ToString("G9") + " , ");

                            if (allPoints.ElementAt(k).MapReferences.lat_MapMatched != null) sw.Write(allPoints.ElementAt(k).MapReferences.lat_MapMatched.Value.ToString("G9") + " , ");
                            if (allPoints.ElementAt(k).MapReferences.lon_MapMatched != null) sw.Write(allPoints.ElementAt(k).MapReferences.lon_MapMatched.Value.ToString("G9") + " , ");
                            if (allPoints.ElementAt(k).MapReferences.wayPointName != null) sw.Write(allPoints.ElementAt(k).MapReferences.wayPointName.ToString());

                            sw.Write(allPoints.ElementAt(k).isComputed + " , ");

                            //sw.Write(dt + ",");
                            var dt = allPoints.ElementAt(k).TS_or_Distance.ToString("yyyy-MM-dd HH:mm:ss");

                            sw.Write(ex1);

                            sw.WriteLine();

                            sw.Close();
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                 }


            }
        }

            

           

        

        private static void Pull_NonGPS_Events(IMongoDatabase mongoDataBase, List<string> collectionNames, FilterDefinitionBuilder<BsonDocument> filterBuilder, FilterDefinition<BsonDocument> filter_Device)
        {
            
            for (int j = 0; j < SensorReader.nonInterpolate_list.Count - 1; j++ )
            {

                //var filter_timestamp = filterBuilder.Gte("@ts", SensorReader.nonInterpolate_list[j].ts_temp) &
                //                                filterBuilder.Lt("@ts", SensorReader.nonInterpolate_list[j + 1].ts_temp);
                //var final_filter = filter_Device & filter_timestamp;


                //Reading events from other collections:
                //inter_list = NonGPSEvents_FromTo(mongoDataBase, collectionNames, final_filter);

                // Sort events
                Sort_nonGPS_Events();

                // Save to allPoints list in order
                SaveTo_AllPoints(j);

                SensorReader.interpolate_list.AddRange(inter_list);


                inter_list.Clear();

            }

            //  var filter_timestamp = filterBuilder.Gte("@ts", batch_TrachPostions.ElementAt(i).GetElement("@ts").Value) &
            //                                                 filterBuilder.Lt("@ts", batch_TrachPostions.ElementAt(i + 1).GetElement("@ts").Value);


        }

        private static void SaveTo_AllPoints(int j)
        {
            allPoints.Add(SensorReader.nonInterpolate_list[j]);
           //TODO allPoints.AddRange(inter_list);
            allPoints.Add(SensorReader.nonInterpolate_list[j + 1]);
        }

        private static void Sort_nonGPS_Events()
        {
            Console.WriteLine();
            //Sorting all  non_GPS events 
            if (inter_list.Count > 0)
            {

               //TODO inter_list.Sort((x1, x2) => x1.TS.CompareTo(x2.TS));
            }
        }

        private static ICollection<Measurement> NonGPSEvents_FromTo(IMongoDatabase mongoDataBase, List<string> collectionNames, FilterDefinition<BsonDocument> final_filter)
        {

            int collectinoCounter = 0;
            foreach (string collection in collectionNames)
            {

                collectinoCounter++;

                try
                {
                    if (collection == "acc.xyz")
                    {
                        int xxxx = 0;
                    }

                    else
                    {

                        IEnumerable<BsonDocument> events_Collection = mongoDataBase.GetCollection<BsonDocument>(collection).Find(final_filter).ToList();

                        foreach (var item in events_Collection)
                        {
                            //Make list for interpolations
                            Measurement interpolated_point = new Measurement();
                            //interpolated_point.UId = item.GetElement("_id").Value.ToString();
                            //interpolated_point.TS = SensorReader.FromString(item.GetElement("@ts").Value.AsString);
                            //interpolated_point.T = item.GetElement("@t").Value.AsString;
                            //interpolated_point.VId = item.GetElement("@token").Value.AsString;

                            interpolated_point.message = item.ToJson();
                            interpolated_point.MeasurementId = Guid.NewGuid();
                            interpolated_point.isComputed = true;
                            //point.lat = (float?)(Convert.ToDouble(events_Collection.ElementAt(i).GetElement("loc").Value.AsBsonDocument.GetElement("lat").Value));
                            //point.lon = (float?)(Convert.ToDouble(events_Collection.ElementAt(i).GetElement("loc").Value.AsBsonDocument.GetElement("lon").Value));

                            
                            inter_list.Add(interpolated_point);                 



                        }
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }


            }
                       

            return inter_list;
        }

        public static void CleanData(Queue eventQueue)
        {
            // get the first event in the queue
            while (eventQueue.Count != 0 )
            {
                Measurement GPS_point = (Measurement)eventQueue.Peek();
                var d = nonInterpolate_list.Find(x => x.TS_or_Distance == GPS_point.TS_or_Distance);
                if (d == null)
                {
                    // thinking about every time we should empty the lsit after interpolation or not, which list to send and use?????
                    nonInter_list.Add(GPS_point);
                    SensorReader.nonInterpolate_list.Add(GPS_point);

                    PrintPoint_Console(GPS_point, "non");

                }

                else
                {
                    //Adding point
                    SensorReader.allPoints.Add(GPS_point);
                }

                eventQueue.Dequeue(); 
            }
        }

        private static void CreateGPSEvents_AddQueue(Queue eventQueue, IEnumerable<BsonDocument> batch_TrachPostions)
        {
            // Here we have retreived 3 numbers of track positions 
            for (int i = 0; i < batch_TrachPostions.Count(); i++)

            {
                //Create GPS points
                Measurement GPS_point = Create_GPS_Point(batch_TrachPostions, i);

                PrintPoint_Console(GPS_point, "all");

                eventQueue.Enqueue(GPS_point);


            }
        }

        private static void PrintPoint_Console(Measurement GPS_point, string prefix)
        {
            Console.Write(prefix + " ");
            Console.Write(allPoints.Contains(GPS_point).ToString() + " ");
            //Console.Write(GPS_point.UId.ToString() + " ");
            Console.Write(GPS_point.lat.ToString() + " ");
            Console.Write(GPS_point.lon.ToString() + " ");
            Console.WriteLine(GPS_point.TS_or_Distance.ToString());
        }

        private void PreReading(out IMongoDatabase mongoDataBase, out List<BsonDocument> devices_List, out IMongoCollection<BsonDocument> track_pos_collection, out List<string> collectionNames, out FindOptions<BsonDocument> options)
        {
            //string StringSerializationFormat = "yyyy-MM-ddTHH:mm:ss.ffffff";
            mongoDataBase = _mongoClient.GetDatabase("lira");

            //Pulling based on "Devices" and "Track Position Events"
            var devices_collection = mongoDataBase.GetCollection<BsonDocument>("devices");
            devices_List = devices_collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
            track_pos_collection = mongoDataBase.GetCollection<BsonDocument>("track.pos");
            var track_pos_List = track_pos_collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();

            track_pos_List.Sort();

            collectionNames = mongoDataBase.ListCollectionNames().ToList();
            collectionNames.Remove("devices");
            collectionNames.Remove("track.pos");

            //filter_Device = filter_Device & filterBuilder.Eq("@token", "token e227bddc-574d-40ba-91cc-1803f638065e");

            BsonDocument sort = new BsonDocument();
            sort.Add("@ts", 1.0);
            options = new FindOptions<BsonDocument>
            {
                BatchSize = 3,
                NoCursorTimeout = false,
                Sort = sort
            };
        }

       

            //var xyz_collection = mongoDataBase.GetCollection<BsonDocument>("acc.xyz");

            //read by batch cosidering the filtering
            ////var j =  documents.ToJson();

            ////var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            ////JObject json = JObject.Parse(documents.ToJson<MongoDB.Bson.BsonDocument>(jsonWriterSettings));

            //// Newtonsoft.Json.JsonConvert.DeserializeObject(BsonTypeMapper.MapToDotNetValue(j));

            //using (IAsyncCursor<BsonDocument> cursor = await collection.Find(new BsonDocument()))
            //{
            //    while (await cursor.MoveNextAsync())
            //    {
            //        IEnumerable<BsonDocument> batch = cursor.Current;
            //        foreach (BsonDocument document in batch)
            //        {
            //            Console.WriteLine(document);
            //            Console.WriteLine();
            //        }
            //    }
            //}

            // documents.ConvertAll(BsonTypeMapper.MapToDotNetValue);


            //List<CSVAutoPiSource> sensorData = Newtonsoft.Json.JsonConvert.DeserializeObject<CSVAutoPiSource>(j);

            //var filterBuilder = Builders<BsonDocument>.Filter;
            //var filter = filterBuilder.Lte(x => Convert.ToDateTime(x.GetElement("@ts").Value), Convert.ToDateTime(documents[0].GetElement("@ts").Value));
            //// filterBuilder.Lte(x => x.GetElement("ts").Value, new ));



            //var xyz_collection = mongoDataBase.GetCollection<BsonDocument>("acc.xyz");
            //var document = xyz_collection.Find(filter).ToList();


            //var results =  mongoDataBase.GetCollection<BsonDocument>("acc.xyz").Find(filter).ToList();


            // var query = mongoDataBase.GetCollection<BsonDocument>("acc.xyz").AsQueryable().Join(_db.GetCollection<Item>("Item"), product => product._id, item => item._id, (product, item) => product;


            //   mongoDataBase.GetCollection<BsonDocument>("acc.xyz").Find( x => x.);                    


            //  ({ ts: {$gt: 2} }).forEach(printjson);








            // var all = mongoDataBase.ListCollectionNames().;

            //foreach (var i in all)
            // {

            // }

            // foreach (BsonDocument device in devices_collection)
            // {
            //     foreach (var trackPostion in track_pos_collection)
            //     {

            //     }
            // }





            // var r = collection.Find(new BsonDocument()).ToJson();// Load all documents of that collection



          //  List<CSVSampleSource> collectionN = new List<CSVSampleSource>();





                //using (var cursor = await mongoClient.ListDatabaseNames())
                //{
                //    await cursor.ForEachAsync(d => Console.WriteLine(d.ToString()));
                //}                            
                //var db = mongoClient.GetDatabase("admin");
                //foreach (var item in db.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result)
                //{
                //    Console.WriteLine(item.ToString());
                //}
                //collection.ToJson();

                //System.IO.File.WriteAllText("file.txt",// path where you want to save the result
                //                             r.Aggregate<BsonDocument, string>("", (seed, document) => seed + document.ToString() + "\n"));





            

           


            //MongoClientSettings settings = new MongoClientSettings();
            //settings.Server = new MongoServerAddress(credential.Host, Convert.ToInt32(credential.Port));
            //MongoIdentity internalIdentity = new MongoInternalIdentity("lira", credential.Username);
            //PasswordEvidence passwordEvidence = new PasswordEvidence(credential.Password);
            //string mongoDbAuthMechanism = "SCRAM-SHA-1";
            //MongoCredential mongoCredential =
            //        new MongoCredential(mongoDbAuthMechanism, internalIdentity, passwordEvidence);
            //settings.Credential = mongoCredential;
            //settings.UseTls   = true;
            //settings.SslSettings = new SslSettings();
            //settings.SslSettings.CheckCertificateRevocation = false;
            //settings.ConnectionMode = MongoDB.Driver.ConnectionMode.Automatic;

            // _mongoClient = new MongoClient(settings);

            //_mongoClient = mongoClient;

       // }

        private static Measurement Create_GPS_Point(IEnumerable<BsonDocument> batch_TrachPostions, int i)
        {
            //Measurement GPS_point = new Measurement();    
            //GPS_point.UId = batch_TrachPostions.ElementAt(i).GetElement("_id").Value.ToString();
            //GPS_point.VId = batch_TrachPostions.ElementAt(i).GetElement("@ts").Value.AsString;
            //GPS_point.TS = SensorReader.FromString(batch_TrachPostions.ElementAt(i).GetElement("@ts").Value.AsString);
            //GPS_point.ts_temp = batch_TrachPostions.ElementAt(i).GetElement("@ts").Value.AsString;
            //GPS_point.T = batch_TrachPostions.ElementAt(i).GetElement("@t").Value.AsString;
            //GPS_point.VId = batch_TrachPostions.ElementAt(i).GetElement("@token").Value.AsString;

            //GPS_point.message = batch_TrachPostions.ElementAt(i).ToJson();
            //GPS_point.LiRA_Id = Guid.NewGuid().ToString();
            //GPS_point.isComputed = false;
            //GPS_point.lat = (float?)(Convert.ToDouble(batch_TrachPostions.ElementAt(i).GetElement("loc").Value.AsBsonDocument.GetElement("lat").Value));
            //GPS_point.lon = (float?)(Convert.ToDouble(batch_TrachPostions.ElementAt(i).GetElement("loc").Value.AsBsonDocument.GetElement("lon").Value));
            //return GPS_point;
            return null;
        }

        private static void Pull_NonGPS_Points(IMongoDatabase mongoDataBase, List<string> collectionNames, FilterDefinitionBuilder<BsonDocument> filterBuilder, FilterDefinition<BsonDocument> filter_Device, Measurement GPS_point)
        {
            //  var filter_timestamp = filterBuilder.Gte("@ts", batch_TrachPostions.ElementAt(i).GetElement("@ts").Value) &
            //                                                 filterBuilder.Lt("@ts", batch_TrachPostions.ElementAt(i + 1).GetElement("@ts").Value);
            //var filter_timestamp = filterBuilder.Gte("@ts", SensorReader.nonInterpolate_list[SensorReader.nonInterpolate_list.Count - 2].ts_temp) &
            //                                                filterBuilder.Lt("@ts", SensorReader.nonInterpolate_list[SensorReader.nonInterpolate_list.Count - 1].ts_temp);
            //var final_filter = filter_Device & filter_timestamp;

            int collectinoCounter = 0;

            //Reading events from other collections:
            foreach (string collection in collectionNames)
            {

                collectinoCounter++;

                try
                {
                    if (collection == "acc.xyz")
                    {
                        int xxxx = 0;
                    }

                    else
                    {

                        //IEnumerable<BsonDocument> events_Collection = mongoDataBase.GetCollection<BsonDocument>(collection).Find(final_filter).ToList();

                        //foreach (var item in events_Collection)
                        //{
                        //    //Make list for interpolations
                        //    Measurement interpolated_point = new Measurement();
                        //    //interpolated_point.UId = item.GetElement("_id").Value.ToString();
                        //    //interpolated_point.TS = SensorReader.FromString(item.GetElement("@ts").Value.AsString);
                        //    //interpolated_point.T = item.GetElement("@t").Value.AsString;
                        //    //interpolated_point.VId = item.GetElement("@token").Value.AsString;

                        //    interpolated_point.message = item.ToJson();
                        //    interpolated_point.MeasurementId = Guid.NewGuid();
                        //    interpolated_point.isComputed = true;
                        //    //point.lat = (float?)(Convert.ToDouble(events_Collection.ElementAt(i).GetElement("loc").Value.AsBsonDocument.GetElement("lat").Value));
                        //    //point.lon = (float?)(Convert.ToDouble(events_Collection.ElementAt(i).GetElement("loc").Value.AsBsonDocument.GetElement("lon").Value));



                        //    inter_list.Add(interpolated_point);



                        //}
                    }

                }
                catch (Exception ex)
                {

                    throw ex;
                }


            }



            Console.WriteLine();

            if (inter_list.Count > 0)
            {
               //TODO inter_list.Sort((x1, x2) => x1.TS.CompareTo(x2.TS));
               //TODO allPoints.AddRange(inter_list);
                SensorReader.interpolate_list.AddRange(inter_list);

            }

            inter_list.Clear();
            //Adding Actual GPS point after the non-Actual points in between
            allPoints.Add(GPS_point);
            // For every batch, we do the interpolation 
            //SensorReader.interpolationType = InterpolationType.Quadratic;
            //SensorReader.Interpolate(SensorReader.interpolationType);
            //SensorReader.Print(SensorReader.interpolationType);

        }

        private static T ProcessBsonConversion<T>(BsonDocument data)
        {
            var content = data.GetElement("_v");
            var jsonDataContent = content.Value.AsBsonValue.ToJson();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonDataContent);

        }



    }
}

