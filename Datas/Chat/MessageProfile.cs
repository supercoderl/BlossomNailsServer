using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BlossomServer.Datas.Chat
{
    public class MessageProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string RoomID { get; set; }
        public string UserID { get; set; }
        public string Message {  get; set; }
        public string Status { get; set; }
        public DateTime DateSent { get; set; }
    }
}
