using Newtonsoft.Json;
using System;

namespace Library.STS.Domain.DTO
{
    public class TokenInfoDto
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("created")]
        public DateTime? Created { get; set; }
        [JsonProperty("expires")]
        public DateTime? Expires { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
