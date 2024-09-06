using System.Data;
using Dapper;

namespace InvenSmartApi.Utils;
public class ErrorLogger
{
    private readonly IDbConnection _dbConnection;

    public ErrorLogger(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task LogErrorAsync(string errorMessage, string stackTrace, string className, string methodName)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@ErrorMessage", errorMessage);
        parameters.Add("@StackTrace", stackTrace ?? (object)DBNull.Value);
        parameters.Add("@ClassName", className);
        parameters.Add("@MethodName", methodName);

        await _dbConnection.ExecuteAsync(
            "[dbo].[spInsertErrorLog]",
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}

