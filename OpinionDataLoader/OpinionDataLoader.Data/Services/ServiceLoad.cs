using OpinionDataLoader.Data.Interfaces.Services;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.Services;

public sealed class ServiceLoad : IServiceLoad
{
    private readonly IServiceCategoria _serviceCategoria;
    private readonly IServiceCliente _serviceCliente;
    private readonly IServiceProducto _serviceProducto;
    private readonly IServiceWebReview _serviceWebReview;
    private readonly IServiceSocialComment _serviceSocialComment;
    private readonly IServiceSurvey _serviceSurvey;

    public ServiceLoad(
        IServiceCategoria serviceCategoria,
        IServiceCliente serviceCliente,
        IServiceProducto serviceProducto,
        IServiceWebReview serviceWebReview,
        IServiceSocialComment serviceSocialComment,
        IServiceSurvey serviceSurvey)
    {
        _serviceCategoria = serviceCategoria;
        _serviceCliente = serviceCliente;
        _serviceProducto = serviceProducto;
        _serviceWebReview = serviceWebReview;
        _serviceSocialComment = serviceSocialComment;
        _serviceSurvey = serviceSurvey;
    }

    public async Task<Dictionary<string, OperationResultLoad>> EjecutarCargaAsync()
    {
        return new Dictionary<string, OperationResultLoad>
        {
            ["Categoria"] = await _serviceCategoria.CargarCategoriaServiceAsync(),
            ["Cliente"] = await _serviceCliente.CargarClienteServiceAsync(),
            ["Producto"] = await _serviceProducto.CargarProductoServiceAsync(),
            ["WebReview"] = await _serviceWebReview.CargarWebReviewServiceAsync(),
            ["SocialComment"] = await _serviceSocialComment.CargarSocialCommentServiceAsync(),
            ["Survey"] = await _serviceSurvey.CargarSurveyServiceAsync()
        };
    }
}