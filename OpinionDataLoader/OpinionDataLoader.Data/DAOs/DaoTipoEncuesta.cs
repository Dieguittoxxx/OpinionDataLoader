using Microsoft.Extensions.Configuration;
using OpinionDataLoader.Data.Base;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Interfaces.Daos;
using System.Data;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.DAOs;

public sealed class DaoTipoEncuesta : OpinionDataConnection, IDaoTipoEncuesta
{
    public DaoTipoEncuesta(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<OperationResult> CargarTipoEncuestaAsync(TipoEncuesta tipoEncuesta)
    {
        int idTipoEncuesta = 0;
        int resultado = -1;
        string mensaje = string.Empty;

        var result = await ExecuteSpAsync("dbo.SP_CargarTipoEncuesta", command =>
        {
            command.Parameters.AddWithValue("@Nombre", tipoEncuesta.Nombre);
            command.Parameters.Add("@IdTipoEncuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
        },
        command =>
        {
            idTipoEncuesta = command.Parameters["@IdTipoEncuesta"].Value == DBNull.Value ? 0 : (int)command.Parameters["@IdTipoEncuesta"].Value;
            resultado = command.Parameters["@Resultado"].Value == DBNull.Value ? -1 : (int)command.Parameters["@Resultado"].Value;
            mensaje = command.Parameters["@Mensaje"].Value == DBNull.Value ? string.Empty : (string)command.Parameters["@Mensaje"].Value;
        });

        if (!result.IsSuccess) return result;

        tipoEncuesta.IdTipoEncuesta = idTipoEncuesta;
        result.Codigo = (CodigoResultado)resultado;
        result.Message = mensaje;
        return result;
    }
}