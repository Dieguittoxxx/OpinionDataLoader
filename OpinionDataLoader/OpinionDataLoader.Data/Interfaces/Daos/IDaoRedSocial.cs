using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Daos;

public interface IDaoRedSocial
{
    Task<OperationResult> CargarRedSocialAsync(RedSocial redSocial);
}
