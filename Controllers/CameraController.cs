using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSMessageProcessor.Kafka.Interface;
using System;
using System.Threading.Tasks;

namespace CameraAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CameraController : ControllerBase
    {
        private readonly string topic = "receipt-image";
        private readonly IKafkaProducer<string, string> _kafkaProducer;
        public CameraController(IKafkaProducer<string, string> kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Upload(string image)
        {
            try
            {
                await _kafkaProducer.ProduceAsync(topic, "image-id", image);
                return Ok("Uploading receipt image");
            }
            catch(Exception)
            {
                return Problem("Could not upload the image");
            }
        }
    }
}
