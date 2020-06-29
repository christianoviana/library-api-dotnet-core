using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library.API.Domain.DTO
{
    public class AuthorUpdateDTO
    {
        [Required]
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [Required]
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [Required]
        [JsonProperty("birthday")]
        public DateTime? Birthday { get; set; }
    }
}
