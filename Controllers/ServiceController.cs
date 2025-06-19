using BlossomServer.Application.Interfaces;
using BlossomServer.Application.SortProviders;
using BlossomServer.Application.ViewModels;
using BlossomServer.Application.ViewModels.Services;
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
    public sealed class ServiceController : ApiController
    {
        private readonly IServiceService _serviceService;

        public ServiceController(
            INotificationHandler<DomainNotification> notifications,
            IServiceService serviceService) : base(notifications)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        [SwaggerOperation("Get a list of all services")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PagedResult<ServiceViewModel>>))]
        public async Task<IActionResult> GetAllUsersAsync(
            [FromQuery] PageQuery query,
            [FromQuery] string searchTerm = "",
            [FromQuery] bool includeDeleted = false,
            [FromQuery] [SortableFieldsAttribute<ServiceViewModelSortProvider, ServiceViewModel, Service>]
        SortQuery? sortQuery = null)
        {
            var services = await _serviceService.GetAllServicesAsync(
                query,
                includeDeleted,
                searchTerm,
                sortQuery);
            return Response(services);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Get a service by id")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<ServiceViewModel>))]
        public async Task<IActionResult> GetServiceByIdAsync([FromRoute] Guid id)
        {
            var service = await _serviceService.GetServiceByServiceIdAsync(id);
            return Response(service);
        }

        [HttpPost]
        [SwaggerOperation("Create a new service")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> CreateServiceAsync([FromForm] CreateServiceViewModel viewModel)
        {
            var serviceId = await _serviceService.CreateServiceAsync(viewModel);
            return Response(serviceId);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Delete a service")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> DeleteServiceAsync([FromRoute] Guid id)
        {
            await _serviceService.DeleteServiceAsync(id);
            return Response(id);
        }

        [HttpPut]
        [SwaggerOperation("Update a service")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UpdateServiceViewModel>))]
        public async Task<IActionResult> UpdateServiceAsync([FromForm] UpdateServiceViewModel viewModel)
        {
            await _serviceService.UpdateServiceAsync(viewModel);
            return Response(viewModel);
        }
    }
}
