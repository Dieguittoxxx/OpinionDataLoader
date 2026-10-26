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

public sealed class ServiceSocialComment : ServiceSecondBase<SocialCommentCsv, SocialComments>, IServiceSocialComment
{
    private readonly IDaoSocialComment _daoSocialComment;

    public ServiceSocialComment(IDaoSocialComment daoSocialComment, OpinionesOLTPContext context, string filePath)
        : base(context, filePath)
    {
        _daoSocialComment = daoSocialComment;
    }

    public Task<OperationResultLoad> CargarSocialCommentServiceAsync() => CargarServiceAsync();

    protected override async Task<(List<SocialComments> Registros, int TotalFilas)> ExtraerRegistrosValidosAsync(
        int idFuenteDatos, List<SocialCommentCsv> registrosCsv)
    {
        var clientesValidos = new HashSet<int>(await _context.Cliente.Select(c => c.IdCliente).ToListAsync());
        var productosValidos = new HashSet<int>(await _context.Producto.Select(p => p.IdProducto).ToListAsync());
        var redesSociales = await _context.RedSocial
            .ToDictionaryAsync(r => r.Nombre, r => r.IdRedSocial, StringComparer.OrdinalIgnoreCase);

        HashSet<string> idsUnicos = [];
        List<SocialComments> registrosAEnviar = [];

        foreach (var record in registrosCsv)
        {
            if (string.IsNullOrWhiteSpace(record.IdCliente))
                continue;

            if (!idsUnicos.Add(record.IdComment))
                continue;

            if (!TryResolveId(record.IdCliente, "C", out int idCliente) || !clientesValidos.Contains(idCliente))
                continue;

            if (!TryResolveId(record.IdProducto, "P", out int idProducto) || !productosValidos.Contains(idProducto))
                continue;

            if (string.IsNullOrWhiteSpace(record.Fuente) || !redesSociales.TryGetValue(record.Fuente.Trim(), out int idRedSocial))
                continue;

            if (string.IsNullOrWhiteSpace(record.Comentario))
                continue;

            if (!DateOnly.TryParse(record.Fecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                continue;

            registrosAEnviar.Add(new SocialComments
            {
                IdOriginal = record.IdComment,
                IdCliente = idCliente,
                IdProducto = idProducto,
                IdRedSocial = idRedSocial,
                Fecha = fecha,
                Comentario = record.Comentario.Trim()
            });
        }

        return (registrosAEnviar, registrosCsv.Count);
    }

    protected override Task<OperationResultLoad> EjecutarCargaBatchAsync(List<SocialComments> registros, int idFuenteDatos)
        => _daoSocialComment.CargarSocialCommentsAsync(registros, idFuenteDatos);
}