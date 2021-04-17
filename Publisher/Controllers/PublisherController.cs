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

        public class simple
        {
            public int counter { get; set; }
        }

        [HttpGet]
        [Route("api/counter")]
        public async Task<object> CallCounterAPI()
        {
            var response = await _daprClient.InvokeMethodAsync<simple>(
                HttpMethod.Get,
                "subscriber",
                "api/counter");

            return response;
        }
    }
}
