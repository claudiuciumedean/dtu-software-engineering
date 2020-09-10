using LiRACore.Models;
using LiRACore.Models.RawData;
using LiRACore.Services.AutoPi.Interpolation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime;
using System.Linq;
using LiRACore.Models.RawData.Raw;
using LiRACore.Models.Repository;

namespace LiRACore.Services.AutoPi
{
    public  static class SensorReader
    {
        public enum RawDataReadType { JSON, CSV, GreenMobilityServer }
        public enum PrintMode { Raw, Interpolated, MapMatched, All}
        public enum InterpolationType { Linear, Quadratic, Cubic, Non };

        public enum Event_Type { FristGPS_Trip, LastGPS_Trip, BetweenGPS_Trip, Others}

        public static InterpolationType interpolationType;

        public static ICollection<Measurement> allPoints = new List<Measurement>();     
        public static List<float> x_list = new List<float>();
        public static List<float> y_list = new List<float>();
        public static List<DateTimeOffset> t_list = new List<DateTimeOffset>();


        public static List<Measurement> nonInterpolate_list = new List<Measurement>();
        public static List<Measurement> interpolate_list = new List<Measurement>();
        public static List<float> xs_list = new List<float>();


       
            public static DateTimeOffset FromString(string offsetString)
            {

                DateTimeOffset offset;
                if (!DateTimeOffset.TryParse(offsetString, out offset))
                {
                    offset = DateTimeOffset.Now;
                }

                return offset;
            }
        


        public  static void ReadSensorDataJSON(RawAutoPiSensor sensorData)
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
                    point.message = JsonConvert.SerializeObject(sensorData.Hits.HitsHits[i].Source);


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


            
            interpolationType = InterpolationType.Quadratic;
            Interpolate(interpolationType);
            PrintAllPoint(PrintMode.Interpolated, InterpolationType.Quadratic);

            //Print(interpolationType);



        }

