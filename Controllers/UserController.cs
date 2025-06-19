using BlossomServer.Application.Interfaces;
using BlossomServer.Application.SortProviders;
using BlossomServer.Application.ViewModels;
using BlossomServer.Application.ViewModels.Sorting;
using BlossomServer.Application.ViewModels.Users;
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
    public sealed class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(
            INotificationHandler<DomainNotification> notifications,
            IUserService userService
        ) : base(notifications)
        {
            _userService = userService;
        }

        [HttpGet]
        [SwaggerOperation("Get a list of all users")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<PagedResult<UserViewModel>>))]
        public async Task<IActionResult> GetAllUsersAsync(
            [FromQuery] PageQuery query,
            [FromQuery] string searchTerm = "",
            [FromQuery] bool includeDeleted = false,
            [FromQuery] [SortableFieldsAttribute<UserViewModelSortProvider, UserViewModel, User>]
        SortQuery? sortQuery = null)
        {
            var users = await _userService.GetAllUsersAsync(
                query,
                includeDeleted,
                searchTerm,
                sortQuery);
            return Response(users);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("Get a user by id")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UserViewModel>))]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id)
        {
            var user = await _userService.GetUserByUserIdAsync(id);
            return Response(user);
        }

        [HttpGet("me")]
        [SwaggerOperation("Get the current active user")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UserViewModel>))]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            var user = await _userService.GetCurrentUserAsync();
            return Response(user);
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation("Create a new user")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserViewModel viewModel)
        {
            var userId = await _userService.CreateUserAsync(viewModel);
            return Response(userId);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation("Delete a user")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<Guid>))]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return Response(id);
        }

        [HttpPut]
        [SwaggerOperation("Update a user")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<UpdateUserViewModel>))]
        public async Task<IActionResult> UpdateUserAsync([FromForm] UpdateUserViewModel viewModel)
        {
            await _userService.UpdateUserAsync(viewModel);
            return Response(viewModel);
        }

        [HttpPost("changePassword")]
        [SwaggerOperation("Change a password for the current active user")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<ChangePasswordViewModel>))]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordViewModel viewModel)
        {
            await _userService.ChangePasswordAsync(viewModel);
            return Response(viewModel);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation("Get a signed token for a user")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<object>))]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserViewModel viewModel)
        {
            var token = await _userService.LoginUserAsync(viewModel);
            return Response(token);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [SwaggerOperation("Get a signed token for a user")]
        [SwaggerResponse(200, "Request successful", typeof(ResponseMessage<object>))]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenViewModel viewModel)
        {
            var token = await _userService.RefreshTokenAsync(viewModel);
            return Response(token);
        }
    }
}
