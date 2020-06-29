using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace Library.API.Authorization.Service
{
    public interface IServiceSTS
    {
        AuthenticationTicket GetTicket(string token);
        Task<bool> IsValidTokenAsync(string token);
    }
}
