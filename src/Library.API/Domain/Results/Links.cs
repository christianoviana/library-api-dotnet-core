using Newtonsoft.Json;
using System;

namespace Library.API.Domain.Results
{
    public class Links
    {
        public Links()
        {
        }

        [JsonProperty("first_page")]
        public Uri FirstPage { get; set; }
        [JsonProperty("next_page")]
        public Uri NextPage { get; set; }
        [JsonProperty("previous_page")]
        public Uri PreviousPage { get; set; }
        [JsonProperty("last_page")]
        public Uri LastPage { get; set; }       
    }
}
