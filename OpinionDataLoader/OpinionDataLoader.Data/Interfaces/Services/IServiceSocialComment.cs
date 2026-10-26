using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Interfaces.Services;

public interface IServiceSocialComment
{
    Task<OperationResultLoad> CargarSocialCommentServiceAsync();
}