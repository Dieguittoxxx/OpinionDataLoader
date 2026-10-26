using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Services;

public interface IServiceWebReview
{
    Task<OperationResultLoad> CargarWebReviewServiceAsync();
}