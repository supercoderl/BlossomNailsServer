using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlossomServer.Datas.Chat
{
    public class ChatProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Sender { get; set; }
        public string? Message { get; set; }

    }
}
