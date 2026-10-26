using Microsoft.Extensions.Configuration;
using OpinionDataLoader.Data.Base;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Interfaces.Daos;
using System.Data;
using OpinionDataLoader.Data.Result;

namespace OpinionDataLoader.Data.DAOs;

public sealed class DaoRedSocial : OpinionDataConnection, IDaoRedSocial
{
    public DaoRedSocial(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<OperationResult> CargarRedSocialAsync(RedSocial redSocial)
    {
        int idRedSocial = 0;
        int resultado = -1;
        string mensaje = string.Empty;

        var result = await ExecuteSpAsync("dbo.SP_CargarRedSocial", command =>
        {
            command.Parameters.AddWithValue("@Nombre", redSocial.Nombre);
            command.Parameters.Add("@IdRedSocial", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
            command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
        },
        command =>
        {
            idRedSocial = command.Parameters["@IdRedSocial"].Value == DBNull.Value ? 0 : (int)command.Parameters["@IdRedSocial"].Value;
            resultado = command.Parameters["@Resultado"].Value == DBNull.Value ? -1 : (int)command.Parameters["@Resultado"].Value;
            mensaje = command.Parameters["@Mensaje"].Value == DBNull.Value ? string.Empty : (string)command.Parameters["@Mensaje"].Value;
        });

        if (!result.IsSuccess) return result;

        redSocial.IdRedSocial = idRedSocial;
        result.Codigo = (CodigoResultado)resultado;
        result.Message = mensaje;
        return result;
    }
}