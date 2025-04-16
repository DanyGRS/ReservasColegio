using ReservasColegio.Models;
using System.Security.Claims;

namespace ReservasColegio.Services
{
    public interface IAuthService
    {
        string GerarToken(Usuario usuario);
        ClaimsPrincipal GerarClaimsPrincipal(Usuario usuario);
    }
}
