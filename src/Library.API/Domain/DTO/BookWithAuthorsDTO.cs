using Newtonsoft.Json;
using System.Collections.Generic;

namespace Library.API.Domain.DTO
{
    public class BookWithAuthorsDTO
    {
        [JsonProperty("book")]
        public BookDTO Book { get; set; }
       
        [JsonProperty("authors")]
        public IEnumerable<AuthorDTO> Authors { get; set; }
    }
}
