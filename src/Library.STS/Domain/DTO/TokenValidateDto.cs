using Newtonsoft.Json;

namespace Library.STS.Domain.DTO
{
    public class TokenValidateDto
    {
        [JsonProperty("valid_token")]
        public bool IsValidToken { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
