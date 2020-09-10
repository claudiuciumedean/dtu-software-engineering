using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiRACore;
using LiRACore.MessageQueue;
using LiRACore.Models;
using LiRACore.Models.DataManager;
using LiRACore.Models.RawData;
using LiRACore.Models.Repository;
using LiRACore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static LiRACore.Services.AutoPi.SensorReader;

namespace ProcessingDataService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMessageQueue _messageQueue;
        private readonly RabbitMQConfiguration _rabbitMQ_Configuration;

        private readonly IMapMatchingService _mapMatchingService;
        // private  IDataRepository<Measurement> _dataRepository;
        private IDataRepository<Device> _deviceRepository;
        private IDataRepository<Trip> _tripRepository;
        private MeasurementManager _measurementRepository;
        private IDataRepository<MeasurementType> _measurementTypeRepository;
        private IServiceProvider _services;
        private InterpolationType _interpolationType;
        public Worker(ILogger<Worker> logger, IMessageQueue messageQueue, RabbitMQConfiguration rabbitMQ_Configuration, IServiceProvider services, IConfiguration configuration)
        {
            _logger = logger;
            _messageQueue = messageQueue;
            _rabbitMQ_Configuration = rabbitMQ_Configuration;
            _services = services;
            _mapMatchingService = (IMapMatchingService)_services.GetService(typeof(IMapMatchingService));
            _interpolationType = (InterpolationType)Enum.Parse(typeof(InterpolationType), configuration["Interpolation_Type"].ToString());

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            using (var scope = _services.CreateScope())
            {


                _deviceRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IDataRepository<Device>>();


                _tripRepository =
                      scope.ServiceProvider
                          .GetRequiredService<IDataRepository<Trip>>();


                _measurementTypeRepository =
                          scope.ServiceProvider
                              .GetRequiredService<IDataRepository<MeasurementType>>();

                _measurementRepository =
                       scope.ServiceProvider
                     .GetRequiredService<MeasurementManager>();


                _messageQueue.Receive(_rabbitMQ_Configuration.ProcessData_QueueName, ProcessData);

                _logger.LogInformation("Processing data running at: {time}", DateTimeOffset.Now);

                // Run something
                //await Start(); This will be run by ConsumeFunc
            }

        }

        public async Task<bool> ProcessData (string message)
        {
            if (System.IO.File.Exists(message))
            {

                // pick up file from virtual machine:

                // Then open and process it:
                var _rawDataPipeLine = new RawDataPipeLine(_mapMatchingService, _deviceRepository, _tripRepository, _measurementTypeRepository, _measurementRepository, interpolationType);

                await _rawDataPipeLine.Start_JSON_RawData_Pipline(message);

            }
            
            return true;
        }






    }
}
