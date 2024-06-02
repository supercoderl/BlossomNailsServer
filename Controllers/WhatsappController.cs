using BlossomServer.Services.WhatsappServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        private readonly IWhatsappService _whatsappService;

        public WhatsappController(IWhatsappService whatsappService)
        {
            _whatsappService = whatsappService;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(string phoneNumber)
        {
            var result = await _whatsappService.SendNotification(phoneNumber);
            return StatusCode(result.Status, result);
        }
    }
}
