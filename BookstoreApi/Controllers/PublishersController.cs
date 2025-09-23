using BookstoreApi.Data;
using BookstoreApi.Models.Domain;
using BookstoreApi.Models.DTO;
using BookstoreApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var newPublisher = _publisherRepository.AddPublisher(addPublisherRequestDTO);
            if(newPublisher == null)
            {
                return NotFound();
            }
            return Ok(newPublisher);
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
            var deletedPublisher = _publisherRepository.DeletePublisherById(id);
            if (deletedPublisher == null)
            {
                return NotFound();
            }
            return Ok(deletedPublisher);
        }
    }
}
