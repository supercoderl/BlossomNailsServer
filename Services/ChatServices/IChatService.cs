using BlossomServer.Datas.Chat;
using BlossomServer.Response;

namespace BlossomServer.Services.ChatServices
{
    public interface IChatService
    {
        Task<ApiResponse<List<ChatProfile>>> GetListMessages();
        Task<ApiResponse<RoomProfile>> GetRoomByUser(Guid userID, int roleCode);
        Task<ApiResponse<List<RoomProfile>>> GetAllRooms();
        Task<ApiResponse<MessageProfile>> SendMessage(Guid userID, string roomID, string message);
        Task<ApiResponse<List<MessageProfile>>> GetMessageFromConversation(string roomID);
        Task<ApiResponse<MessageProfile>> GetMessage(MessageParam param);
        Task<ApiResponse<bool>> UpdateReadMessage(string roomID);
    }
}
