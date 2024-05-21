using AutoMapper;
using BlossomServer.Datas.Notification;
using BlossomServer.Entities;
using BlossomServer.Response;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlossomServer.Services.NotificationServices
{
	public class NotificationService : INotificationService
	{
		private readonly BlossomNailsContext _context;
		private readonly IMapper _mapper;

		public NotificationService(BlossomNailsContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}
		public async Task<ApiResponse<List<NotificationProfile>>> GetNotifications()
		{
			try
			{
				await Task.CompletedTask;
				var notifications = await _context.Notifications.OrderByDescending(x => x.CreatedAt).ToListAsync();
				if (!notifications.Any())
				{
					return new ApiResponse<List<NotificationProfile>>
					{
						Success = false,
						Message = "There is not any notifications.",
						Status = (int)HttpStatusCode.OK
					};
				}
				return new ApiResponse<List<NotificationProfile>>
				{
					Success = true,
					Message = $"Get notifications successfully. Found {notifications.Count()} notifications!",
					Data = notifications.Select(x => _mapper.Map<NotificationProfile>(x)).ToList(),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<List<NotificationProfile>>
				{
					Success = false,
					Message = "NotificationService - GetNotifications: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}

		public async Task<ApiResponse<NotificationProfile>> UpdateNotification(int NotificationID, UpdateNotificationRequest request)
		{
			try
			{
				await Task.CompletedTask;

				if (NotificationID != request.NotificationID)
				{
					return new ApiResponse<NotificationProfile>
					{
						Success = false,
						Message = "Notification with id not match.",
						Status = (int)HttpStatusCode.OK
					};
				};

				var notificationEntity = await _context.Notifications.FindAsync(NotificationID);

				if (notificationEntity == null)
				{
					return new ApiResponse<NotificationProfile>
					{
						Success = false,
						Message = "Couldn't update because this notification does not exists.",
						Status = (int)HttpStatusCode.OK
					};
				};

				_mapper.Map(request, notificationEntity);
				_context.Notifications.Update(notificationEntity);
				await _context.SaveChangesAsync();

				return new ApiResponse<NotificationProfile>
				{
					Success = true,
					Message = $"Updated notification with ID {NotificationID}.",
					Data = _mapper.Map<NotificationProfile>(notificationEntity),
					Status = (int)HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				return new ApiResponse<NotificationProfile>
				{
					Success = false,
					Message = "NotificationService - UpdateNotification: " + ex.Message,
					Status = (int)HttpStatusCode.InternalServerError
				};
			}
		}
	}
}
