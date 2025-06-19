using BlossomServer.Application.Interfaces;
using BlossomServer.Application.SortProviders;
using BlossomServer.Application.ViewModels;
using BlossomServer.Application.ViewModels.Categories;
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
    public sealed class CategoryController : ApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(
            INotificationHandler<DomainNotification> notifications,
            ICategoryService categoryService) : base(notifications)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [SwaggerOperation("Get a list of all categories")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PagedResult<CategoryViewModel>>))]
        public async Task<IActionResult> GetAllCategoriesAsync(
            [FromQuery] PageQuery query,
            [FromQuery] string searchTerm = "",
            [FromQuery] bool includeDeleted = false,
            [FromQuery] [SortableFieldsAttribute<CategoryViewModelSortProvider, CategoryViewModel, Category>]
        SortQuery? sortQuery = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(
                query,
                includeDeleted,
                searchTerm,
                sortQuery);
            return Response(categories);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Get a category by id")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<CategoryViewModel>))]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] Guid id)
        {
            var category = await _categoryService.GetCategoryByCategoryIdAsync(id);
            return Response(category);
        }

        [HttpPost]
        [SwaggerOperation("Create a new category")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryViewModel viewModel)
        {
            var categoryId = await _categoryService.CreateCategoryAsync(viewModel);
            return Response(categoryId);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Delete a category")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] Guid id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Response(id);
        }

        [HttpPut]
        [SwaggerOperation("Update a category")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UpdateCategoryViewModel>))]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] UpdateCategoryViewModel viewModel)
        {
            await _categoryService.UpdateCategoryAsync(viewModel);
            return Response(viewModel);
        }
    }
}
