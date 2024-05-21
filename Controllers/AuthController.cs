using AutoMapper;
using BlossomServer.Datas.Authentication;
using BlossomServer.Datas.User;
using BlossomServer.Services.AuthenticationServices;
using BlossomServer.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + "Firebase")]
	public class AuthController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IAuthenticationService _authenticationService;
		private readonly IMapper _mapper;

		public AuthController(IUserService userService, IAuthenticationService authenticationService, IMapper mapper)
		{
			_userService = userService;
			_authenticationService = authenticationService;
			_mapper = mapper;
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			var result = await _userService.Login(request);
			return StatusCode(result.Status, result);
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<IActionResult> Register(CreateUserRequest newUser)
		{
			newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
			var result = await _userService.Register(newUser);
			return StatusCode(result.Status, result);
		}

		[HttpPut("change-password")]
		public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
		{
			Guid userID = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserID")!.Value);
			var result = await _userService.ChangePassword(userID, request);
			return StatusCode(result.Status, result);
		}

		[HttpPost("logout")]
		public async Task<IActionResult> RevokeToken(RefreshTokenRequest refreshToken)
		{
			var result = await _userService.Logout(refreshToken.RefreshToken);
			return StatusCode(result.Status, result);
		}

		[AllowAnonymous]
		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
		{
			var result = await _authenticationService.Refresh(request.RefreshToken, DateTime.Now);
			return StatusCode(result.Status, result);
		}

		[AllowAnonymous]
		[HttpPost("send-verify-code")]
		public async Task<IActionResult> GenerateResetPasswordCode(SendResetCodeRequest request)
		{
			var result = await _authenticationService.SendResetCode(request);
			return StatusCode(result.Status, result);
		}

		[AllowAnonymous]
		[HttpPost("verify-code")]
		public async Task<IActionResult> VerifyCode(VerifyCodeRequest request)
		{
			var result = await _authenticationService.VerifyResetCode(request);
			return StatusCode(result.Status, result);
		}

		[AllowAnonymous]
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
		{
			var result = await _authenticationService.ResetPassword(request);
			return StatusCode(result.Status, result);
		}
	}
}
