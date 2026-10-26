using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Daos;

public interface IDaoWebReview
{
    Task<OperationResultLoad> CargarWebReviewsAsync(List<WebReviews> registros, int idFuenteDatos);
}
