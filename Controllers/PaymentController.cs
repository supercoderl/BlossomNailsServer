using BlossomServer.Application.Interfaces;
using BlossomServer.Application.SortProviders;
using BlossomServer.Application.ViewModels;
using BlossomServer.Application.ViewModels.Payments;
using BlossomServer.Application.ViewModels.Sorting;
using BlossomServer.Domain.Entities;
using BlossomServer.Domain.Notifications;
using BlossomServer.Models;
using BlossomServer.Swagger;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BlossomServer.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/v1/[controller]")]
    public sealed class PaymentController : ApiController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(
            INotificationHandler<DomainNotification> notifications,
            IPaymentService paymentService) : base(notifications)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [SwaggerOperation("Get a list of all payments")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PagedResult<PaymentViewModel>>))]
        public async Task<IActionResult> GetAllPaymentsAsync(
            [FromQuery] PageQuery query,
            [FromQuery] string searchTerm = "",
            [FromQuery] bool includeDeleted = false,
            [FromQuery] [SortableFieldsAttribute<PaymentViewModelSortProvider, PaymentViewModel, Payment>]
        SortQuery? sortQuery = null)
        {
            var payments = await _paymentService.GetAllPaymentsAsync(
                query,
                includeDeleted,
                searchTerm,
                sortQuery);
            return Response(payments);
        }
    }
}
