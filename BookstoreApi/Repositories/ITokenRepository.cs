using Microsoft.AspNetCore.Identity;

namespace BookstoreApi.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
