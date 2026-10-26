using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Daos;

public interface IDaoCategoria
{
    Task<OperationResult> CargarCategoriaAsync(Categoria categoria);
}
