using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Daos;

public interface IDaoSocialComment
{
    Task<OperationResultLoad> CargarSocialCommentsAsync(List<SocialComments> registros, int idFuenteDatos);
}
