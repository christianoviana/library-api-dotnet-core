using Newtonsoft.Json;

namespace Library.STS.Domain.DTO
{
    public class TokenValueDto
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
