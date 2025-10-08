using BookstoreApi.Models.Domain;
namespace BookstoreApi.Repositories
{
    public interface IImageRepository
    {
        Image Upload(Image image);
        List<Image> GetAllInfoImage();
        (byte[], string, string)DownloadFile(int id);
    }
}
