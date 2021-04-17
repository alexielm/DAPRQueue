using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Publisher.Controllers
{
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly DaprClient _daprClient;

        public PublisherController(DaprClient daprClient)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        }

        public class testData
        {
            public int counter { get; set; }
        }

        [HttpGet]
        [Route("api/counter")]
        public async Task<object> CallCounterAPI()
        {
            var response = await _daprClient.InvokeMethodAsync<testData>(
                HttpMethod.Get,
                "subscriber",
                "api/counter");

            return response;
        }

        [HttpGet]
        [Route("api/publish")]
        public async Task Publish()
        {
            var data = new testData
            {
                counter = 33
            };
            await _daprClient.PublishEventAsync("redis-pubsub", "testData", data);
        }
    }
}
