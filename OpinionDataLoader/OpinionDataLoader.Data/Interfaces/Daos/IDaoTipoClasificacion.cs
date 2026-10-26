using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Daos;

public interface IDaoTipoClasificacion
{
    Task<OperationResult> CargarTipoClasificacionAsync(TipoClasificacion tipoClasificacion);
}
