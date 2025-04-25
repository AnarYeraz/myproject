using Gateway.Models;
using Gateway.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IRabbitProducer rabbitProducer;
        public NotificationController(IRabbitProducer producer)
        {
            rabbitProducer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> PostNotificationAsync(NotificationRequest request)
        {
            var resultSend = rabbitProducer.Publish(request);
            if (!resultSend.Success)
            {
                return StatusCode(500, new { message = resultSend.Message });
            }

            return Ok(new { message = resultSend.Message });
        }
    }
}
