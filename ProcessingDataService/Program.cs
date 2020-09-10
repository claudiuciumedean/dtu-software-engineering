using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiRACore.MessageQueue;
using LiRACore.Models;
using LiRACore.Models.DataManager;
using LiRACore.Models.RawData;
using LiRACore.Models.Repository;
using LiRACore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProcessingDataService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<IMessageQueue, RabbitMQHandler>();

                    services.AddHostedService<Worker>();

                    IConfiguration configuration = hostContext.Configuration;
                    RabbitMQConfiguration processingDataConnection = configuration.GetSection("RabbitMQ").Get<RabbitMQConfiguration>();
                    services.AddSingleton(processingDataConnection);

                    //Postgresql
                    services.AddEntityFrameworkNpgsql().AddDbContext<DeviceContext>(opt =>
                    opt.UseNpgsql(configuration["WriteConnectionStrings:SaveRawDataConnection"]));

                    //configure the data repository using dependency injection.
                    services.AddScoped<IDataRepository<Device>, DeviceManager>();
                    services.AddScoped<IDataRepository<Trip>, TripManager>();
                    services.AddScoped<IDataRepository<MeasurementType>, MeasurementTypeManager>();
                    //services.AddScoped<IDataRepository<Measurement>, MeasurementManager>();
                    services.AddScoped<MeasurementManager>();


                    //Mapmatching Service
                    services.AddSingleton<IMapMatchingService>(new OsrmMapMatchingService(configuration["MapMatching:Connection"]));


                    //services.AddSingleton<IConfiguration>(Configuration);


                    //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


                });
            
    }
}
