using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Services;

public interface IServiceSurvey
{
    Task<OperationResultLoad> CargarSurveyServiceAsync();
}