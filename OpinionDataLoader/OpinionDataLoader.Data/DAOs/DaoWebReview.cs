using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OpinionDataLoader.Data.Base;
using OpinionDataLoader.Data.Result;
using OpinionDataLoader.Data.Models.dbo;
using OpinionDataLoader.Data.Interfaces.Daos;
using System.Data;

namespace OpinionDataLoader.Data.DAOs;

public sealed class DaoWebReview : OpinionDataConnection, IDaoWebReview
{
    public DaoWebReview(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<OperationResultLoad> CargarWebReviewsAsync(List<WebReviews> registros, int idFuenteDatos)
    {
        int totalYaExistian = 0;
        int totalInsertados = 0;
        int totalRechazados = 0;
        int resultado = -1;
        string mensaje = string.Empty;

        var result = await ExecuteSpAsync("dbo.SP_CargarWebReviews",
            command =>
            {
                command.Parameters.Add(new SqlParameter("@Registros", SqlDbType.Structured)
                {
                    TypeName = "dbo.TVP_WebReviews",
                    Value = ConstruirTabla(registros)
                });

                command.Parameters.AddWithValue("@IdFuenteDatos", idFuenteDatos);
                command.Parameters.Add("@TotalInsertados", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@TotalRechazados", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@TotalYaExistian", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@Mensaje", SqlDbType.NVarChar, -1).Direction = ParameterDirection.Output;
            },
            command =>
            {
                totalYaExistian = command.Parameters["@TotalYaExistian"].Value == DBNull.Value
                    ? 0 : (int)command.Parameters["@TotalYaExistian"].Value;

                totalInsertados = command.Parameters["@TotalInsertados"].Value == DBNull.Value
                    ? 0 : (int)command.Parameters["@TotalInsertados"].Value;

                totalRechazados = command.Parameters["@TotalRechazados"].Value == DBNull.Value
                    ? 0 : (int)command.Parameters["@TotalRechazados"].Value;

                resultado = command.Parameters["@Resultado"].Value == DBNull.Value
                    ? -1 : (int)command.Parameters["@Resultado"].Value;

                mensaje = command.Parameters["@Mensaje"].Value == DBNull.Value
                    ? string.Empty : (string)command.Parameters["@Mensaje"].Value;
            });

        if (!result.IsSuccess)
        {
            return new OperationResultLoad
            {
                IsSuccess = false,
                Message = result.Message,
                Codigo = CodigoResultado.Rechazado
            };
        }

        return new OperationResultLoad
        {
            IsSuccess = true,
            Codigo = (CodigoResultado)resultado,
            Message = mensaje,
            TotalInsertados = totalInsertados,
            TotalRechazados = totalRechazados,
            TotalYaExistian = totalYaExistian
        };
    }

    private static DataTable ConstruirTabla(List<WebReviews> registros)
    {
        var tabla = new DataTable();
        tabla.Columns.Add("IdOriginal", typeof(string));
        tabla.Columns.Add("IdCliente", typeof(int));
        tabla.Columns.Add("IdProducto", typeof(int));
        tabla.Columns.Add("Fecha", typeof(DateTime));
        tabla.Columns.Add("Comentario", typeof(string));
        tabla.Columns.Add("Rating", typeof(decimal));

        foreach (var r in registros)
        {
            tabla.Rows.Add(r.IdOriginal, r.IdCliente, r.IdProducto, r.Fecha.ToDateTime(TimeOnly.MinValue), r.Comentario, r.Rating);
        }

        return tabla;
    }
}