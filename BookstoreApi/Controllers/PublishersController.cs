using BookstoreApi.Data;
using BookstoreApi.Models.Domain;
using BookstoreApi.Models.DTO;
using BookstoreApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace BookstoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly AppDbContext _appdbContext;
        private readonly IPublisherRepository _publisherRepository;
        public PublishersController(IPublisherRepository publisherRepository, AppDbContext appDbContext)
        {
            _appdbContext = appDbContext;
            _publisherRepository = publisherRepository;
        }
        [HttpGet]
        public IActionResult GetAllPublishers()
        {
            var publishers = _publisherRepository.GetAllPublishers();
            return Ok(publishers);
        }
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetPublisherById([FromRoute] int id)
        {
            var publisher = _publisherRepository.GetPublisherById(id);
            if (publisher == null)
            {
                return NotFound();
            }
            return Ok(publisher);
        }
        [HttpPost]
        [Route("Add")]
        public IActionResult AddPublisher([FromBody] AddPublisherRequestDTO addPublisherRequestDTO)
        {
            if(ValidateAddPublisher(addPublisherRequestDTO))
            {
                var pubadd = _publisherRepository.AddPublisher(addPublisherRequestDTO);
                return Ok(pubadd);
            }

            return BadRequest(ModelState);
        }
        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdatePublisherById([FromRoute] int id, [FromBody] PublisherNoIdDTO publisherNoIdDTO)
        {
            var updatedPublisher = _publisherRepository.UpdatePublisherById(id, publisherNoIdDTO);
            if (updatedPublisher == null)
            {
                return NotFound();
            }
            return Ok(updatedPublisher);
        }
        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeletePublisherById([FromRoute] int id)
        {
            
            if (ValidateDeletePublisher(id))
            {
                var deletedPublisher = _publisherRepository.DeletePublisherById(id);
                return Ok();
            }
            return BadRequest(ModelState);
        }
        #region Private Methods
        private bool ValidateAddPublisher(AddPublisherRequestDTO a)
        {
            if (a == null)
            {
                ModelState.AddModelError(nameof(a), $"Please add Pub data");
                return false;
            }
            // kiem tra 
            if (string.IsNullOrEmpty(a.Name)|| a.Name.Length < 5)
            {
                ModelState.AddModelError(nameof(a.Name), $"{nameof(a.Name)} must be more 5 letter");
            }
            if (_appdbContext.Publishers.Any(p => p.Name != null && EF.Functions.Like(p.Name, a.Name)))
            {
                ModelState.AddModelError(nameof(a.Name), $"{nameof(a.Name)} has a Pub same name");
            }

            if (ModelState.ErrorCount > 0)
            {
                return false;
            }
            return true;
        }

        private bool ValidateDeletePublisher(int id)
        {
            if (!_appdbContext.Publishers.Any(p => p.Id == id))
            {
                ModelState.AddModelError(nameof(id), $"No Pub with id {id}");
            }
            if (_appdbContext.Books.Any(b => b.PublisherId == id))
            {
                ModelState.AddModelError(nameof(id), $"Cannot delete this Pub because it has related books");
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
