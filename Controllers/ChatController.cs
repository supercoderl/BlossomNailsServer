using BlossomServer.Datas.Chat;
using BlossomServer.Services.ChatServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlossomServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListMessages()
        {
            var result = await _chatService.GetListMessages();
            return StatusCode(result.Status, result);
        }

        [HttpGet("get-room-by-user")]
        public async Task<IActionResult> GetRoomByUser([FromQuery]RoomParam param)
        {
            var result = await _chatService.GetRoomByUser(param.UserID, param.RoleCode);
            return StatusCode(result.Status, result);
        }

        [HttpGet("get-all-rooms")]
        public async Task<IActionResult> GetAllRooms()
        {
            var result = await _chatService.GetAllRooms();
            return StatusCode(result.Status, result);
        }

        [HttpGet("get-message-by-room")]
        public async Task<IActionResult> GetMessagesByUserAndRoom([FromQuery] string roomID)
        {
            var result = await _chatService.GetMessageFromConversation(roomID);
            return StatusCode(result.Status, result);
        }

        [HttpGet("get-message")]
        public async Task<IActionResult> GetMessage([FromQuery] MessageParam param)
        {
            var result = await _chatService.GetMessage(param);
            return StatusCode(result.Status, result);
        }

        [HttpPut("update-read-message/{roomID}")]
        public async Task<IActionResult> UpdateReadMessage(string roomID)
        {
            var result = await _chatService.UpdateReadMessage(roomID);
            return StatusCode(result.Status, result);
        }
    }
}
