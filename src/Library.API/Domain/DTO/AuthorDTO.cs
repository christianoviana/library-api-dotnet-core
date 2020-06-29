using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library.API.Domain.DTO
{
    public class AuthorDTO
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("age")]
        public int Age { get; set; }
    }
}
