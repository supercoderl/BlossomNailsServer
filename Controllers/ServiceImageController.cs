using BlossomServer.Application.Interfaces;
using BlossomServer.Application.SortProviders;
using BlossomServer.Application.ViewModels.Services;
using BlossomServer.Application.ViewModels.Sorting;
using BlossomServer.Application.ViewModels;
using BlossomServer.Domain.Entities;
using BlossomServer.Domain.Notifications;
using BlossomServer.Models;
using BlossomServer.Swagger;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BlossomServer.Application.ViewModels.ServiceImages;

namespace BlossomServer.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/v1/[controller]")]
    public sealed class ServiceImageController : ApiController
    {
        private readonly IServiceImageService _serviceImageService;

        public ServiceImageController(
            INotificationHandler<DomainNotification> notifications,
            IServiceImageService serviceImageService) : base(notifications)
        {
            _serviceImageService = serviceImageService;
        }

        [HttpGet]
        [SwaggerOperation("Get a list of all service images")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PagedResult<ServiceImageViewModel>>))]
        public async Task<IActionResult> GetAllUsersAsync(
            [FromQuery] PageQuery query,
            [FromQuery] string searchTerm = "",
            [FromQuery] bool includeDeleted = false,
            [FromQuery] [SortableFieldsAttribute<ServiceImageViewModelSortProvider, ServiceImageViewModel, ServiceImage>]
        SortQuery? sortQuery = null)
        {
            var serviceImages = await _serviceImageService.GetAllServiceImagesAsync(
                query,
                includeDeleted,
                searchTerm,
                sortQuery);
            return Response(serviceImages);
        }

/*        [HttpGet("{id}")]
        [SwaggerOperation("Get a service by id")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<ServiceViewModel>))]
        public async Task<IActionResult> GetServiceByIdAsync([FromRoute] Guid id)
        {
            var service = await _serviceService.GetServiceByServiceIdAsync(id);
            return Response(service);
        }*/

        [HttpPost]
        [SwaggerOperation("Create a new service image")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> CreateServiceImageAsync([FromForm] CreateServiceImageViewModel viewModel)
        {
            var serviceImageId = await _serviceImageService.CreateServiceImageAsync(viewModel);
            return Response(serviceImageId);
        }

/*        [HttpDelete("{id}")]
        [SwaggerOperation("Delete a service")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> DeleteServiceAsync([FromRoute] Guid id)
        {
            await _serviceService.DeleteServiceAsync(id);
            return Response(id);
        }*/

        [HttpPut]
        [SwaggerOperation("Update a service image")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UpdateServiceImageViewModel>))]
        public async Task<IActionResult> UpdateServiceAsync([FromBody] UpdateServiceImageViewModel viewModel)
        {
            await _serviceImageService.UpdateServiceImageAsync(viewModel);
            return Response(viewModel);
        }
    }
}
