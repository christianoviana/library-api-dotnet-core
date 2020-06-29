using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library.API.Domain.DTO
{
    public class BookDTO
    {       
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [Required]
        [JsonProperty("name")]
        public String Name { get; set; }

        [Required]
        [JsonProperty("description")]
        public String Description { get; set; }

        [Required]
        [JsonProperty("pages")]
        public int Pages { get; set; }

        [Required]
        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}
