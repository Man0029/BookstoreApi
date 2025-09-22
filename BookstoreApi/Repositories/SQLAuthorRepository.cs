using BookstoreApi.Data;
using BookstoreApi.Models.Domain;
using BookstoreApi.Models.DTO;

namespace BookstoreApi.Repositories
{
    public class SQLAuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _dbContext;
        public SQLAuthorRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<AuthorDTO> GellAllAuthors()
        {
            var allAuthors = _dbContext.Authors.Select(Authors => new AuthorDTO()
            {
                Id = Authors.Id,
                FullName = Authors.FullName,
            }).ToList();
            return allAuthors;
        }
        public AuthorNoIdDTO GetAuthorById(int id)
        {
            var authorWithDomain = _dbContext.Authors.Where(n => n.Id == id);
            //Map Domain Model to DTOs
            var authorNoIdDTO = authorWithDomain.Select(author => new AuthorNoIdDTO()
            {
                FullName = author.FullName,
            }).FirstOrDefault();
            return authorNoIdDTO;
        }
        public AddAuthorRequestDTO AddAuthor(AddAuthorRequestDTO addAuthorRequestDTO)
        {
            //map DTO to Domain Model
            var authorDomainModel = new Author
            {
                FullName = addAuthorRequestDTO.FullName,
            };
            _dbContext.Authors.Add(authorDomainModel);
            _dbContext.SaveChanges();
            return addAuthorRequestDTO;
        }
        public AuthorNoIdDTO UpdateAuthorById(int id, AuthorNoIdDTO authorNoIdDTO)
        {            
            var authorDomainModel = _dbContext.Authors.Find(id);
            if (authorDomainModel != null)
            {
                authorDomainModel.FullName = authorNoIdDTO.FullName;
                _dbContext.SaveChanges();
                return authorNoIdDTO;
            }
            return null;
        }        
        public Author? DeleteAuthorById(int id)
        {            
            var authorDomainModel = _dbContext.Authors.Find(id);
            if (authorDomainModel != null)
            {                
                _dbContext.Authors.Remove(authorDomainModel);
                _dbContext.SaveChanges();
                return authorDomainModel;
            }           
            return null;
        }
    }
}
