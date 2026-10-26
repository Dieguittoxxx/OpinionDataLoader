using Microsoft.Extensions.Configuration;
using OpinionDataLoader.Data.Base;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Interfaces.Daos;
using System.Data;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.DAOs;

public sealed class DaoProducto : OpinionDataConnection, IDaoProducto
{
    public DaoProducto(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<OperationResult> CargarProductoAsync(Producto producto, string categoria)
    {
        int resultado = -1;
        string mensaje = string.Empty;

        var result = await ExecuteSpAsync("dbo.SP_CargarProducto", command =>
        {
            command.Parameters.AddWithValue("@IdProducto", producto.IdProducto);
            command.Parameters.AddWithValue("@Nombre", producto.Nombre);
            command.Parameters.AddWithValue("@Categoria", categoria);
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