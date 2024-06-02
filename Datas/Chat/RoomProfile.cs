using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BlossomServer.Datas.Chat
{
    public class RoomProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserID { get; set; }
        public string? ConsultantID { get; set; }
    }
}
