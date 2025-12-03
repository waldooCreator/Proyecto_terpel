using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IProcessingService
    {
        Task<ProcessResultDto> ProcessSyncAsync(ProcessingRequestDto request);
        Task<string> ProcessAsync(ProcessingRequestDto request); // returns transaction id immediately
    }
}
