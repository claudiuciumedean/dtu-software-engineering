using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LiRACore.MessageQueue;
using LiRACore.Models;
using LiRACore.Models.RawData;
using LiRAWebApi.Models;
using LiRAWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static LiRAWebApi.Services.Authentication;

namespace LiRAWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripNotificationsController : ControllerBase
    {

        private IMessageQueue _messageQueue;
        private readonly RabbitMQConfiguration _rabbitMQ_Configuration;
        private List<Authentication> _authenticationList;
        public TripNotificationsController( IMessageQueue messageQueue, RabbitMQConfiguration rabbitMQ_Configuration, List<Authentication> authenticationList )
        {
            _messageQueue = messageQueue;
            _rabbitMQ_Configuration = rabbitMQ_Configuration;
            _authenticationList = authenticationList;
        }
        // GET: api/TripNotifications
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/TripNotifications/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TripNotifications
        [HttpPost]
        public async Task<HttpStatusCode> Post([FromBody] TripNotification tripNotification)
        {
            Authentication auth = new Authentication();
            auth.Token = this.HttpContext.Request.Headers["Authorization"];
            auth.Role = Roles.AutoPi.ToString();

            if (_authenticationList.Find(x => x.Token == auth.Token) != null)
            { 
                try
                {


                    //The exported data package can be downloaded by calling the tasks/ID/result endpoint, which will
                    //return the file TRIP_ID.json.gz containing the exported data.After you have downloaded the exported
                    //data, you must remove the exported result by calling the endpoint tasks/ID/clear_result
                    _messageQueue.Send(_rabbitMQ_Configuration.PullData_QueueName, tripNotification.TripId.ToString());

                    return HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    return HttpStatusCode.NotFound;
                }

            }

            else
            {
                return HttpStatusCode.Unauthorized;
            }

            //WebHeaderCollection myWebHeaderCollection = webClient.ResponseHeaders;
            //for (int i = 0; i < myWebHeaderCollection.Count; i++)
            //    Console.WriteLine(myWebHeaderCollection.GetKey(i) + " = " + myWebHeaderCollection.Get(i));

            // fcf37c7afa39de197a65b099c401a188904ccbab
            // return Json(tripNotification);-
            //dynamic json = tripNotification;

            //var keyValuePairs = ((System.Collections.Generic.IDictionary<string, object>)tripNotification);



        }

        // PUT: api/TripNotifications/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
