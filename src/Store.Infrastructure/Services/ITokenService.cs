
using Store.Infrastructure.Identity;

namespace Store.Infrastructure.Services;

public interface ITokenService
{
 Task<string> CreateTokenAsync(AppUser user);
}
