using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvenSmartApi.Utils
{
    public class ErrorLogger
    {
        private readonly string _connectionString;

        public ErrorLogger(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionDb")
                ?? throw new InvalidOperationException("Connection string 'ConnectionDb' not found.");
        }

        public async Task LogErrorAsync(string errorMessage, string? stackTrace, string className, string methodName)
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@ErrorMessage", errorMessage);
                parameters.Add("@StackTrace", (object?)stackTrace ?? DBNull.Value);
                parameters.Add("@ClassName", className);
                parameters.Add("@MethodName", methodName);

                await conn.ExecuteAsync(
                    "[dbo].[spInsertErrorLog]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                // Nunca rompas el flujo de la app por fallar al loguear.
                // Aquí podrías hacer Console.Error.WriteLine(...) si lo deseas.
            }
        }
    }
}
