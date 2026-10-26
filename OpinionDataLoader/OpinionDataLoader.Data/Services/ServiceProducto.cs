using OpinionDataLoader.Data.Interfaces.Daos;
using OpinionDataLoader.Data.Interfaces.Services;
using OpinionDataLoader.Data.Models.Csv;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;
using OpinionDataLoader.Data.Services.Base;

namespace OpinionDataLoader.Data.Services;

public sealed class ServiceProducto : ServicePrimaryBase<ProductoCsv, (Producto Producto, string Categoria)>, IServiceProducto
{
    private readonly IDaoProducto _daoProducto;

    public ServiceProducto(IDaoProducto daoProducto, string filePath) : base(filePath)
    {
        _daoProducto = daoProducto;
    }

    public Task<OperationResultLoad> CargarProductoServiceAsync() => CargarServiceAsync();

    protected override (List<(Producto Producto, string Categoria)> Items, int TotalFilas) ExtraerItemsValidos(List<ProductoCsv> registros)
    {
        HashSet<int> productosUnicos = [];
        List<(Producto Producto, string Categoria)> items = [];

        foreach (var record in registros)
        {
            if (string.IsNullOrWhiteSpace(record.Nombre) || string.IsNullOrWhiteSpace(record.Categoria))
                continue;

            if (productosUnicos.Add(record.IdProducto))
            {
                var producto = new Producto
                {
                    IdProducto = record.IdProducto,
                    Nombre = record.Nombre.Trim()
                };

                items.Add((producto, record.Categoria.Trim()));
            }
        }

        return (items, registros.Count);
    }

    protected override Task<OperationResult> CargarItemAsync((Producto Producto, string Categoria) item)
        => _daoProducto.CargarProductoAsync(item.Producto, item.Categoria);
}   