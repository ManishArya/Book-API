using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.models
{
    public class Book : Base
    {
        [Required(ErrorMessage = "name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "release date is required.")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "description is required.")]
        [MaxLength(200, ErrorMessage = "description is allowed upto 200 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "genre is required.")]
        [MinLength(1)]
        public string[] Genres { get; set; }

        public byte[] Poster { get; set; }
    }
}