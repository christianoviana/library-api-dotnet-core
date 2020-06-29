using Newtonsoft.Json;

namespace Library.API.Domain.Results.Exceptions
{
    public class ErrorMessage
    {
        public ErrorMessage(int statusCode, string message, string details = "")
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("details")]
        public string Details { get; set; }
    }
}
