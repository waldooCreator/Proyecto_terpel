using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Primitives;

namespace Domain.Repositories
{
    public interface IConsolidadoArchivoRepository
    {
        Task<OperationResult<int>> CrearArchivoAsync(ConsolidadoArchivo archivo, CancellationToken cancellationToken = default);
    }
}
