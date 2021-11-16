using System.Threading.Tasks;
using DotNet_API.Domain.Entities;

namespace DotNet_API.Application.Repositories
{
    public interface IIdentityRepository
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password);
    }
}