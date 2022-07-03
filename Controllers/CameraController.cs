using CameraAPI.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSMessageProcessor.RabbitMQ.Interface;
using System;
using System.Threading.Tasks;

namespace CameraAPI.Controllers
{
    [Route("api/camera")]
    [ApiController]
    public class CameraController : ControllerBase
    {
        private readonly IRabbitProducer<string> _rabbitProducer;
        private readonly IFileUploadService _fileUploadService;

        public CameraController(IRabbitProducer<string> rabbitProducer, IFileUploadService fileUploadService)
        {
            _rabbitProducer = rabbitProducer;
            _fileUploadService = fileUploadService;
        }
        [HttpPost]
        [Route("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Upload(IFormFile imageFile)
        {
            try
            {
                // Upload file to blob
                var uri = await _fileUploadService.UploadFileToStorage(imageFile);

                // Send rabbit message with blob uri to the form recognizer service
                await _rabbitProducer.ProduceMessage("ReceiptImage", uri);
                return Ok();
            }
            catch(Exception e) 
            {
                return Problem($"Could not upload the image: {e}");
            }
        }
    }
}
