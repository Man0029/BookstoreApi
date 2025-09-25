using BookstoreApi.Data;
using BookstoreApi.Models.DTO;
using BookstoreApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;


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
        public IActionResult Update(int id, AuthorNoIdDTO nameauthor)
        {
            var UpdateAuthor = _authorsRepository.UpdateAuthorById(id, nameauthor);
            if (UpdateAuthor != null)
            {
                return Ok();
            }
            return NotFound();

        }
        [HttpDelete("Delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            if (ValidateDeleteAuthor(id))
            {
                var author = _authorsRepository.DeleteAuthorById(id);
                return Ok(author);
            }
            return BadRequest(ModelState);
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

        private bool ValidateDeleteAuthor(int id)
        {
            var existingAuthor = _dbContext.Authors.Find(id);
            if (existingAuthor == null)
            {
                ModelState.AddModelError(nameof(id), $"Not found author with id={id}");
                return false;
            }
            // kiem tra ràng buộc khóa ngoại với bảng Book_Author
            bool hasRelatedBooks = _dbContext.Book_Authors.Any(ba => ba.AuthorId == id);
            if (hasRelatedBooks)
            {
                ModelState.AddModelError(nameof(id), $"Cannot delete author with id={id} because it is referenced by existing books.");
                return false;
            }
            return true;
        }
        #endregion
    }
}
