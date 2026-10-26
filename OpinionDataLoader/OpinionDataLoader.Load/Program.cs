using Microsoft.Extensions.DependencyInjection;
using OpinionDataLoader.Data.Interfaces.Services;
using OpinionDataLoader.Data.Result;
using OpinionDataLoader.Load.Hosting;

namespace OpinionDataLoader.Load;

internal class Program
{
    static async Task Main(string[] args)
    {
        var host = AppHost.CreateHost(args);
        using var scope = host.Services.CreateScope();
        var provider = scope.ServiceProvider;

        var resultados = await provider.GetRequiredService<IServiceLoad>().EjecutarCargaAsync();

        foreach (var (entidad, resultado) in resultados)
        {
            MostrarResultado(entidad, resultado);
        }
    }

    static void MostrarResultado(string nombre, OperationResultLoad resultado)
    {
        Console.WriteLine($"--- {nombre} ---");
        Console.WriteLine($"Exito: {resultado.IsSuccess}");

        if (!string.IsNullOrWhiteSpace(resultado.Message))
            Console.WriteLine($"Mensaje: {resultado.Message}");

        Console.WriteLine($"Leidos: {resultado.TotalLeidos}");
        Console.WriteLine($"RechazadosPreSp: {resultado.TotalRechazadosPreSp}");
        Console.WriteLine($"Enviados: {resultado.TotalEnviados}");
        Console.WriteLine($"Insertados: {resultado.TotalInsertados}");
        Console.WriteLine($"YaExistian: {resultado.TotalYaExistian}");
        Console.WriteLine($"Rechazados: {resultado.TotalRechazados}");
        Console.WriteLine();
    }
}