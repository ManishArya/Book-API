using System.Collections.Generic;
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

        [BsonIgnoreIfNull]
        public IEnumerable<Book> Books { get; set; }
    }
}