using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.models
{
    public class Book : Base
    {
        [Required(ErrorMessage = "nameRequired")]
        public string Name { get; set; }

        [Required(ErrorMessage = "dateRequired")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ReleaseDate { get; set; }

        [Required(ErrorMessage = "descriptionRequired")]
        [MaxLength(1000, ErrorMessage = "descriptionMaxLength")]
        public string Description { get; set; }

        [Required(ErrorMessage = "genreRequired")]
        [MinLength(1, ErrorMessage = "genreMinLength")]
        public string[] Genres { get; set; }

        public byte[] Poster { get; set; }

        [Required(ErrorMessage = "priceRequired")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}