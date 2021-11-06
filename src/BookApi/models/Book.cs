using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.models
{
    public class Book : Base
    {
        [Required]
        public string Name { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ReleaseDate { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        [MinLength(1)]
        public string[] Genres { get; set; }

        public byte[] Poster { get; set; }
    }
}