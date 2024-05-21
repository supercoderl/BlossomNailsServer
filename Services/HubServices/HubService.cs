using AutoMapper;
using BlossomServer.Datas.Notification;
using BlossomServer.Datas;
using BlossomServer.Entities;
using Microsoft.AspNetCore.SignalR;

namespace BlossomServer.Services.HubServices
{
	public class HubService : Hub
	{
		private readonly string _botUser;
		private readonly IDictionary<string, UserConnection> _connection;
		private readonly BlossomNailsContext _context;
		private readonly IMapper _mapper;

		public HubService(IDictionary<string, UserConnection> connection, BlossomNailsContext context, IMapper mapper)
		{
			_botUser = "MyChat Bot";
			_connection = connection;
			_context = context;
			_mapper = mapper;
		}
		public async Task JoinRoom(UserConnection conn)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, conn.Room);

			_connection[Context.ConnectionId] = conn;

			/*await Clients.Group(conn.Room).SendAsync("ReceiveMessage", _botUser, $"{conn.User} has joined ${conn.Room}");*/
		}

		public async Task SendNotify(string msg, string type, int? objectID)
		{
			if (_connection.TryGetValue(Context.ConnectionId, out UserConnection conn))
			{
				var result = await SaveNotification(type, objectID, msg);
				await Clients.Group(conn.Room).SendAsync("ReceiveMessage", conn.User, msg, result);
			}
		}

		public async Task<NotificationProfile> SaveNotification(string type, int? objectID, string msg)
		{
			try
			{
				var notification = new Notification
				{
					Receiver = null,
					Type = type,
					ObjectID = objectID,
					Message = msg,
					CreatedAt = DateTime.Now,
					ReadAt = null
				};

				await _context.Notifications.AddAsync(notification);
				await _context.SaveChangesAsync();

				return _mapper.Map<NotificationProfile>(notification);
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
