using BlossomServer.Application.Interfaces;
using BlossomServer.Application.SortProviders;
using BlossomServer.Application.ViewModels;
using BlossomServer.Application.ViewModels.Reviews;
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
    public sealed class ReviewController : ApiController
    {
        private readonly IReviewService _reviewService;

        public ReviewController(
            INotificationHandler<DomainNotification> notifications,
            IReviewService reviewService) : base(notifications)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [SwaggerOperation("Get a list of all reviews")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PagedResult<ReviewViewModel>>))]
        public async Task<IActionResult> GetAllReviewsAsync(
            [FromQuery] PageQuery query,
            [FromQuery] string searchTerm = "",
            [FromQuery] bool includeDeleted = false,
            [FromQuery] [SortableFieldsAttribute<ReviewViewModelSortProvider, ReviewViewModel, Review>]
        SortQuery? sortQuery = null)
        {
            var reviews = await _reviewService.GetAllReviewsAsync(
                query,
                includeDeleted,
                searchTerm,
                sortQuery);
            return Response(reviews);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Get a review by id")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<ReviewViewModel>))]
        public async Task<IActionResult> GetReviewByIdAsync([FromRoute] Guid id)
        {
            var review = await _reviewService.GetReviewByReviewIdAsync(id);
            return Response(review);
        }

        [HttpPost]
        [SwaggerOperation("Create a new review")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> CreateReviewAsync([FromBody] CreateReviewViewModel viewModel)
        {
            var reviewId = await _reviewService.CreateReviewAsync(viewModel);
            return Response(reviewId);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Delete a review")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> DeleteReviewAsync([FromRoute] Guid id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return Response(id);
        }

        [HttpPut]
        [SwaggerOperation("Update a review")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UpdateReviewViewModel>))]
        public async Task<IActionResult> UpdateReviewAsync([FromBody] UpdateReviewViewModel viewModel)
        {
            await _reviewService.UpdateReviewAsync(viewModel);
            return Response(viewModel);
        }
    }
}
