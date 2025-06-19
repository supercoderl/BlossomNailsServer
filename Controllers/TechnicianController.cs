using BlossomServer.Application.Interfaces;
using BlossomServer.Application.SortProviders;
using BlossomServer.Application.ViewModels;
using BlossomServer.Application.ViewModels.Sorting;
using BlossomServer.Application.ViewModels.Technicians;
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
    public sealed class TechnicianController : ApiController
    {
        private readonly ITechnicianService _technicianService;

        public TechnicianController(
            INotificationHandler<DomainNotification> notifications,
            ITechnicianService technicianService) : base(notifications)
        {
            _technicianService = technicianService;
        }

        [HttpGet]
        [SwaggerOperation("Get a list of all technicians")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PagedResult<TechnicianViewModel>>))]
        public async Task<IActionResult> GetAllTechniciansAsync(
            [FromQuery] PageQuery query,
            [FromQuery] string searchTerm = "",
            [FromQuery] bool includeDeleted = false,
            [FromQuery] [SortableFieldsAttribute<TechnicianViewModelSortProvider, TechnicianViewModel, Technician>]
        SortQuery? sortQuery = null)
        {
            var technicians = await _technicianService.GetAllTechniciansAsync(
                query,
                includeDeleted,
                searchTerm,
                sortQuery);
            return Response(technicians);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Get a technician by id")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<TechnicianViewModel>))]
        public async Task<IActionResult> GetTechnicianByIdAsync([FromRoute] Guid id)
        {
            var technician = await _technicianService.GetTechnicianByTechnicianIdAsync(id);
            return Response(technician);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Delete a technician")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> DeleteTechnicianAsync([FromRoute] Guid id)
        {
            await _technicianService.DeleteTechnicianAsync(id);
            return Response(id);
        }

        [HttpPut]
        [SwaggerOperation("Update a technician")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UpdateTechnicianViewModel>))]
        public async Task<IActionResult> UpdateTechnicianAsync([FromBody] UpdateTechnicianViewModel viewModel)
        {
            await _technicianService.UpdateTechnicianAsync(viewModel);
            return Response(viewModel);
        }
    }
}
