using System;
using System.Collections.Generic;

namespace Library.API.Domain.Models
{
    public class Author
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public virtual IEnumerable<BookAuthor> BookAuthors { get; set; }
    }
}