        public static void ReadSensorDataCSV(RawAutoPiSensor sensorData)
        {

      

            using (var reader = new StreamReader(@"C:\test.csv"))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }
            }


            //int eventNo = sensorData.Hits.HitsHits.Length;


            //int FirstGPSIndex = -1;
            //int LastNonGPSIndex = -1;


            //for (int i = 0; i < eventNo; i++)
            //{


            //    dynamic point = new Interpolated();
            //    point.primaryKey = Guid.NewGuid();
            //    point.ts = sensorData.Hits.HitsHits[i].Source.Ts;
            //    point.tag = sensorData.Hits.HitsHits[i].Source.T;
            //    point.message = JsonConvert.SerializeObject(sensorData.Hits.HitsHits[i].Source);


            //    // Points with actual GPS coordinates
            //    if (sensorData.Hits.HitsHits[i].Source.TrackPosLoc != null)
            //    {
            //        point.lat = sensorData.Hits.HitsHits[i].Source.TrackPosLoc.Lat;
            //        point.lon = sensorData.Hits.HitsHits[i].Source.TrackPosLoc.Lon;
            //        point.isInterpolated = false;

            //        nonInterpolate_list.Add(point);

            //        if (FirstGPSIndex == -1)
            //        {
            //            FirstGPSIndex = i;

            //        }
            //        else
            //        {
            //            LastNonGPSIndex = i - 1;
            //        }
            //    }

            //    else if (FirstGPSIndex != -1)  //Pointes which require interpolation but we do not do backwards interpolation 
            //    {

            //        point.isInterpolated = true;

            //        interpolate_list.Add(point);

            //    }

            //    allPoints.Add(point);
            //}


            ////maybe temporary: removing points which their timestamp is after visiting the last actual GPS point 
            //interpolate_list.RemoveAll(r => r.ts > nonInterpolate_list[nonInterpolate_list.Count - 1].ts);

            //interpolationType = InterpolationType.Quadratic;
            //Interpolate(interpolationType);
            //print(interpolationType);

        }
        
        public static void ReadSensorDataGMServer(RawAutoPiSensor sensorData)
        {



            using (var reader = new StreamReader(@"C:\test.csv"))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }
            }


            //int eventNo = sensorData.Hits.HitsHits.Length;


            //int FirstGPSIndex = -1;
            //int LastNonGPSIndex = -1;


            //for (int i = 0; i < eventNo; i++)
            //{


            //    dynamic point = new Interpolated();
            //    point.primaryKey = Guid.NewGuid();
            //    point.ts = sensorData.Hits.HitsHits[i].Source.Ts;
            //    point.tag = sensorData.Hits.HitsHits[i].Source.T;
            //    point.message = JsonConvert.SerializeObject(sensorData.Hits.HitsHits[i].Source);


            //    // Points with actual GPS coordinates
            //    if (sensorData.Hits.HitsHits[i].Source.TrackPosLoc != null)
            //    {
            //        point.lat = sensorData.Hits.HitsHits[i].Source.TrackPosLoc.Lat;
            //        point.lon = sensorData.Hits.HitsHits[i].Source.TrackPosLoc.Lon;
            //        point.isInterpolated = false;

            //        nonInterpolate_list.Add(point);

            //        if (FirstGPSIndex == -1)
            //        {
            //            FirstGPSIndex = i;

            //        }
            //        else
            //        {
            //            LastNonGPSIndex = i - 1;
            //        }
            //    }

            //    else if (FirstGPSIndex != -1)  //Pointes which require interpolation but we do not do backwards interpolation 
            //    {

            //        point.isInterpolated = true;

            //        interpolate_list.Add(point);

            //    }

            //    allPoints.Add(point);
            //}


            ////maybe temporary: removing points which their timestamp is after visiting the last actual GPS point 
            //interpolate_list.RemoveAll(r => r.ts > nonInterpolate_list[nonInterpolate_list.Count - 1].ts);

            //interpolationType = InterpolationType.Quadratic;
            //Interpolate(interpolationType);
            //print(interpolationType);

        }

        public static void Interpolate(InterpolationType interpolationType)
        {
            switch (interpolationType)
            {
                case InterpolationType.Quadratic:
                    QuadraticInterpolation();
                    break;
                case InterpolationType.Cubic:
                    QubicInterpolation();
                    break;
                default: //InterpolationType.Linear
                    LinearInterpolation();
                    break;
            }

        }


        //    public static void Interpolate(ref Interpolated point, bool isInterpolated)
        //    {
        //        //if is original GPS coordinate
        //        if (!isInterpolated)
        //        {
        //            x_list.Add((float)point.lat);
        //            y_list.Add((float)point.lon);
        //            t_list.Add((DateTimeOffset)point.ts);

        //            nonInterpolate_list.Add(point);
        //}

        //        else
        //        {
        //            interpolate_list.Add(point);
        //        }




        //        //double latest_Original_Lat;
        //        //double latest_Original_Lon;

        //        //double current_temp_Lat;
        //        //double current_temp_Lon;

        //        //bool found_first_GSP = false;

        //        //foreach (Interpolated point in allPoints)
        //        //{

        //        //    if (point.lat != null && point.lon != null)
        //        //    {
        //        //        latest_Original_Lat = Convert.ToDouble(point.lat);
        //        //        latest_Original_Lon = Convert.ToDouble(point.lon);

        //        //        found_first_GSP = true;


        //        //    }

        //        //    else if (found_first_GSP)
        //        //    {
        //        //        //  float yy = yy0 + (yy1 - yy0) * ((xx - xx0) / (xx1 - xx0));


        //        //    }
        //        //}



        //    }


        //public void ConstructSensorData(ref List<float> x_list, ref List<float> y_list, List<DateTimeOffset> t_list, ref List<DateTimeOffset>  time_interpolate_list )
        //{
        //    //Preparing float arrays for interpolation
        //    float[] x_ = new float[x_list.Count];
        //    float[] y_ = new float[y_list.Count];
        //    DateTimeOffset[] t_ = new DateTimeOffset[t_list.Count];
        //    DateTimeOffset[] time_interpolate = new DateTimeOffset[time_interpolate_list.Count];
        //    x_ = x_list.ToArray();
        //    y_ = y_list.ToArray();
        //    t_ = t_list.ToArray();

        //    time_interpolate_list.RemoveAll(r => r > t_list[t_list.Count - 1]);
        //    time_interpolate = time_interpolate_list.ToArray();


        //    float[] xs, ys;

        //    //Fit and eval for GPS coordinates => parametric
        //    CubicSpline.FitSensorDataGeometric(x_, y_, t_, time_interpolate, time_interpolate_list.Count, out  xs, out  ys);

        //  //  PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit", x_, y_, xs, ys, @"..\..\testSplineFitParametric.png");


        //    string path = "../interpolateData.txt";
        //    if (!File.Exists(path))
        //    {
        //        // Create a file to write to.
        //        using (StreamWriter sw = File.CreateText(path))
        //        {
        //            foreach (var item in x_)

        //            {
        //                sw.WriteLine(item.ToString());

        //            }

        //            sw.WriteLine("");

        //            foreach (var item in y_)

        //            {
        //                sw.WriteLine(item.ToString());

        //            }

        //            sw.WriteLine("");

        //            foreach (var item in xs)

        //            {
        //                sw.WriteLine(item.ToString());

        //            }

        //            sw.WriteLine("");

        //            foreach (var item in ys)

        //            {
        //                sw.WriteLine(item.ToString());

        //            }

        //        }
        //    }

        //    // Open the file to read from.
        //    using (StreamReader sr = File.OpenText(path))
        //    {
        //        string s = "";
        //        while ((s = sr.ReadLine()) != null)
        //        {
        //            Console.WriteLine(s);
        //        }
        //    }
        //}

        public static void LinearInterpolation()
        {
            int c = -1;
            for (int i = 0; i < nonInterpolate_list.Count - 1; i++)
            {
                foreach (Measurement point in interpolate_list)
                {
                    if (point.TS_or_Distance >= nonInterpolate_list[i].TS_or_Distance && point.TS_or_Distance <= nonInterpolate_list[i + 1].TS_or_Distance)
                    {
                        // float yy = yy0 + (yy1 - yy0) * ((xx - xx0) / (xx1 - xx0));

                        c = interpolate_list.IndexOf(point);
                        point.lon = (float?)(nonInterpolate_list[i].lon + (nonInterpolate_list[i + 1].lon - nonInterpolate_list[i].lon) * ((point.TS_or_Distance - nonInterpolate_list[i].TS_or_Distance) / (nonInterpolate_list[i + 1].TS_or_Distance - nonInterpolate_list[i].TS_or_Distance)));
                        point.lat = (float?)(nonInterpolate_list[i].lat + (nonInterpolate_list[i + 1].lat - nonInterpolate_list[i].lat) * ((point.TS_or_Distance - nonInterpolate_list[i].TS_or_Distance) / (nonInterpolate_list[i + 1].TS_or_Distance - nonInterpolate_list[i].TS_or_Distance)));
                        
                    }

                }

            }
            
        }
        
        public static void QuadraticInterpolation()
        {
            // http://mathonline.wikidot.com/deleted:quadratic-polynomial-interpolation

            int k = -1;
            int k0 = -1; int k1 = -1; int k2 = -1;

            for (int i = 0; i < nonInterpolate_list.Count; i++)
            {
                foreach (Measurement point in interpolate_list)
                {
                    if (point.TS_or_Distance >= nonInterpolate_list[i].TS_or_Distance && point.TS_or_Distance < nonInterpolate_list[i + 1].TS_or_Distance)
                    {

                        //k0 = i;
                        //k1 = i + 1;



                        if (i == 0 )
                        {
                            k = i;
                        }
                        else
                        {
                            if (i == nonInterpolate_list.Count - 1)
                            {
                                k = i - 2;
                            }
                            else
                            {   
                                k = i - 1;
                            }
                        }

                        //P2(x) = y0L0(x) + y1L1(x) + y2L2(x) = y0(x−x1)(x−x2)(x0−x1)(x0−x2) + y1(x−x0)(x−x2)(x1−x0)(x1−x2) + y2(x−x0)(x−x1)(x2−x0)(x2−x1)

                        float? X0_X1 = (float)(nonInterpolate_list[k].TS_or_Distance - nonInterpolate_list[k + 1].TS_or_Distance).TotalMilliseconds;
                        float? X0_X2 = (float)(nonInterpolate_list[k].TS_or_Distance - nonInterpolate_list[k + 2].TS_or_Distance).TotalMilliseconds;

                        float? X1_X0 = (float)(nonInterpolate_list[k + 1].TS_or_Distance - nonInterpolate_list[k].TS_or_Distance).TotalMilliseconds;
                        float? X1_X2 = (float)(nonInterpolate_list[k + 1].TS_or_Distance - nonInterpolate_list[k + 2].TS_or_Distance).TotalMilliseconds;

                        float? X2_X0 = (float)(nonInterpolate_list[k + 2].TS_or_Distance - nonInterpolate_list[k].TS_or_Distance).TotalMilliseconds;
                        float? X2_X1 = (float)(nonInterpolate_list[k + 2].TS_or_Distance - nonInterpolate_list[k + 1].TS_or_Distance).TotalMilliseconds;


                        float? L0 = (float?)( (point.TS_or_Distance - nonInterpolate_list[k + 1].TS_or_Distance).TotalMilliseconds * (point.TS_or_Distance - nonInterpolate_list[k + 2].TS_or_Distance).TotalMilliseconds) / ((X0_X1) * (X0_X2));                        
                        float? L1 = (float?)((point.TS_or_Distance - nonInterpolate_list[k].TS_or_Distance).TotalMilliseconds * (point.TS_or_Distance - nonInterpolate_list[k + 2].TS_or_Distance).TotalMilliseconds) / ((X1_X0) * (X1_X2));
                        float? L2 = (float?)((point.TS_or_Distance - nonInterpolate_list[k].TS_or_Distance).TotalMilliseconds * (point.TS_or_Distance - nonInterpolate_list[k + 1].TS_or_Distance).TotalMilliseconds) / ((X2_X0) * (X2_X1));

                        point.lon = (float?)nonInterpolate_list[k].lon * (L0) + nonInterpolate_list[k + 1].lon * (L1) + nonInterpolate_list[k + 2].lon * (L2);
                        point.lat = (float?)nonInterpolate_list[k].lat * (L0) + nonInterpolate_list[k + 1].lat * (L1) + nonInterpolate_list[k + 2].lat * (L2);
                        

                    }
                }
                }
             }

        public static void QuadraticInterpolation_New (int start_index_nonInterpolatedList = 0 )
        {
            // http://mathonline.wikidot.com/deleted:quadratic-polynomial-interpolation

            int k = -1;


            for (int i = start_index_nonInterpolatedList ; i < nonInterpolate_list.Count; i++)
            {
                foreach (Measurement point in interpolate_list)
                {
                    if (point.TS_or_Distance >= nonInterpolate_list[i].TS_or_Distance && point.TS_or_Distance <= nonInterpolate_list[i + 1].TS_or_Distance)
                    {


                        if (i == 0)
                        {
                            k = i;
                        }
                        else
                        {
                            if (i == nonInterpolate_list.Count - 1)
                            {
                                k = i - 2;
                            }
                            else
                            {
                                k = i - 1;
                            }
                        }

                        //P2(x) = y0L0(x) + y1L1(x) + y2L2(x) = y0(x−x1)(x−x2)(x0−x1)(x0−x2) + y1(x−x0)(x−x2)(x1−x0)(x1−x2) + y2(x−x0)(x−x1)(x2−x0)(x2−x1)

                        float? X0_X1 = (float)(nonInterpolate_list[k].TS_or_Distance - nonInterpolate_list[k + 1].TS_or_Distance).TotalMilliseconds;
                        float? X0_X2 = (float)(nonInterpolate_list[k].TS_or_Distance - nonInterpolate_list[k + 2].TS_or_Distance).TotalMilliseconds;

                        float? X1_X0 = (float)(nonInterpolate_list[k + 1].TS_or_Distance - nonInterpolate_list[k].TS_or_Distance).TotalMilliseconds;
                        float? X1_X2 = (float)(nonInterpolate_list[k + 1].TS_or_Distance - nonInterpolate_list[k + 2].TS_or_Distance).TotalMilliseconds;

                        float? X2_X0 = (float)(nonInterpolate_list[k + 2].TS_or_Distance - nonInterpolate_list[k].TS_or_Distance).TotalMilliseconds;
                        float? X2_X1 = (float)(nonInterpolate_list[k + 2].TS_or_Distance - nonInterpolate_list[k + 1].TS_or_Distance).TotalMilliseconds;


                        float? L0 = (float?)((point.TS_or_Distance - nonInterpolate_list[k + 1].TS_or_Distance).TotalMilliseconds * (point.TS_or_Distance - nonInterpolate_list[k + 2].TS_or_Distance).TotalMilliseconds) / ((X0_X1) * (X0_X2));
                        float? L1 = (float?)((point.TS_or_Distance - nonInterpolate_list[k].TS_or_Distance).TotalMilliseconds * (point.TS_or_Distance - nonInterpolate_list[k + 2].TS_or_Distance).TotalMilliseconds) / ((X1_X0) * (X1_X2));
                        float? L2 = (float?)((point.TS_or_Distance - nonInterpolate_list[k].TS_or_Distance).TotalMilliseconds * (point.TS_or_Distance - nonInterpolate_list[k + 1].TS_or_Distance).TotalMilliseconds) / ((X2_X0) * (X2_X1));

                        point.lon = (float?)nonInterpolate_list[k].lon * (L0) + nonInterpolate_list[k + 1].lon * (L1) + nonInterpolate_list[k + 2].lon * (L2);
                        point.lat = (float?)nonInterpolate_list[k].lat * (L0) + nonInterpolate_list[k + 1].lat * (L1) + nonInterpolate_list[k + 2].lat * (L2);


                    }
                }
            }
        }

        public static void QubicInterpolation()
        {
            List<Measurement> xs, ys;

            //Fit and eval for GPS coordinates => parametric
            CubicSpline.FitSensorDataGeometric(nonInterpolate_list, interpolate_list, out xs, out ys);
        }

        public static void Print(InterpolationType interpolationType)
        {

            string path = @"..\PullingRawDataService\bin\GPSPoints_" + interpolationType.ToString() + ".txt";

            // Create a file to write to.
            if (!File.Exists(path))
            {
                File.Create(path);

                //StreamWriter sw = new StreamWriter(path);
            }
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (Measurement item in nonInterpolate_list)

                    {
                    var dt = item.TS_or_Distance.ToString("yyyy-MM-dd HH:mm:ss");
                        sw.Write(dt + ",");
                        sw.Write(item.lat.Value.ToString("G9") + ",");
                        sw.WriteLine(item.lon.Value.ToString("G9"));
                    }
                }

            

            path = @"..\PullingRawDataService\bin\ComputedPoints_" + interpolationType.ToString() + ".txt";

            // Create a file to write to.
            if (!File.Exists(path))
            {
               File.Create(path);

               // StreamWriter sw = new StreamWriter(path);
            }
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (Measurement item in interpolate_list)

                    {
                    var dt = item.TS_or_Distance.ToString("yyyy-MM-dd HH:mm:ss");
                    sw.Write(dt + ",");
                    // sw.Write(item.ts.ToString() + ",");
                    sw.Write(item.lat.Value.ToString("G9") + ",");
                        sw.WriteLine(item.lon.Value.ToString("G9"));

                    }
                }
            
 

        }

        public static void PrintAllPoint(PrintMode printMode, InterpolationType interpolationType = InterpolationType.Non, IDataRepository<Measurement> _dataRepository = null)
        {

            // string path = @"C:\SHahrzad Files\LiRA\LiRA_Code\MyLiRAProject\LiRA\PullingRawDataService\bin\AllPoint" + printMode.ToString() + "_"  + interpolationType.ToString() + "_" + ".txt";

            string path = @"..\PullingRawDataService\bin\AllPoint" + printMode.ToString() + "_" + interpolationType.ToString() + "_" + ".txt";



            // Create a file to write to.
            //  if (!File.Exists(path))
            // {
            // File.Create(path);

            //StreamWriter sw = new StreamWriter(path);
            // }
            using (StreamWriter sw = File.CreateText(path))
            {
                foreach (Measurement item in allPoints)

                {
                    if (printMode == PrintMode.Raw && item.T == "track.pos")
                    {
                        sw.Write(item.lat.Value.ToString("G9") + " ");
                        sw.Write(item.lon.Value.ToString("G9") + ", ");
                       // sw.Write(allPoints.IndexOf(item) +);

                    }

                    if (printMode == PrintMode.Interpolated)
                    {
                        if (item.lat != null && item.lon != null)
                        {
                            sw.Write(item.lat.Value.ToString("G9") + " ");
                            sw.Write(item.lon.Value.ToString("G9") + ", ");


                        }
                    }

                    if (printMode == PrintMode.MapMatched)
                    {
                        if (item.MapReferences.lat_MapMatched != null && item.MapReferences.lon_MapMatched != null && item.MapReferences.lat_MapMatched != -1 && item.MapReferences.lon_MapMatched != -1)
                        {
                            if (item.MapReferences.lon_MapMatched != null) sw.Write(item.MapReferences.lon_MapMatched.Value.ToString("G9") + " ");
                            if (item.MapReferences.lat_MapMatched != null) sw.Write(item.MapReferences.lat_MapMatched.Value.ToString("G9") + ", ");

                            _dataRepository.Add(item);
                        }
                    }

                    if (printMode == PrintMode.All)
                    {
                        if (item.lat != null && item.lon != null)
                        {
                            sw.Write(allPoints.Contains(item) + ", ");
                            sw.Write(item.lat.Value.ToString("G9") + " ");
                            sw.Write(item.lon.Value.ToString("G9") + ", ");
                            sw.Write(item.isComputed + " , ");

                            if (item.MapReferences.lat_MapMatched != null) sw.Write(item.MapReferences.lat_MapMatched.Value.ToString("G9") + " , ");
                            if (item.MapReferences.lon_MapMatched != null) sw.Write(item.MapReferences.lon_MapMatched.Value.ToString("G9") + " , ");
                            if (item.MapReferences.wayPointName != null) sw.Write(item.MapReferences.wayPointName.ToString() + " , ");
                            var dt = item.TS_or_Distance.ToString("yyyy-MM-dd HH:mm:ss" + " , ");
                            sw.Write(dt + " , ");

                            sw.WriteLine(); 
                        }
                    }
                }

                sw.Close();
            }

            
        }

        //public static void ConstructSensorData(List<Interpolated> nonInterpolate_list, List<Interpolated> interpolate_list)
        //{
        //    ////Preparing float arrays for interpolation
        //    //float[] x_ = new float[x_list.Count];
        //    //float[] y_ = new float[y_list.Count];
        //    //DateTimeOffset[] t_ = new DateTimeOffset[t_list.Count];
        //    //// DateTimeOffset[] time_interpolate = new DateTimeOffset[time_interpolate_list.Count];
        //    //x_ = x_list.ToArray();
        //    //y_ = y_list.ToArray();
        //    //t_ = t_list.ToArray();





        //    List<Interpolated> xs, ys;

        //    //Fit and eval for GPS coordinates => parametric
        //    CubicSpline.FitSensorDataGeometric(nonInterpolate_list, interpolate_list, out xs, out ys);

        //    // CubicSpline.FitSensorDataGeometric(x_, y_, t_, time_interpolate, time_interpolate_list.Count, out xs, out ys);

        //    //  PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit", x_, y_, xs, ys, @"..\..\testSplineFitParametric.png");





        //}
        private static void PlotSplineSolution(string title, float[] x, float[] y, float[] xs, float[] ys, string path, float[] qPrime = null)
        {
            //var chart = new Chart();
            //chart.Size = new Size(600, 400);
            //chart.Titles.Add(title);
            //chart.Legends.Add(new Legend("Legend"));

            //ChartArea ca = new ChartArea("DefaultChartArea");
            //ca.AxisX.Title = "X";
            //ca.AxisY.Title = "Y";
            //chart.ChartAreas.Add(ca);

            //Series s1 = CreateSeries(chart, "Spline", CreateDataPoints(xs, ys), Color.Blue, MarkerStyle.None);
            //Series s2 = CreateSeries(chart, "Original", CreateDataPoints(x, y), Color.Green, MarkerStyle.Diamond);

            //chart.Series.Add(s2);
            //chart.Series.Add(s1);

            //if (qPrime != null)
            //{
            //    Series s3 = CreateSeries(chart, "Slope", CreateDataPoints(xs, qPrime), Color.Red, MarkerStyle.None);
            //    chart.Series.Add(s3);
            //}

            //ca.RecalculateAxesScale();
            //ca.AxisX.Minimum = Math.Floor(ca.AxisX.Minimum);
            //ca.AxisX.Maximum = Math.Ceiling(ca.AxisX.Maximum);
            //int nIntervals = (x.Length - 1);
            //nIntervals = Math.Max(4, nIntervals);
            //ca.AxisX.Interval = (ca.AxisX.Maximum - ca.AxisX.Minimum) / nIntervals;

            //// Save
            //if (File.Exists(path))
            //{
            //    File.Delete(path);
            //}

            //using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            //{
            //    chart.SaveImage(fs, ChartImageFormat.Png);
            //}
        }
    }



   
}
