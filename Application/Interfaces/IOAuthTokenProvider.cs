using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOAuthTokenProvider
    {
        /// <summary>
        /// Obtiene un token de acceso OAuth2 (client_credentials) y lo cachea hasta su expiración.
        /// </summary>
        Task<string> GetAccessTokenAsync();
    }
}
