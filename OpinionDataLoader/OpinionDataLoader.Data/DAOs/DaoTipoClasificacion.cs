using Microsoft.Extensions.Configuration;
using OpinionDataLoader.Data.Base;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Interfaces.Daos;
using System.Data;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.DAOs;

public sealed class DaoTipoClasificacion : OpinionDataConnection, IDaoTipoClasificacion
{
    public DaoTipoClasificacion(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<OperationResult> CargarTipoClasificacionAsync(TipoClasificacion tipoClasificacion)
    {
        int idClasificacion = 0;
        int resultado = -1;
        string mensaje = string.Empty;

        var result = await ExecuteSpAsync("dbo.SP_CargarTipoClasificacion", command =>
        {
            command.Parameters.AddWithValue("@Descripcion", tipoClasificacion.Descripcion);
            command.Parameters.Add("@IdClasificacion", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
        },
        command =>
        {
            idClasificacion = command.Parameters["@IdClasificacion"].Value == DBNull.Value ? 0 : (int)command.Parameters["@IdClasificacion"].Value;
            resultado = command.Parameters["@Resultado"].Value == DBNull.Value ? -1 : (int)command.Parameters["@Resultado"].Value;
            mensaje = command.Parameters["@Mensaje"].Value == DBNull.Value ? string.Empty : (string)command.Parameters["@Mensaje"].Value;
        });

        if (!result.IsSuccess) return result;

        tipoClasificacion.IdClasificacion = idClasificacion;
        result.Codigo = (CodigoResultado)resultado;
        result.Message = mensaje;
        return result;
    }
}