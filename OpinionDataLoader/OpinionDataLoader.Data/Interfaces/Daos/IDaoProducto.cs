using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Daos;

public interface IDaoProducto
{
    Task<OperationResult> CargarProductoAsync(Producto producto, string categoria);
}
