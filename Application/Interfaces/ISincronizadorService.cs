using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Primitives;

namespace Application.Interfaces
{
    public interface ISincronizadorService
    {
        Task<OperationResult<int>> SincronizarAsync(ConsolidadoJob job, ConsolidadoArchivo archivo, IEnumerable<VentaDetallada> ventas, CancellationToken cancellationToken = default);
    }
}
