using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Primitives;

namespace Domain.Repositories
{
    public interface IVentaDetalladaRepository
    {
        Task<OperationResult<int>> CrearVentaAsync(VentaDetallada venta, CancellationToken cancellationToken = default);
        Task<OperationResult<int>> CrearVentasAsync(IEnumerable<VentaDetallada> ventas, CancellationToken cancellationToken = default);
    }
}
