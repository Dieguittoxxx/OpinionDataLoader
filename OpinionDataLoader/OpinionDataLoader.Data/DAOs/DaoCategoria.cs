using Microsoft.Extensions.Configuration;
using OpinionDataLoader.Data.Base;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Interfaces.Daos;
using System.Data;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.DAOs;

public sealed class DaoCategoria : OpinionDataConnection, IDaoCategoria
{
    public DaoCategoria(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<OperationResult> CargarCategoriaAsync(Categoria categoria)
    {
        int idCategoria = 0;
        int resultado = -1;
        string mensaje = string.Empty;

        var result = await ExecuteSpAsync("dbo.SP_CargarCategoria", command =>
        {
            command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            command.Parameters.Add("@IdCategoria", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
        },
        command =>
        {
            idCategoria = command.Parameters["@IdCategoria"].Value == DBNull.Value ? 0 : (int)command.Parameters["@IdCategoria"].Value;
            resultado = command.Parameters["@Resultado"].Value == DBNull.Value ? -1 : (int)command.Parameters["@Resultado"].Value;
            mensaje = command.Parameters["@Mensaje"].Value == DBNull.Value ? string.Empty : (string)command.Parameters["@Mensaje"].Value;
        });

        if (!result.IsSuccess) return result;

        categoria.IdCategoria = idCategoria;
        result.Codigo = (CodigoResultado)resultado;
        result.Message = mensaje;
        return result;
    }
}