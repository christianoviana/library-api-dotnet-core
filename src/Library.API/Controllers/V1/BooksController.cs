using AutoMapper;
using Library.API.Configuration;
using Library.API.Domain.DTO;
using Library.API.Domain.Models;
using Library.API.Domain.Parameters;
using Library.API.Domain.Results;
using Library.API.Domain.Results.Exceptions;
using Library.API.Filters;
using Library.API.Infrastructure.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Library.API.Controllers.V1
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private IBookRepository repository;
        private IMapper mapper;
        private EnableFlags enableFlags;
        private ILogger<BooksController> logger;
        private IRepository<BookAuthor> repositoryBookAuthor;

        public BooksController(IBookRepository repository,
                               IRepository<BookAuthor> repositoryBookAuthor,
                               IMapper mapper, 
                               ILogger<BooksController> logger, 
                               IOptions<EnableFlags> options)
        {
            this.repository = repository;
            this.repositoryBookAuthor = repositoryBookAuthor;
            this.mapper = mapper;
            this.logger = logger;
            this.enableFlags = options.Value;
        }

        // GET api/books
        [AllowAnonymous]
        [ResponseWithLink]
        [HttpGet(Name = "GetBooks")]
        public ActionResult<PagedResponse<BookWithAuthorsDTO>> GetBooks([FromQuery]PaginationParams pagination)
        {
            IQueryable<Book> lstBookDTO = this.repository.GetBooksWithAuthorsAsQueryable();
            PagedResponse<BookWithAuthorsDTO> pagedResponse = new PagedResponse<BookWithAuthorsDTO>();
            pagedResponse.ToPagedResponse(lstBookDTO, pagination, this.mapper.Map<IEnumerable<BookWithAuthorsDTO>>);
            
            return Ok(pagedResponse);
        }

        // GET api/books/60B346DD-3AF9-40BF-8A3E-EFF5A1F1C853
        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetBooksById")]
        public ActionResult<BookWithAuthorsDTO> GetBooksById(Guid id)
        {            
            Book book = this.repository.GetBooksWithAuthorsById(id);

            logger.LogInformation($"Getting book id {id}.");

            if (book == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The book id {id} was not found");
                logger.LogError($"{error.StatusCode} - {error.Message}.\r\nDetails: {error.Details}");
                return NotFound(error);
            }

            BookWithAuthorsDTO bookDTO = this.mapper.Map<BookWithAuthorsDTO>(book);

            logger.LogDebug($"{bookDTO.Book.Id} - {bookDTO.Book.Name}.");

            return Ok(bookDTO);
        }
       
        // POST api/books
        [HttpPost(Name = "AddBook")]
        public IActionResult AddBook([FromBody] BookDTO bookDto)
        {
            if (bookDto == null)
            {
                return BadRequest("The book cannot be null");
            }

            Book bookModel = this.mapper.Map<Book>(bookDto);
            bookModel.Id = Guid.NewGuid(); 
            
            // Check if the books already exists
            IEnumerable<Book> lstBooks = this.repository
                                             .GetWhere((item) => item.Name.ToLower().Trim().Equals(bookModel.Name.ToLower().Trim()));
            
            if (lstBooks.Any())
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The book, {bookModel.Name}, already exists.");
                return BadRequest(error);
            }
            this.repository.Add(bookModel);
            this.repository.Save();

            return CreatedAtRoute("GetBooksById", new { id = bookModel.Id }, null);
        }

        // PUT api/books/60B346DD-3AF9-40BF-8A3E-EFF5A1F1C853/authors
        [HttpPut("{id}/authors", Name = "AddBookAuthors")]
        public IActionResult AddBookAuthors(Guid id, [FromBody] IEnumerable<AuthorCreationDTO> authors,
                                            [FromServices] IAuthorRepository authorRepository)
        {
            // Check if the book already exist
            Book bookById = this.repository.GetWhere((item) => item.Id.Equals(id)).FirstOrDefault();

            if (bookById == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The book, {id}, was not found.");
                return NotFound(error);
            }

            // Get the books' authors
            IEnumerable<Author> lstAuthors = this.mapper.Map<IEnumerable<Author>>(authors);

            #region ========= Check if the authors already exists =================
            IEnumerable<Author> lstCheckAuthors = lstAuthors.Where(e => 
            {
                if (e.Id != null && e.Id != Guid.Empty)
                    return false;

                var filterAuthors = authorRepository.GetWhere((item) => item.FirstName.ToLower().Trim().Equals(e.FirstName.ToLower().Trim()) &&
                                                                        item.LastName.ToLower().Trim().Equals(e.LastName.ToLower().Trim())).FirstOrDefault();

                return filterAuthors != null;
            }).ToList();

            if (lstCheckAuthors.Any())
            {
                string authorsAlreadyExists = String.Join(",", lstCheckAuthors.Select(e => string.Concat(e.FirstName, " ", e.LastName)));
                
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"Cannot create the authors, ({authorsAlreadyExists}), because them already exists.");
                return BadRequest(error);
            }
            #endregion

            foreach (var _author in lstAuthors)
            {
                if (_author.Id == null || _author.Id == Guid.Empty)
                {
                    _author.Id = Guid.NewGuid();
                    this.repositoryBookAuthor.Add(new BookAuthor() { Author = _author, Book = bookById });
                }
                else
                {
                    Author author = authorRepository.GetWhere((item) => item.Id.Equals(_author.Id)).FirstOrDefault();

                    if (author != null)
                    {
                        BookAuthor bookAuthor = this.repositoryBookAuthor.GetWhere((item) => item.BookId.Equals(bookById.Id) &&
                                                                                             item.AuthorId.Equals(author.Id)).FirstOrDefault();

                        if (bookAuthor != null)
                        {
                            ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The author id, {author.Id}, already has been added to book id {bookById.Id}.");
                            return BadRequest(error);
                        }

                        this.repositoryBookAuthor.Add(new BookAuthor() { AuthorId = author.Id, BookId = bookById.Id });
                    }
                    else
                    {
                        ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The author, {id}, was not found.");
                        return NotFound(error);
                    }
                }                   
            }

            this.repositoryBookAuthor.Save();

            return Ok();            
        }

        // PUT api/books/60B346DD-3AF9-40BF-8A3E-EFF5A1F1C853
        [HttpPut("{id}", Name = "UpdateBooks")]
        public IActionResult UpdateBooks(Guid id, [FromBody] BookDTO bookDto)
        {
            // Check if the book already exist
            Book bookById = this.repository.GetWhere((item) => item.Id.Equals(id)).FirstOrDefault();

            if (bookById == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The book, {id}, was not found.");
                return NotFound(error);
            }

            if (bookDto == null)
                return BadRequest("The book cannot be null");

            Book bookModel = this.mapper.Map<Book>(bookDto);
            bookModel.Id = bookById.Id;
           
            this.repository.Udate(bookModel);
            this.repository.Save();

            return Ok();
        }

        // DELETE api/books/60B346DD-3AF9-40BF-8A3E-EFF5A1F1C853
        [HttpDelete("{id}", Name = "DeleteBooksById")]
        public IActionResult DeleteBooksById(Guid id)
        {
            // Check if the book already exist
            Book bookById = this.repository.GetWhere((item) => item.Id.Equals(id)).FirstOrDefault();

            if (bookById == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The book, {id}, was not found.");
                return NotFound(error);
            }

            this.repository.DeleteById(bookById.Id);
            this.repository.Save();

            return Ok();
        }

        // DELETE api/books/60B346DD-3AF9-40BF-8A3E-EFF5A1F1C853/authors/0495b152-dbee-42fe-8231-08da6aceb26c
        [HttpDelete("{book_id}/authors/{author_id}", Name = "DeleteBookAuthorsById")]
        public IActionResult DeleteBookAuthorsById(Guid book_id, Guid author_id)
        {
            // Check if the book already exist
            Book bookById = this.repository.GetWhere((item) => item.Id.Equals(book_id)).FirstOrDefault();

            if (bookById == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The book, {book_id}, was not found.");
                return NotFound(error);
            }

            // Check if the author exist
            BookAuthor bookAuthor = this.repositoryBookAuthor.GetWhere((item) => item.Author.Id.Equals(author_id) &&
                                                                                 item.Book.Id.Equals(book_id)).FirstOrDefault();

            if (bookAuthor == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The author id {author_id} was not found.");
                return NotFound(error);
            }

            this.repositoryBookAuthor.Delete(bookAuthor);
            this.repositoryBookAuthor.Save();

            return Ok();
        }
    }
}
