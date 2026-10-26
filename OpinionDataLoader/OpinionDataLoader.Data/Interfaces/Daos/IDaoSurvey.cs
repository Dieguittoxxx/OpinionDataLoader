using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Daos;

public interface IDaoSurvey
{
    Task<OperationResultLoad> CargarSurveysAsync(List<Surveys> registros, int idFuenteDatos);
}
