using BlossomServer.Application.Interfaces;
using BlossomServer.Application.SortProviders;
using BlossomServer.Application.ViewModels;
using BlossomServer.Application.ViewModels.Promotions;
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
    public sealed class PromotionController : ApiController
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(
            INotificationHandler<DomainNotification> notifications,
            IPromotionService promotionService) : base(notifications)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        [SwaggerOperation("Get a list of all promotions")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PagedResult<PromotionViewModel>>))]
        public async Task<IActionResult> GetAllPromotionsAsync(
            [FromQuery] PageQuery query,
            [FromQuery] string searchTerm = "",
            [FromQuery] bool includeDeleted = false,
            [FromQuery] [SortableFieldsAttribute<PromotionViewModelSortProvider, PromotionViewModel, Promotion>]
        SortQuery? sortQuery = null)
        {
            var promotions = await _promotionService.GetAllPromotionsAsync(
                query,
                includeDeleted,
                searchTerm,
                sortQuery);
            return Response(promotions);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Get a promotion by id")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PromotionViewModel>))]
        public async Task<IActionResult> GetPromotionByIdAsync([FromRoute] Guid id)
        {
            var promotion = await _promotionService.GetPromotionByPromotionIdAsync(id);
            return Response(promotion);
        }

        [HttpPost]
        [SwaggerOperation("Create a new promotion")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> CreatePromotionAsync([FromBody] CreatePromotionViewModel viewModel)
        {
            var promotionId = await _promotionService.CreatePromotionAsync(viewModel);
            return Response(promotionId);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Delete a promotion")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> DeletePromotionAsync([FromRoute] Guid id)
        {
            await _promotionService.DeletePromotionAsync(id);
            return Response(id);
        }

        [HttpPut]
        [SwaggerOperation("Update a promotion")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UpdatePromotionViewModel>))]
        public async Task<IActionResult> UpdatePromotionAsync([FromBody] UpdatePromotionViewModel viewModel)
        {
            await _promotionService.UpdatePromotionAsync(viewModel);
            return Response(viewModel);
        }
    }
}
