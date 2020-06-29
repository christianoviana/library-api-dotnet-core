using Newtonsoft.Json;

namespace Library.STS.Domain.DTO
{
    public class UserDto
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
