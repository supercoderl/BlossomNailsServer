using BlossomServer.Datas.Chat;
using BlossomServer.Entities;
using BlossomServer.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;

namespace BlossomServer.Services.ChatServices
{
    public class ChatService : IChatService
    {
        private readonly IOptions<MongoDBSetting> _mongoDBSetting;
        private readonly BlossomNailsContext _context;
        private readonly IMongoCollection<ChatProfile> _chatCollection;
        private readonly IMongoCollection<RoomProfile> _roomCollection;
        private readonly IMongoCollection<MessageProfile> _messageCollection;

        public ChatService(IOptions<MongoDBSetting> mongoDBSetting, BlossomNailsContext context)
        {
            MongoClient client = new MongoClient(mongoDBSetting.Value.DefaultConnection);
            IMongoDatabase database = client.GetDatabase(mongoDBSetting.Value.DatabaseName);
            _chatCollection = database.GetCollection<ChatProfile>(mongoDBSetting.Value.ChatCollection);
            _roomCollection = database.GetCollection<RoomProfile>(mongoDBSetting.Value.RoomCollection);
            _messageCollection = database.GetCollection<MessageProfile>(mongoDBSetting.Value.MessageCollection);
            _mongoDBSetting = mongoDBSetting;
            _context = context;
        }

        public async Task<ApiResponse<List<RoomProfile>>> GetAllRooms()
        {
            try
            {
                await Task.CompletedTask;
                var rooms = await _roomCollection.Find(new BsonDocument()).ToListAsync();
                return new ApiResponse<List<RoomProfile>>
                {
                    Success = true,
                    Message = $"Got {rooms.Count()} rooms.",
                    Data = rooms,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<RoomProfile>>
                {
                    Success = false,
                    Message = "Chat Service - Get All Rooms: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<ChatProfile>>> GetListMessages()
        {
            try
            {
                await Task.CompletedTask;
                var messages = await _chatCollection.Find(new BsonDocument()).ToListAsync();
                if (!messages.Any())
                {
                    return new ApiResponse<List<ChatProfile>>
                    {
                        Success = false,
                        Message = "The conversations is empty.",
                        Status = (int)HttpStatusCode.OK,
                    };
                }
                return new ApiResponse<List<ChatProfile>>
                {
                    Success = true,
                    Message = "Get messages successfully.",
                    Data = messages,
                    Status = (int)HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ChatProfile>>
                {
                    Success = false,
                    Message = "Chat Service - Get List Messages: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<MessageProfile>> GetMessage(MessageParam param)
        {
            try
            {
                await Task.CompletedTask;
                var message = await _messageCollection.Find(Builders<MessageProfile>.Filter.Eq("RoomID", param.RoomID)).ToListAsync();
                if(param.WannaLatestMessage == true)
                {
                    message = message.OrderByDescending(x => x.DateSent).ToList();
                }
                if(message == null)
                {
                    return new ApiResponse<MessageProfile>
                    {
                        Success = false,
                        Message = "Conversation is empty",
                        Status = (int)HttpStatusCode.OK
                    };
                }
                return new ApiResponse<MessageProfile>
                {
                    Success = true,
                    Message = $"Got latest message from room {param?.RoomID}",
                    Data = message.FirstOrDefault(),
                    Status = (int)HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MessageProfile>
                {
                    Success = false,
                    Message = "Chat Serivce - Get Message: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<MessageProfile>>> GetMessageFromConversation(string roomID)
        {
            try
            {
                await Task.CompletedTask;
                var messages = await _messageCollection.Find(Builders<MessageProfile>.Filter.Eq("RoomID", roomID)).ToListAsync();
                return new ApiResponse<List<MessageProfile>>
                {
                    Success = true,
                    Message = $"Get list messages from room {roomID}.",
                    Data = messages,
                    Status = (int)HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<MessageProfile>>
                {
                    Success = false,
                    Message = "Chat Service - Get Message From Conversation: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<RoomProfile>> GetRoomByUser(Guid userID, int roleCode)
        {
            try
            {
                await Task.CompletedTask;
                var rooms = await _roomCollection.Find(Builders<RoomProfile>.Filter.Eq("UserID", userID.ToString())).ToListAsync();
                if (!rooms.Any())
                {
                    RoomProfile newRoom = new RoomProfile
                    {
                        UserID = roleCode == 300 ? userID.ToString() : null,
                        ConsultantID = roleCode == 200 ? userID.ToString() : null,
                    };
                    await _roomCollection.InsertOneAsync(newRoom);
                    return new ApiResponse<RoomProfile>
                    {
                        Success = true,
                        Message = "Created new room.",
                        Data = newRoom,
                        Status = (int)HttpStatusCode.OK,
                    };
                }
                else if (rooms.Count() > 1)
                {
                    rooms.ForEach(async room =>
                    {
                        await _roomCollection.DeleteOneAsync(r => r.Id == room.Id);
                    });
                    RoomProfile newRoom = new RoomProfile
                    {
                        UserID = roleCode == 300 ? userID.ToString() : null,
                        ConsultantID = roleCode == 200 ? userID.ToString() : null,
                    };
                    await _roomCollection.InsertOneAsync(newRoom);
                    return new ApiResponse<RoomProfile>
                    {
                        Success = true,
                        Message = "Created new room.",
                        Data = newRoom,
                        Status = (int)HttpStatusCode.OK,
                    };
                }
                else
                {
                    return new ApiResponse<RoomProfile>
                    {
                        Success = true,
                        Message = "Got 1 room.",
                        Data = rooms.FirstOrDefault(),
                        Status = (int)HttpStatusCode.OK,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<RoomProfile>
                {
                    Success = false,
                    Message = "Chat Service - Get Room By User: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<MessageProfile>> SendMessage(Guid userID, string roomID, string message)
        {
            try
            {
                await Task.CompletedTask;
                MessageProfile newMessage = new MessageProfile
                {
                    UserID = userID.ToString(),
                    RoomID = roomID,
                    Message = message,
                    Status = "Sent",
                    DateSent = DateTime.Now,
                };

                await _messageCollection.InsertOneAsync(newMessage);
                return new ApiResponse<MessageProfile>
                {
                    Success = true,
                    Message = "Sent 1 message",
                    Data = newMessage,
                    Status = (int)HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MessageProfile>
                {
                    Success = false,
                    Message = "Chat Service - Send Message: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<bool>> UpdateReadMessage(string roomID)
        {
            try
            {
                await Task.CompletedTask;
                var filter = Builders<MessageProfile>.Filter.Eq("RoomID", roomID);
                var update = Builders<MessageProfile>.Update.Set(m => m.Status, "Read");
                await _messageCollection.UpdateManyAsync(filter, update);
                return new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Updated all messages.",
                    Data = true,
                    Status = (int)HttpStatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Chat Service - Update Read Message: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
