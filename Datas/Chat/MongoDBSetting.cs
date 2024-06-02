namespace BlossomServer.Datas.Chat
{
    public class MongoDBSetting
    {
        public string? DefaultConnection { get; set; } = null;
        public string? DatabaseName { get; set; } = null;
        public string? ChatCollection { get; set; } = null;
        public string? RoomCollection { get; set; } = null;
        public string? MessageCollection { get; set; } = null;
    }
}
