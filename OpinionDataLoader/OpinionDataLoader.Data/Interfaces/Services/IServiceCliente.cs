using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Services;

public interface IServiceCliente
{
    Task<OperationResultLoad> CargarClienteServiceAsync();
}