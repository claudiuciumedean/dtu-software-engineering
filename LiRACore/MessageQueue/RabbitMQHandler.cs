using System;
using System.Collections.Generic;
using System.Text;

namespace LiRACore.MessageQueue
{
    using System.Threading;
    using System.Threading.Tasks;
    using LiRACore.Models;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    //Setting up is the same as the publisher; we open a connection and a channel, and declare the queue from
    //which we're going to consume. Note this matches up with the queue that Send publishes to
    public class RabbitMQHandler : IMessageQueue
    {
        //IJWTFactory
        //private readonly ConnectionFactory _jwtFactory;
        private RabbitMQConfiguration _rabbitMQConfiguration;
        public RabbitMQHandler(RabbitMQConfiguration rabbitMQConfiguration)
        {
            // _jwtFactory = jWTFactory;
            _rabbitMQConfiguration = rabbitMQConfiguration;
        }

        public void Receive()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Note that we declare the queue here as well. 
                    //Because we might start the consumer before the publisher, we want to make sure the queue exists before we try to consume messages from it.
                    channel.QueueDeclare(queue: "LiRA_Queue",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    //We're about to tell the server to deliver us the messages from the queue. Since it will push us messages asynchronously, we provide a callback. 
                    //That is what EventingBasicConsumer.Received event handler does.
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);


                        //call worker

                    };
                    channel.BasicConsume(queue: "LiRA_Queue",
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();

                }
            }
        }

        public void Receive(string QueueName, Func<string, Task> ConsumeFunc)
        {
            var factory = new ConnectionFactory()
            {

               
               HostName = _rabbitMQConfiguration.QueueConnection.HostName,
               UserName = _rabbitMQConfiguration.QueueConnection.Username,
               Password = _rabbitMQConfiguration.QueueConnection.Password,
               Port = _rabbitMQConfiguration.QueueConnection.Port,
               RequestedConnectionTimeout = 3000

            };


            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Note that we declare the queue here as well. 
                    //Because we might start the consumer before the publisher, we want to make sure the queue exists before we try to consume messages from it.
                    channel.QueueDeclare(queue: QueueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    //We're about to tell the server to deliver us the messages from the queue. Since it will push us messages asynchronously, we provide a callback. 
                    //That is what EventingBasicConsumer.Received event handler does.
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        //Console.WriteLine(" [x] Received {0}", message);

                        ////fake task to simulate execution time
                        //int dots = message.Split('.').Length - 1;
                        //Thread.Sleep(dots * 1000);


                        var result = ConsumeFunc(message);

                        //call worker

                    };
                    channel.BasicConsume(queue: QueueName,
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();

                }
            }
        }

        public void Send()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                //Queue Declaration
                // Declaring a queue is idempotent - it will only be created if it doesn't exist already. The message content is a byte array, so you can encode whatever you like there.
                //When the code above finishes running, the channel and the connection will be disposed.That's it for our publisher.
                channel.QueueDeclare(queue: "hello",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                string message = "test!";
                var body = Encoding.UTF8.GetBytes(message);

                //Publish message
                channel.BasicPublish(exchange: "",
                                                 routingKey: "LiRA_PullingData_Queue",
                                                 basicProperties: null,
                                                 body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        public void Send(string QueueName, string Message)
        { 
           var factory = new ConnectionFactory() {

                HostName =  _rabbitMQConfiguration.QueueConnection.HostName,
                UserName = _rabbitMQConfiguration.QueueConnection.Username,
                Password = _rabbitMQConfiguration.QueueConnection.Password,
                Port = _rabbitMQConfiguration.QueueConnection.Port,
                RequestedConnectionTimeout = _rabbitMQConfiguration.QueueConnection.RequestedConnectionTimeout, // milliseconds
           };

  //            var factory = new ConnectionFactory();
            
  //              Endpoint.set = @"amqp://guest:guest@liradb.compute.dtu.dk:5672/";
            //factory.set("amqp://guest:guest@localhost:5672/");
            //Connection conn = factory.newConnection();


            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                //Queue Declaration
                // Declaring a queue is idempotent - it will only be created if it doesn't exist already. The message content is a byte array, so you can encode whatever you like there.
                //When the code above finishes running, the channel and the connection will be disposed.That's it for our publisher.
                channel.QueueDeclare(queue: QueueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                string message = Message;
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;


                try
                {
                    //Publish message
                    channel.BasicPublish(exchange: "",
                                                     routingKey: QueueName,
                                                     basicProperties: properties,
                                                     body: body);
                    //Console.WriteLine(" [x] Sent {0}", message);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

           // Console.WriteLine(" Press [enter] to exit.");
            //Console.ReadLine();
        }



        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "test!");
        }
    }
}
