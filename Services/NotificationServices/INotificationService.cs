using BlossomServer.Datas.Notification;
using BlossomServer.Response;

namespace BlossomServer.Services.NotificationServices
{
	public interface INotificationService
	{
		Task<ApiResponse<List<NotificationProfile>>> GetNotifications();

		Task<ApiResponse<NotificationProfile>> UpdateNotification(int NotificationID, UpdateNotificationRequest request);
	}
}
