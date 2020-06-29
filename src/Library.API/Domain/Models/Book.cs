using System;
using System.Collections.Generic;

namespace Library.API.Domain.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public int Pages { get; set; }
        public DateTime Date { get; set; }       
        public virtual IEnumerable<BookAuthor> BookAuthors { get; set; }
    }
}
