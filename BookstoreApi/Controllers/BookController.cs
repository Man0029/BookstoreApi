using BookstoreApi.Data;
using BookstoreApi.Models.Domain;
using BookstoreApi.Models.DTO;
using BookstoreApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
//Past3
namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IBookRepository _bookRepository;
        public BookController(AppDbContext dbContext, IBookRepository bookRepository)
        {
            _dbContext = dbContext;
            _bookRepository = bookRepository;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAllBooks([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool isAscending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            var allBooks = _bookRepository.GetAllBooks(filterOn, filterQuery);
            return Ok(allBooks);
        }

        [HttpGet]
        [Route("get-book-by-id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookWithIdDTO = _bookRepository.GetBookById(id);
            return Ok(bookWithIdDTO);
        }

        [HttpPost("add-book")]
        [ValidateModel]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            if (ValidateAddBook(addBookRequestDTO))
            {
                var bookAdd = _bookRepository.AddBook(addBookRequestDTO);
                return Ok(bookAdd);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("update-book-by-id/{id}")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO bookDTO)
        {
            var updateBook = _bookRepository.UpdateBookById(id, bookDTO);
            return Ok(updateBook);
        }
        [HttpDelete("delete-book-by-id/{id}")]
        public IActionResult DeleteBookById(int id)
        {
            var deleteBook = _bookRepository.DeleteBookById(id);
            return Ok(deleteBook);
        }

        #region private methods
        private bool ValidateAddBook(AddBookRequestDTO addBookRequestDTO)
        {
            if (addBookRequestDTO == null)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO), $"Please add book data");
                return false;
            }

            // kiem tra Description NotNull
            if (string.IsNullOrEmpty(addBookRequestDTO.Description))
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Description), $"{nameof(addBookRequestDTO.Description)} cannot be null");
            }

            // kiem tra rating (0,5)
            if (addBookRequestDTO.Rate < 0 || addBookRequestDTO.Rate > 5)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Rate), $"{nameof(addBookRequestDTO.Rate)} cannot be less than 0 and more than 5");
            }

            // kiem tra ID cua publisher co ton tai trong DB khong
            var pubsearch = _dbContext.Publishers.Find(addBookRequestDTO.PublisherId);
            if (pubsearch == null)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.PublisherId), $"{nameof(addBookRequestDTO.PublisherId)} does not exist in database");
            }

            //kiem tra AuthorIds co ton tai trong DB khong
            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var authorSearch = _dbContext.Authors.Find(authorId);
                if (authorSearch == null)
                {
                    ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), $"{nameof(addBookRequestDTO.AuthorIds)} with id {authorId} does not exist in database");
                }
            }

            //1 author khong duoc gan nhieu lan trong AuthorIds
            var distinctAuthorIds = addBookRequestDTO.AuthorIds.Distinct().ToList();
            if (distinctAuthorIds.Count != addBookRequestDTO.AuthorIds.Count)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), $"{nameof(addBookRequestDTO.AuthorIds)} cannot have duplicate author IDs");
            }

            //moi book phai co 1 author
            if (addBookRequestDTO.AuthorIds.Count() == 0)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), "Book must be has a author");
            }

            //1 author là tối đa của 20 book
            foreach (var authorid in addBookRequestDTO.AuthorIds)
            {
                var author = _dbContext.Authors.Find(authorid);
                if (author == null)
                {
                    continue; // Author not found, skip to the next ID
                }
                var bookCount = _dbContext.Book_Authors.Count(ba => ba.AuthorId == authorid);
                if (bookCount >= 20)
                {
                    ModelState.AddModelError(nameof(addBookRequestDTO.AuthorIds), $"Author with ID {authorid} cannot be assigned to more than 20 books");
                }
            }

            //1 publisher là tối đa của 100 book trong 1nam qua(365day)
            var today = DateTime.Today;
            var ayearago = today.AddDays(-365);
            var booksAddedAYearAgoCount = _dbContext.Books.Count(b => b.PublisherId == addBookRequestDTO.PublisherId && b.DateAdded >= ayearago);
            if (booksAddedAYearAgoCount >= 100)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.PublisherId), $"Publisher with ID {addBookRequestDTO.PublisherId} cannot have more than 100 books added in the last year");
            }

            //title khong duoc trung trong publisher
            var isTitleExist = _dbContext.Books.Any(b => b.Title == addBookRequestDTO.Title && b.PublisherId == addBookRequestDTO.PublisherId);
            if (isTitleExist)
            {
                ModelState.AddModelError(nameof(addBookRequestDTO.Title), $"A book with the title '{addBookRequestDTO.Title}' already exists for the specified publisher.");
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
