using OpinionDataLoader.Data.Interfaces.Daos;
using OpinionDataLoader.Data.Interfaces.Services;
using OpinionDataLoader.Data.Models.Csv;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;
using OpinionDataLoader.Data.Services.Base;

namespace OpinionDataLoader.Data.Services;

public sealed class ServiceCategoria : ServicePrimaryBase<ProductoCsv, Categoria>, IServiceCategoria
{
    private readonly IDaoCategoria _daoCategoria;

    public ServiceCategoria(IDaoCategoria daoCategoria, string filePath) : base(filePath)
    {
        _daoCategoria = daoCategoria;
    }

    public Task<OperationResultLoad> CargarCategoriaServiceAsync() => CargarServiceAsync();

    protected override (List<Categoria> Items, int TotalFilas) ExtraerItemsValidos(List<ProductoCsv> registros)
    {
        HashSet<string> categoriasUnicas = [];
        List<Categoria> items = [];

        foreach (var record in registros)
        {
            if (string.IsNullOrWhiteSpace(record.Categoria))
                continue;

            if (categoriasUnicas.Add(record.Categoria.Trim()))
            {
                items.Add(new Categoria { Nombre = record.Categoria.Trim() });
            }
        }

        return (items, registros.Count);
    }

    protected override Task<OperationResult> CargarItemAsync(Categoria item)
        => _daoCategoria.CargarCategoriaAsync(item);
}