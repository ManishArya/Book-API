using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.models
{
    public class MyList : Base
    {
        [JsonIgnore]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BookId { get; set; }

        public string Username { get; set; }

        public int Quantity {get; set;}

        [BsonIgnoreIfNull]
        public Book Book { get; set; }
    }
}