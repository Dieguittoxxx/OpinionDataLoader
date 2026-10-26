using CsvHelper;
using OpinionDataLoader.Data.Result;
using System.Globalization;

namespace OpinionDataLoader.Data.Services.Base;

public abstract class ServicePrimaryBase<TCsvRecord, TItem>
{
    protected readonly string _filePath;

    protected ServicePrimaryBase(string filePath)
    {
        _filePath = filePath;
    }

    protected abstract (List<TItem> Items, int TotalFilas) ExtraerItemsValidos(List<TCsvRecord> registros);

    protected abstract Task<OperationResult> CargarItemAsync(TItem item);

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

            List<TCsvRecord> registrosCsv = [];

            using (var reader = new StreamReader(_filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                await foreach (var record in csv.GetRecordsAsync<TCsvRecord>())
                {
                    registrosCsv.Add(record);
                }
            }

            var (items, totalFilas) = ExtraerItemsValidos(registrosCsv);

            int totalRechazadosPreSp = totalFilas - items.Count;
            int totalInsertados = 0;
            int totalYaExistian = 0;
            int totalRechazados = 0;

            foreach (var item in items)
            {
                var resultadoDao = await CargarItemAsync(item);

                switch (resultadoDao.Codigo)
                {
                    case CodigoResultado.Insertado:
                        totalInsertados++;
                        break;
                    case CodigoResultado.YaExistia:
                        totalYaExistian++;
                        break;
                    default:
                        totalRechazados++;
                        break;
                }
            }

            result.IsSuccess = true;
            result.TotalLeidos = totalFilas;
            result.TotalRechazadosPreSp = totalRechazadosPreSp;
            result.TotalEnviados = items.Count;
            result.TotalInsertados = totalInsertados;
            result.TotalYaExistian = totalYaExistian;
            result.TotalRechazados = totalRechazados;

            return result;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = ex.Message;
            return result;
        }
    }
}