using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OpinionDataLoader.Data.Result;
using System.Data;
#nullable disable

namespace OpinionDataLoader.Data.Base;

public abstract class OpinionDataConnection
{
    private readonly string _connectionString;
    public OpinionDataConnection(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    protected async Task<OperationResult> ExecuteSpAsync(string spName, Action<SqlCommand> parameters, Action<SqlCommand>? readOutputs = null)
    {
        OperationResult result = new();
        try
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;

            parameters(command);

            await command.ExecuteNonQueryAsync();

            readOutputs?.Invoke(command);

            result.IsSuccess = true;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = ex.Message;
        }
        return result;
    }

    protected async Task<OperationResult<List<T>>> ExecuteReaderAsync<T>(string spName, Action<SqlCommand> parameters, Func<SqlDataReader, T> map)
    {
        OperationResult<List<T>> result = new();
        try
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;

            parameters(command);

            var data = new List<T>();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                data.Add(map(reader));
            }

            result.Data = data;
            result.IsSuccess = true;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = ex.Message;
        }
        return result;
    }

    protected async Task<OperationResult<T>> ExecuteScalarAsync<T>(string spName, Action<SqlCommand> parameters)
    {
        OperationResult<T> result = new();
        try
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;

            parameters(command);

            var value = await command.ExecuteScalarAsync();

            result.Data = value == DBNull.Value || value is null
                ? default
                : (T)Convert.ChangeType(value, typeof(T));

            result.IsSuccess = true;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Message = ex.Message;
        }
        return result;
    }

}
