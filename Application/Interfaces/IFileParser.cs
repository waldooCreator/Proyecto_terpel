using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IFileParser
    {
        Task<List<RegistroVentaDto>> ParseAsync(Stream fileStream, string fileName);
    }
}
