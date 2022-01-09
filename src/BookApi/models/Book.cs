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
        [MaxLength(200, ErrorMessage = "description can not exceed 200 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "genres is required.")]
        [MinLength(1)]
        public string[] Genres { get; set; }

        public byte[] Poster { get; set; }
    }
}