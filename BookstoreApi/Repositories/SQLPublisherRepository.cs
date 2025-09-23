using BookstoreApi.Data;
using BookstoreApi.Models.Domain;
using BookstoreApi.Models.DTO;

namespace BookstoreApi.Repositories
{
    public class SQLPublisherRepository : IPublisherRepository
    {
        private readonly AppDbContext _dbContext;
        public SQLPublisherRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<PublisherDTO> GetAllPublishers()
        {
            var allPublishers = _dbContext.Publishers.Select(publisher => new PublisherDTO()
            {
                Id = publisher.Id,
                Name = publisher.Name,
            }).ToList();
            return allPublishers;
        }
        public PublisherNoIdDTO GetPublisherById(int id)
        {
            var publisherWithDomain = _dbContext.Publishers.Where(n => n.Id == id);
            //Map Domain Model to DTOs
            var publisherWithIdDTO = publisherWithDomain.Select(publisher => new PublisherNoIdDTO()
            {
                Name = publisher.Name,
            }).FirstOrDefault();
            return publisherWithIdDTO!;
        }
        public AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO)
        {
            //map DTO to Domain Model
            var publisherDomainModel = new Publisher
            {
                Name = addPublisherRequestDTO.Name,
            };
            _dbContext.Publishers.Add(publisherDomainModel);
            _dbContext.SaveChanges();
            //map Domain Model to DTO
            return new AddPublisherRequestDTO
            {
                Name = publisherDomainModel.Name,
            };
        }
        public PublisherNoIdDTO UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO)
        {
            var publisherDomainModel = _dbContext.Publishers.Find(id);
            if (publisherDomainModel != null)
            {
                publisherDomainModel.Name = publisherNoIdDTO.Name;
                _dbContext.SaveChanges();
                //map Domain Model to DTO
                return new PublisherNoIdDTO
                {
                    Name = publisherDomainModel.Name,
                };
            }
            return null!;
        }
        public Publisher? DeletePublisherById(int id)
        {
            var publisherDomainModel = _dbContext.Publishers.Find(id);
            if (publisherDomainModel != null)
            {
                _dbContext.Publishers.Remove(publisherDomainModel);
                _dbContext.SaveChanges();
                return publisherDomainModel;
            }
            return null;
        }
    }
}
