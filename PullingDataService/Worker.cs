using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LiRACore.MessageQueue;
using LiRACore.Models;
using LiRACore.Models.DataManager;
using LiRACore.Models.RawData;
using LiRACore.Models.Repository;
using LiRACore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PullingDataService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageQueue _messageQueue;
        private readonly RabbitMQConfiguration _rabbitMQ_Configuration;

        public Worker(ILogger<Worker> logger, IMessageQueue messageQueue, RabbitMQConfiguration rabbitMQ_Configuration)
        {
            _logger = logger;
            _messageQueue = messageQueue;
            _rabbitMQ_Configuration = rabbitMQ_Configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
          
            _messageQueue.Receive(_rabbitMQ_Configuration.PullData_QueueName, PullData );

            _logger.LogInformation("Pulling Data service running at: {time}", DateTimeOffset.Now);


        }
        public async Task<bool> PullData(string message) {

            // The exported data package can be downloaded by calling the tasks/ID/result endpoint, which will
            //return the file TRIP_ID.json.gz containing the exported data. After you have downloaded the exported
            //data, you must remove the exported result by calling the endpoint tasks/ID/clear_result

            _messageQueue.Send(_rabbitMQ_Configuration.ProcessData_QueueName, message);

            //TripNotification notification = JsonConvert.DeserializeObject<TripNotification>(message);

            //using (var httpClient = new HttpClient())
            //{
            //    using (var response = await httpClient.GetAsync(_rabbitMQ_Configuration.PullingEndpoint + notification.TripId.ToString() + "/result"))
            //    {
            //        // Pick up data from AutoPi server 
            //         string apiResponse_sensorDataFileJSON = await response.Content.ReadAsStringAsync();

            //        // var jsonReader = new JsonTextReader(new StreamReader(apiResponse_sensorDataFileJSON));

            //        if (System.IO.File.Exists(apiResponse_sensorDataFileJSON))
            //        {
            //            //TripNotification tripData = JsonConvert.DeserializeObject<TripNotification>(apiResponse_sensorDataFileJSON);

            //            //save it somewhere in DTU VM

            //            //Then Insert trip Id in the second queue, this means data has been read from AutoPi server and is ready for being preprocessed
            //            _messageQueue.Send(_rabbitMQ_Configuration.ProcessData_QueueName, notification.TripId.ToString());

            //            // if sent successfully, remove it from DataPulling queue


            //        }
            //    }
            //}

            return true;

        }
    }
}
