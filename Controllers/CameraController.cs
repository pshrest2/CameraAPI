using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CameraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CameraController : ControllerBase
    {
        public CameraController()
        {
        }

        private readonly ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };
        private readonly string topic = "receipt-image";
        [HttpPost("/upload")]
        public async Task<IActionResult> Upload(string image)
        {
            try
            {
                var result = await PublishMessage(topic, image);
                return Ok(result);
            }
            catch(Exception)
            {
                return Problem("Could not upload the image");
            }
        }

        private async Task<object> PublishMessage(string topic, string message)
        {
            using(var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                    return result;
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Oops, something went wrong: {e}");
                }
            }
            return null;
        }
    }
}
