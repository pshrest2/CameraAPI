using CameraAPI.Services.Interface;
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
        private readonly IFileUploadService _fileUploadService;

        public CameraController(IKafkaProducer<string, string> kafkaProducer, IFileUploadService fileUploadService)
        {
            _kafkaProducer = kafkaProducer;
            _fileUploadService = fileUploadService;
        }
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Upload(IFormFile imageFile)
        {
            try
            {
                // Upload file to blob
                //var uri = await _fileUploadService.UploadFileToStorage(imageFile);
                var uri = "https://receiptimages.blob.core.windows.net/receipt-images/IMG_8972.JPG?sv=2021-06-08&se=2022-06-26T23%3A10%3A45Z&sr=b&sp=r&sig=3TmijsQp5l9BN2ntWoLvc4wRPDsTprzzs7i%2FuKj8mek%3D";

                // Send kafka message with blob uri to the form recognizer service
                await _kafkaProducer.ProduceAsync(topic, "image-id", uri);
                return Ok();
            }
            catch(Exception)
            {
                return Problem("Could not upload the image");
            }
        }
    }
}
