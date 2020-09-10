using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiRACore.MessageQueue;
using LiRACore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PullingDataService
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
                    RabbitMQConfiguration pullingDataConnection = configuration.GetSection("RabbitMQ").Get<RabbitMQConfiguration>();
                    services.AddSingleton(pullingDataConnection);

               
                    //Data_Connection pullingDataConnection = configuration.GetSection("RabbitMQ").Get<RabbitMQConfiguration>().QueueConnection;
                    //services.AddSingleton(pullingDataConnection);
                });
    }
}
