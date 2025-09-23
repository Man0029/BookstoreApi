using BookstoreApi.Data;
using BookstoreApi.Models.DTO;
using BookstoreApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IAuthorRepository _authorsRepository;
        public AuthorsController(AppDbContext dbContext, IAuthorRepository authorRepository)
        {
            _dbContext = dbContext;
            _authorsRepository = authorRepository;
        }
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_dbContext.Authors.ToList());
        }
        [HttpGet("GetById/{id:int}")]
        public IActionResult GetById(int id)
        {
            var author = _dbContext.Authors.Find(id);
            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }
        [HttpPost("Add")]
        [ValidateModel]
        public IActionResult Add([FromBody] AddAuthorRequestDTO author)
        {
            if (ValidateAddAuthor(author))
            {
                var authoradd = _authorsRepository.AddAuthor(author);

                return Ok(authoradd);
            }
            return BadRequest(ModelState);
        }
        [HttpPut("Update/{id:int}")]
        public IActionResult Update(int id, Models.Domain.Author author)
        {
            var existingAuthor = _dbContext.Authors.Find(id);
            if (existingAuthor == null)
            {
                return NotFound();
            }
            existingAuthor.FullName = author.FullName;            
            _dbContext.SaveChanges();
            return NoContent();
        }
        [HttpDelete("Delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var existingAuthor = _dbContext.Authors.Find(id);
            if (existingAuthor == null)
            {
                return NotFound();
            }
            _dbContext.Authors.Remove(existingAuthor);
            _dbContext.SaveChanges();
            return NoContent();
        }
        #region private methods
        private bool ValidateAddAuthor(AddAuthorRequestDTO addauthorRequestDTO)
        {
            if (addauthorRequestDTO == null)
            {
                ModelState.AddModelError(nameof(addauthorRequestDTO), $"Please add author data");
                return false;
            }
            string fullNameTrimmed = addauthorRequestDTO.FullName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(addauthorRequestDTO.FullName)||fullNameTrimmed.Length<3)
            {
                ModelState.AddModelError(nameof(addauthorRequestDTO.FullName), $"Please add full name of author(more 3 char)");
            }
            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
