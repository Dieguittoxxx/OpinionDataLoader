using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Services;

public interface IServiceCategoria
{
    Task<OperationResultLoad> CargarCategoriaServiceAsync();
}