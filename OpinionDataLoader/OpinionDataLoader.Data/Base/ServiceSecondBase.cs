using CsvHelper;
using Microsoft.EntityFrameworkCore;
using OpinionDataLoader.Data.Context;
using OpinionDataLoader.Data.Result;
using System.Globalization;

namespace OpinionDataLoader.Data.Services.Base;

public abstract class ServiceSecondBase<TCsvRecord, TEntity>
{
    protected readonly OpinionesOLTPContext _context;
    protected readonly string _filePath;

    protected ServiceSecondBase(OpinionesOLTPContext context, string filePath)
    {
        _context = context;
        _filePath = filePath;
    }

    protected abstract Task<(List<TEntity> Registros, int TotalFilas)> ExtraerRegistrosValidosAsync(
        int idFuenteDatos, List<TCsvRecord> registrosCsv);

    protected abstract Task<OperationResultLoad> EjecutarCargaBatchAsync(List<TEntity> registros, int idFuenteDatos);

    public async Task<OperationResultLoad> CargarServiceAsync()
    {
        OperationResultLoad result = new();

        try
        {
            if (string.IsNullOrWhiteSpace(_filePath))
            {
                result.IsSuccess = false;
                result.Message = "La ruta del archivo CSV es requerida.";
                return result;
            }

            if (!File.Exists(_filePath))
            {
                result.IsSuccess = false;
                result.Message = $"No se encontro el archivo CSV especificado: {_filePath}";
                return result;
            }

            int idFuenteDatos = await _context.FuenteDatos
                .Where(f => f.TipoFuente == "CSV")
                .Select(f => f.IdFuenteDatos)
                .FirstOrDefaultAsync();

            if (idFuenteDatos == 0)
            {
                result.IsSuccess = false;
                result.Message = "TipoFuente 'CSV' no existe en el catalogo FuenteDatos.";
                return result;
            }

            List<TCsvRecord> registrosCsv = [];

            using (var reader = new StreamReader(_filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                await foreach (var record in csv.GetRecordsAsync<TCsvRecord>())
                {
                    registrosCsv.Add(record);
                }
            }

            var (registros, totalFilas) = await ExtraerRegistrosValidosAsync(idFuenteDatos, registrosCsv);

            int totalRechazadosPreSp = totalFilas - registros.Count;

            var resultadoDao = await EjecutarCargaBatchAsync(registros, idFuenteDatos);

            resultadoDao.TotalLeidos = totalFilas;
            resultadoDao.TotalRechazadosPreSp = totalRechazadosPreSp;
            resultadoDao.TotalEnviados = registros.Count;

            return resultadoDao;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = ex.Message;
            return result;
        }
    }

    protected static bool TryResolveId(string valorCrudo, string prefijo, out int id)
    {
        id = 0;

        if (string.IsNullOrWhiteSpace(valorCrudo))
            return false;

        var texto = valorCrudo.Trim().ToUpperInvariant().Replace(prefijo, string.Empty);

        return int.TryParse(texto, out id);
    }
}