using BookstoreApi.Data;
using BookstoreApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public AuthorsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
        public IActionResult Add(Models.Domain.Author author)
        {
            _dbContext.Authors.Add(author);
            _dbContext.SaveChanges();
            return CreatedAtAction("GetById", new { id = author.Id }, author);
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
    }
}
