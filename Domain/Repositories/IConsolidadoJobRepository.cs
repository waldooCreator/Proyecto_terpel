using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Primitives;

namespace Domain.Repositories
{
    public interface IConsolidadoJobRepository
    {
        Task<OperationResult<int>> CrearJobAsync(ConsolidadoJob job, CancellationToken cancellationToken = default);
    }
}
