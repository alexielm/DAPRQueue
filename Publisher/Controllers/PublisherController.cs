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

        public class items
        {
            public int total { get; set; }
        }

        [HttpGet]
        [Route("api/getTotal")]
        public async Task<object> CallCounterAPI()
        {
            var response = await _daprClient.InvokeMethodAsync<items>(
                HttpMethod.Get,
                "subscriber",
                "api/total");

            return response;
        }

        [HttpGet]
        [Route("api/clearItems")]
        public async Task ClearCounter()
        {
            await _daprClient.InvokeMethodAsync(
                HttpMethod.Get,
                "subscriber",
                "api/clear");
        }

        [HttpGet]
        [Route("api/addItems")]
        public async Task Publish([FromQuery] int newItems)
        {
            await _daprClient.PublishEventAsync("redis-pubsub", "newItems", new items { total = newItems });
        }
    }
}
