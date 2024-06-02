namespace BlossomServer.Datas.Chat
{
    public class RoomParam
    {
        public Guid UserID { get; set; }
        public int RoleCode { get; set; }
    }

    public class MessageParam
    {
        public Guid? UserID { get; set; }
        public string RoomID { get; set; }
        public bool? WannaLatestMessage { get; set; }
    }
}
