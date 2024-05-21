using BlossomServer.Datas.Notification;
using BlossomServer.Services.NotificationServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + "Firebase")]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationService _notificationService;

		public NotificationController(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}

		[HttpGet("notifications")]
		public async Task<IActionResult> GetNotifications()
		{
			var result = await _notificationService.GetNotifications();
			return StatusCode(result.Status, result);
		}

		[HttpPut("update-notification/{NotificationID}")]
		public async Task<IActionResult> UpdateNotification(int NotificationID, UpdateNotificationRequest request)
		{
			var result = await _notificationService.UpdateNotification(NotificationID, request);
			return StatusCode(result.Status, result);
		}
	}
}
