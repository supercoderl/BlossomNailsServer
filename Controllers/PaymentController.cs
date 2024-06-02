using BlossomServer.Datas.Payment;
using BlossomServer.Services.PaymentServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentController(IPaymentService paymentService, IHttpContextAccessor httpContextAccessor)
        {
            _paymentService = paymentService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> VnPayClick(VnPayRequest req)
        {
            var result = await _paymentService.VnPayClick(req, _httpContextAccessor);
            return StatusCode(result.Status, result);
        }
    }
}
