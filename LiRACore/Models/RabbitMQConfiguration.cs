using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace LiRACore.Models
{
    public class RabbitMQConfiguration 
    {
        //        "HostName": "localhost",
        //        "Username": "guest",
        //        "Password": "guest",
        //        "VirtualHost": "/",
        //        "AutomaticRecoveryEnabled": true,
        //        "RequestedHeartbeat": 30

        public Data_Connection QueueConnection { get; set; }

        public string PullingEndpoint { get; set; }

        public string ProcessData_QueueName { get; set; }

        public string PullData_QueueName { get; set; }
    }


        public class Data_Connection
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public bool AutomaticRecoveryEnabled { get; set; }
        public int RequestedHeartbeat { get; set; }

        public int Port { get; set; }

        public int RequestedConnectionTimeout { get; set; }

    }

}
