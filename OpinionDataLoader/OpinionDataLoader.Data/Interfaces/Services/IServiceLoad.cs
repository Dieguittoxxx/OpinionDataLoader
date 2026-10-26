using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Services;

public interface IServiceLoad
{
    Task<Dictionary<string, OperationResultLoad>> EjecutarCargaAsync();
}