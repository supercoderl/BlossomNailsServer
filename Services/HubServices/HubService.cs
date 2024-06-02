using AutoMapper;
using BlossomServer.Datas.Notification;
using BlossomServer.Datas;
using BlossomServer.Entities;
using Microsoft.AspNetCore.SignalR;
using BlossomServer.Datas.Chat;
using BlossomServer.Services.ChatServices;

namespace BlossomServer.Services.HubServices
{
	public class HubService : Hub
	{
		private readonly string _botUser;
		private readonly IDictionary<string, UserConnection> _connection;
        private readonly IDictionary<string, ChatConnection> _chatConnection;
        private readonly BlossomNailsContext _context;
		private readonly IMapper _mapper;
        private readonly IChatService _chatService;

        public HubService(
			IDictionary<string, UserConnection> connection, 
			IDictionary<string, ChatConnection> chatConnection, 
			BlossomNailsContext context, 
			IMapper mapper,
			IChatService chatService
		)
		{
			_botUser = "MyChat Bot";
			_connection = connection;
            _chatConnection = chatConnection;
            _context = context;
			_mapper = mapper;
            _chatService = chatService;
        }
		public async Task JoinRoom(UserConnection conn)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, conn.Room);

			_connection[Context.ConnectionId] = conn;

/*			await Clients.Group(conn.Room).SendAsync("ReceiveMessageChat", _botUser, $"{conn.User} has joined ${conn.Room}");*/
		}

		public async Task JoinRoomChatCustomer(ChatConnection conn)
		{
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.RoomID);

            _chatConnection[Context.ConnectionId] = conn;

/*            await Clients.Group(conn.RoomID).SendAsync("ReceiveMessageChat", $"User {conn.UserID} has joined into room ${conn.RoomID}");
*/        }

        public async Task JoinRoomChatConsultant(ChatConnection conn)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.RoomID);

            _chatConnection[Context.ConnectionId] = conn;

            await Clients.Group(conn.RoomID).SendAsync("ReceiveMessageChat", $"User {conn.UserID} has joined into room ${conn.RoomID}");
        }

        public async Task SendNotify(string msg, string type, int? objectID)
		{
			if (_connection.TryGetValue(Context.ConnectionId, out UserConnection? conn))
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

        public async Task SendMessage(string message)
        {
			try
			{
                if (_chatConnection.TryGetValue(Context.ConnectionId, out ChatConnection? conn))
                {
                    var result = await _chatService.SendMessage(Guid.Parse(conn.UserID), conn.RoomID, message);

                    if (result == null || result.Data == null) return;
                    await Clients.Groups(conn.RoomID).SendAsync("ReceiveMessageChat", result.Data);
					await Clients.All.SendAsync("ReceiveMessageChatForConsultant", result.Data);
				}
            }
			catch (Exception)
			{
				throw;
			}
        }
    }
}
