using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Daos;

public interface IDaoCliente
{
    Task<OperationResult> CargarClienteAsync(Cliente cliente);
}
