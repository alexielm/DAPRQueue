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
        const string key = "total";

        private readonly DaprClient _daprClient;

        public SubscriberController(DaprClient daprClient)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        }
        public class items
        {
            public int total { get; set; }
        }

        [HttpGet]
        [Route("api/total")]
        public async Task<object> GetCounter()
        {
            var total = await _daprClient.GetStateAsync<int>(storeName, key);
            return new items { total = total };
        }

        [HttpGet]
        [Route("api/clear")]
        public async Task ClearCounter()
        {
            await _daprClient.DeleteStateAsync(storeName, key);
        }


        [Topic("redis-pubsub", "newItems")]
        [HttpPost("/newItems")]
        public async Task<IActionResult> PostWeathers(items newItems)
        {
            var total = await _daprClient.GetStateAsync<int>(storeName, key);
            await _daprClient.SaveStateAsync(storeName, key, total + newItems.total);
            return NoContent();
        }
    }
}
