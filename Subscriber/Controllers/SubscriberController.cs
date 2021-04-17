using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Subscriber.Controllers
{
    [ApiController]
    public class SubscriberController : ControllerBase
    {
        const string storeName = "redis-state";
        const string key = "counter";

        private readonly DaprClient _daprClient;

        public SubscriberController(DaprClient daprClient)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        }

        [HttpGet]
        [Route("api/counter")]
        public async Task<object> GetCounter()
        {
            var counter = await _daprClient.GetStateAsync<int>(storeName, key) + 1;
            await _daprClient.SaveStateAsync(storeName, key, counter);
            return new { counter };
        }

        [HttpGet]
        [Route("api/clear")]
        public async Task ClearCounter()
        {
            await _daprClient.DeleteStateAsync(storeName, key);
        }

        public class testData
        {
            public int counter { get; set; }
        }


        [Topic("redis-pubsub", "testData")]
        [HttpPost("/weather")]
        public async Task<IActionResult> PostWeathers(testData data)
        {
            await _daprClient.SaveStateAsync(storeName, key, data.counter);
            return NoContent();
        }
    }
}
