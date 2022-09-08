using AutoMapper;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Library.API.Controllers.V1
{
    [Authorize]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]   
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private IAuthorRepository repository;
        private IMapper mapper;
        private ILogger<AuthorsController> logger;

        public AuthorsController(IAuthorRepository repository,                                  
                                 ILogger<AuthorsController> logger,
                                 IMapper mapper)
        {
            this.repository = repository;
            this.logger = logger;
            this.mapper = mapper;
        }
      
        // GET api/authors
        [AllowAnonymous]
        [ResponseWithLink]
        [HttpGet(Name = "GetAuthors")]
        public ActionResult<PagedResponse<AuthorDTO>> GetAuthors([FromQuery]PaginationParams pagination)
        {          
            IQueryable<Author> response = this.repository.GetAllAsQueryable();
            
            var result = new PagedResponse<AuthorDTO>().ToPagedResponse(response, pagination, this.mapper.Map<IEnumerable<AuthorDTO>>);
                                                  
            return Ok(result);
        }

        // GET api/authors/0495b152-dbee-42fe-8231-08da6aceb26c
        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetAuthorsById")]
        public ActionResult<AuthorDTO> GetAuthorsById(Guid id)
        {
            Author author = this.repository.GetById(id);

            logger.LogInformation($"Getting author id {id}.");

            if (author == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The author id {id} was not found");
                logger.LogError($"{error.StatusCode} - {error.Message}.\r\nDetails: {error.Details}");
                return NotFound(error);
            }
            
            AuthorDTO authorDTO = this.mapper.Map<AuthorDTO>(author);

            logger.LogDebug($"{authorDTO.Id} - {authorDTO.Name}.");

            return Ok(authorDTO);
        }

        // GET api/authors/0495b152-dbee-42fe-8231-08da6aceb26c/books  
        [AllowAnonymous]
        [HttpGet("{id}/books", Name = "GetAuthorBooks")]
        public ActionResult<IEnumerable<BookDTO>> GetAuthorhBooks(Guid id)
        {
            Author author = this.repository.GetById(id);

            if (author == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The author id {id} was not found");
                return NotFound(error);
            }
            else
            {
                IEnumerable<Book> authorBooks = this.repository.GetAuthorsBooksById(id);
                //Connvert to Data Transfer Object
                IEnumerable<BookDTO> authorBooksDTO = this.mapper.Map<IEnumerable<BookDTO>>(authorBooks);

                return Ok(authorBooksDTO);
            }          
        }

        // POST api/authors
        [HttpPost(Name = "AddAuthor")]
        public IActionResult AddAuthor([FromBody] AuthorCreationDTO authorDto)
        {
            if (authorDto == null)
            {
                return BadRequest("The author cannot be null");
            }

            Author authorModel = this.mapper.Map<Author>(authorDto);
            authorModel.Id = Guid.NewGuid(); 

            // Check if the authors already exists
            IEnumerable<Author> lstAuthors = this.repository
                                             .GetWhere((item) => item.FirstName.ToLower().Trim().Equals(authorModel.FirstName.ToLower().Trim()) && 
                                                                 item.LastName.ToLower().Trim().Equals(authorModel.LastName.ToLower().Trim()));

            if (lstAuthors.Any())
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The author, {authorModel.FirstName} {authorModel.LastName}, already exists.");
                return BadRequest(error);
            }
            this.repository.Add(authorModel);
            this.repository.Save();

            return CreatedAtRoute("GetAuthorsById", new { id = authorModel.Id }, null);
        }

        // PUT api/authors/0495b152-dbee-42fe-8231-08da6aceb26c
        [HttpPut("{id}", Name = "UpdateAuthors")]
        public IActionResult UpdateAuthors(Guid id, [FromBody] AuthorUpdateDTO author)
        {
            Author _author = this.repository.GetAllAsQueryable().Where(a => a.Id == id).FirstOrDefault();            

            if (_author == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The author id {id} was not found");
                return NotFound(error);
            }
            else
            {
                Author authorToUpdate = this.mapper.Map<Author>(author);
                authorToUpdate.Id = _author.Id;

                this.repository.Udate(authorToUpdate);
                this.repository.Save();

                return Ok();
            }                                
        }
               
        // DELETE api/authors/0495b152-dbee-42fe-8231-08da6aceb26c
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            // Check if the author already exist
            Author authorById = this.repository.GetWhere((item) => item.Id.Equals(id)).FirstOrDefault();

            if (authorById == null)
            {
                ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, $"The author, {id}, was not found.");
                return NotFound(error);
            }

            this.repository.DeleteById(authorById.Id);
            this.repository.Save();

            return Ok();
        }
    }
}
