using System.IO;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITerpelClient
    {
        // Consulta la URL dinámica (placeholder)
        Task<string> GetDynamicUrlAsync(string dynamicUrlPlaceholder);

        // Descarga el archivo desde la URL final
        Task<Stream> DownloadFileAsync(string fileUrl);
    }
}
