using Microsoft.Extensions.Configuration;
using OpinionDataLoader.Data.Base;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Interfaces.Daos;
using System.Data;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.DAOs;

public sealed class DaoCliente : OpinionDataConnection, IDaoCliente
{
    public DaoCliente(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<OperationResult> CargarClienteAsync(Cliente cliente)
    {
        int resultado = -1;
        string mensaje = string.Empty;

        var result = await ExecuteSpAsync("dbo.SP_CargarCliente", command =>
        {
            command.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
            command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
            command.Parameters.AddWithValue("@Email", cliente.Email);
            command.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
        },
        command =>
        {
            resultado = command.Parameters["@Resultado"].Value == DBNull.Value ? -1 : (int)command.Parameters["@Resultado"].Value;
            mensaje = command.Parameters["@Mensaje"].Value == DBNull.Value ? string.Empty : (string)command.Parameters["@Mensaje"].Value;
        });

        if (!result.IsSuccess) return result;

        result.Codigo = (CodigoResultado)resultado;
        result.Message = mensaje;
        return result;
    }
}