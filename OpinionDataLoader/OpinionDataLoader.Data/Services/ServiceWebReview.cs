using OpinionDataLoader.Data.Context;
using OpinionDataLoader.Data.Interfaces.Daos;
using OpinionDataLoader.Data.Interfaces.Services;
using OpinionDataLoader.Data.Models.Csv;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;
using OpinionDataLoader.Data.Services.Base;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace OpinionDataLoader.Data.Services;

public sealed class ServiceWebReview : ServiceSecondBase<WebReviewCsv, WebReviews>, IServiceWebReview
{
    private readonly IDaoWebReview _daoWebReview;

    public ServiceWebReview(IDaoWebReview daoWebReview, OpinionesOLTPContext context, string filePath)
        : base(context, filePath)
    {
        _daoWebReview = daoWebReview;
    }

    public Task<OperationResultLoad> CargarWebReviewServiceAsync() => CargarServiceAsync();

    protected override async Task<(List<WebReviews> Registros, int TotalFilas)> ExtraerRegistrosValidosAsync(
        int idFuenteDatos, List<WebReviewCsv> registrosCsv)
    {
        var clientesValidos = new HashSet<int>(await _context.Cliente.Select(c => c.IdCliente).ToListAsync());
        var productosValidos = new HashSet<int>(await _context.Producto.Select(p => p.IdProducto).ToListAsync());

        HashSet<string> idsUnicos = [];
        List<WebReviews> registrosAEnviar = [];

        foreach (var record in registrosCsv)
        {
            if (string.IsNullOrWhiteSpace(record.IdCliente))
                continue;

            if (!idsUnicos.Add(record.IdReview))
                continue;

            if (!TryResolveId(record.IdCliente, "C", out int idCliente) || !clientesValidos.Contains(idCliente))
                continue;

            if (!TryResolveId(record.IdProducto, "P", out int idProducto) || !productosValidos.Contains(idProducto))
                continue;

            if (string.IsNullOrWhiteSpace(record.Comentario))
                continue;

            if (!DateOnly.TryParse(record.Fecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                continue;

            registrosAEnviar.Add(new WebReviews
            {
                IdOriginal = record.IdReview,
                IdCliente = idCliente,
                IdProducto = idProducto,
                Fecha = fecha,
                Comentario = record.Comentario.Trim(),
                Rating = record.Rating
            });
        }

        return (registrosAEnviar, registrosCsv.Count);
    }

    protected override Task<OperationResultLoad> EjecutarCargaBatchAsync(List<WebReviews> registros, int idFuenteDatos)
        => _daoWebReview.CargarWebReviewsAsync(registros, idFuenteDatos);
}