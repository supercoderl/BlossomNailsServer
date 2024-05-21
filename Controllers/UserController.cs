using AutoMapper;
using BlossomServer.Datas.Service;
using BlossomServer.Datas.User;
using BlossomServer.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + "Firebase")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IMapper _mapper;

		public UserController(IUserService userService, IMapper mapper)
		{
			_userService = userService;
			_mapper = mapper;
		}

		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			var userAuthID = Guid.Parse(User.FindFirstValue("UserID")!);
			var result = await _userService.GetProfile(userAuthID);
			return StatusCode(result.Status, result);
		}

		[HttpGet("user-by-id/{userID}")]
		public async Task<IActionResult> GetUserByID(Guid userID)
		{
			var result = await _userService.GetProfile(userID);
			return StatusCode(result.Status, result);
		}

		[HttpGet("users")]
		public async Task<IActionResult> GetUsers([FromQuery] FilterUser? filterObject)
		{
			var result = await _userService.GetUsers(filterObject);
			return StatusCode(result.Status, result);
		}

		[HttpPut("update-user/{UserID}")]
		public async Task<IActionResult> UpdateUser(Guid UserID, UpdateUserRequest request)
		{
			var result = await _userService.UpdateUser(UserID, request);
			return StatusCode(result.Status, result);
		}

		[HttpDelete("delete-user/{UserID}")]
		public async Task<IActionResult> DeleteUser(Guid UserID)
		{
			var result = await _userService.DeleteUser(UserID);
			return StatusCode(result.Status, result);
		}
	}
}
