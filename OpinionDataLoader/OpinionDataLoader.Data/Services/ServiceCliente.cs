using OpinionDataLoader.Data.Interfaces.Daos;
using OpinionDataLoader.Data.Interfaces.Services;
using OpinionDataLoader.Data.Models.Csv;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Result;
using OpinionDataLoader.Data.Services.Base;

namespace OpinionDataLoader.Data.Services;

public sealed class ServiceCliente : ServicePrimaryBase<ClienteCsv, Cliente>, IServiceCliente
{
    private readonly IDaoCliente _daoCliente;

    public ServiceCliente(IDaoCliente daoCliente, string filePath) : base(filePath)
    {
        _daoCliente = daoCliente;
    }

    public Task<OperationResultLoad> CargarClienteServiceAsync() => CargarServiceAsync();

    protected override (List<Cliente> Items, int TotalFilas) ExtraerItemsValidos(List<ClienteCsv> registros)
    {
        HashSet<int> clientesUnicos = [];
        List<Cliente> items = [];

        foreach (var record in registros)
        {
            if (string.IsNullOrWhiteSpace(record.Nombre) || string.IsNullOrWhiteSpace(record.Email))
                continue;

            if (clientesUnicos.Add(record.IdCliente))
            {
                items.Add(new Cliente
                {
                    IdCliente = record.IdCliente,
                    Nombre = record.Nombre.Trim(),
                    Email = record.Email.Trim()
                });
            }
        }

        return (items, registros.Count);
    }

    protected override Task<OperationResult> CargarItemAsync(Cliente item)
        => _daoCliente.CargarClienteAsync(item);
}