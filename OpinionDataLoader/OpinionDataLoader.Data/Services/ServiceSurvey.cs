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

public sealed class ServiceSurvey : ServiceSecondBase<SurveyCsv, Surveys>, IServiceSurvey
{
    private readonly IDaoSurvey _daoSurvey;

    private int _rechazadosDuplicado;
    private int _rechazadosCliente;
    private int _rechazadosProducto;
    private int _rechazadosClasificacion;
    private int _rechazadosTipoEncuesta;
    private int _rechazadosComentario;
    private int _rechazadosFecha;

    public ServiceSurvey(IDaoSurvey daoSurvey, OpinionesOLTPContext context, string filePath)
        : base(context, filePath)
    {
        _daoSurvey = daoSurvey;
    }

    public Task<OperationResultLoad> CargarSurveyServiceAsync() => CargarServiceAsync();

    protected override async Task<(List<Surveys> Registros, int TotalFilas)> ExtraerRegistrosValidosAsync(
        int idFuenteDatos, List<SurveyCsv> registrosCsv)
    {
        var clientesValidos = new HashSet<int>(await _context.Cliente.Select(c => c.IdCliente).ToListAsync());
        var productosValidos = new HashSet<int>(await _context.Producto.Select(p => p.IdProducto).ToListAsync());
        var clasificaciones = await _context.TipoClasificacion
            .ToDictionaryAsync(t => t.Descripcion, t => t.IdClasificacion, StringComparer.OrdinalIgnoreCase);
        var tiposEncuesta = await _context.TipoEncuesta
            .ToDictionaryAsync(t => t.Nombre, t => t.IdTipoEncuesta, StringComparer.OrdinalIgnoreCase);

        HashSet<int> idsUnicos = [];
        List<Surveys> registrosAEnviar = [];

        foreach (var record in registrosCsv)
        {
            if (!idsUnicos.Add(record.IdOpinion))
            {
                _rechazadosDuplicado++;
                continue;
            }

            if (!clientesValidos.Contains(record.IdCliente))
            {
                _rechazadosCliente++;
                continue;
            }

            if (!productosValidos.Contains(record.IdProducto))
            {
                _rechazadosProducto++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(record.Clasificacion) || !clasificaciones.TryGetValue(record.Clasificacion.Trim(), out int idClasificacion))
            {
                _rechazadosClasificacion++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(record.Fuente) || !tiposEncuesta.TryGetValue(record.Fuente.Trim(), out int idTipoEncuesta))
            {
                _rechazadosTipoEncuesta++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(record.Comentario))
            {
                _rechazadosComentario++;
                continue;
            }

            if (!DateOnly.TryParse(record.Fecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
            {
                _rechazadosFecha++;
                continue;
            }

            registrosAEnviar.Add(new Surveys
            {
                IdOpinion = record.IdOpinion,
                IdCliente = record.IdCliente,
                IdProducto = record.IdProducto,
                IdClasificacion = idClasificacion,
                IdTipoEncuesta = idTipoEncuesta,
                Fecha = fecha,
                Comentario = record.Comentario.Trim(),
                PuntajeSatisfaccion = record.PuntajeSatisfaccion
            });
        }

        return (registrosAEnviar, registrosCsv.Count);
    }

    protected override async Task<OperationResultLoad> EjecutarCargaBatchAsync(List<Surveys> registros, int idFuenteDatos)
    {
        var resultadoDao = await _daoSurvey.CargarSurveysAsync(registros, idFuenteDatos);

        if (resultadoDao.IsSuccess)
        {
            resultadoDao.Message = $"Duplicado: {_rechazadosDuplicado}, Cliente: {_rechazadosCliente}, " +
                $"Producto: {_rechazadosProducto}, Clasificacion: {_rechazadosClasificacion}, " +
                $"TipoEncuesta: {_rechazadosTipoEncuesta}, Comentario: {_rechazadosComentario}, Fecha: {_rechazadosFecha}";
        }

        return resultadoDao;
    }
}