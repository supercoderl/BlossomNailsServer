using BlossomServer.Application.Interfaces;
using BlossomServer.Application.SortProviders;
using BlossomServer.Application.ViewModels;
using BlossomServer.Application.ViewModels.Sorting;
using BlossomServer.Application.ViewModels.WorkSchedules;
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
    public sealed class WorkScheduleController : ApiController
    {
        private readonly IWorkScheduleService _workScheduleService;

        public WorkScheduleController(
            INotificationHandler<DomainNotification> notifications,
            IWorkScheduleService workScheduleService) : base(notifications)
        {
            _workScheduleService = workScheduleService;
        }

        [HttpGet]
        [SwaggerOperation("Get a list of all workSchedules")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PagedResult<WorkScheduleViewModel>>))]
        public async Task<IActionResult> GetAllWorkSchedulesAsync(
            [FromQuery] PageQuery query,
            [FromQuery] string searchTerm = "",
            [FromQuery] bool includeDeleted = false,
            [FromQuery] [SortableFieldsAttribute<WorkScheduleViewModelSortProvider, WorkScheduleViewModel, WorkSchedule>]
        SortQuery? sortQuery = null)
        {
            var workSchedules = await _workScheduleService.GetAllWorkSchedulesAsync(
                query,
                includeDeleted,
                searchTerm,
                sortQuery);
            return Response(workSchedules);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Get a workSchedule by id")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<WorkScheduleViewModel>))]
        public async Task<IActionResult> GetWorkScheduleByIdAsync([FromRoute] Guid id)
        {
            var workSchedule = await _workScheduleService.GetWorkScheduleByWorkScheduleIdAsync(id);
            return Response(workSchedule);
        }

        [HttpPost]
        [SwaggerOperation("Create a new workSchedule")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> CreateWorkScheduleAsync([FromBody] CreateWorkScheduleViewModel viewModel)
        {
            var workScheduleId = await _workScheduleService.CreateWorkScheduleAsync(viewModel);
            return Response(workScheduleId);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Delete a workSchedule")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> DeleteWorkScheduleAsync([FromRoute] Guid id)
        {
            await _workScheduleService.DeleteWorkScheduleAsync(id);
            return Response(id);
        }

        [HttpPut]
        [SwaggerOperation("Update a workSchedule")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UpdateWorkScheduleViewModel>))]
        public async Task<IActionResult> UpdateWorkScheduleAsync([FromBody] UpdateWorkScheduleViewModel viewModel)
        {
            await _workScheduleService.UpdateWorkScheduleAsync(viewModel);
            return Response(viewModel);
        }
    }
}
