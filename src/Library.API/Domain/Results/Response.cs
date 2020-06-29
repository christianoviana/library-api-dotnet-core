using Newtonsoft.Json;

namespace Library.API.Domain.Results
{
    public class Response<T>
    {
        [JsonProperty("data", Order = 1)]
        public T Data { get; set; }
    }
}
