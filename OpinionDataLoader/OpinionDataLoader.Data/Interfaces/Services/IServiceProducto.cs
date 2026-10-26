using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Services;

public interface IServiceProducto
{
    Task<OperationResultLoad> CargarProductoServiceAsync();
}