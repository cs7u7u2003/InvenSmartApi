using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;

namespace InvenSmartApi.Database;

public sealed class DbInitializerHostedService : IHostedService
{
    private readonly IConfiguration _config;
    private readonly ILogger<DbInitializerHostedService> _logger;
    private readonly DbScriptRunner _runner;

    public DbInitializerHostedService(
        IConfiguration config,
        ILogger<DbInitializerHostedService> logger,
        DbScriptRunner runner)
    {
        _config = config;
        _logger = logger;
        _runner = runner;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        var cs = _config.GetConnectionString("ConnectionDb")
                 ?? throw new InvalidOperationException("ConnectionStrings:ConnectionDb no existe.");

        var builder = new SqlConnectionStringBuilder(cs);
        var dbName = builder.InitialCatalog;

        if (string.IsNullOrWhiteSpace(dbName))
            throw new InvalidOperationException("ConnectionDb debe incluir Database=InvenSmartDB.");

        // 1) Verificar/crear DB
        await EnsureDatabaseExistsAsync(builder, dbName, ct);

        // 2) Ejecutar scripts dentro de la DB
        builder.InitialCatalog = dbName;

        await using var conn = new SqlConnection(builder.ConnectionString);
        await conn.OpenAsync(ct);

        var scriptsFolder = Path.Combine(AppContext.BaseDirectory, "Database", "Scripts");
        _logger.LogInformation("DB init: ejecutando scripts en {Folder}", scriptsFolder);

        await _runner.RunFolderAsync(conn, scriptsFolder, ct);

        _logger.LogInformation("DB init: OK");
    }

    private async Task EnsureDatabaseExistsAsync(SqlConnectionStringBuilder builder, string dbName, CancellationToken ct)
    {
        var originalDb = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        await using var masterConn = new SqlConnection(builder.ConnectionString);
        await masterConn.OpenAsync(ct);

        using var cmd = masterConn.CreateCommand();
        cmd.CommandText = "SELECT 1 FROM sys.databases WHERE name = @db";
        cmd.Parameters.AddWithValue("@db", dbName);

        var exists = await cmd.ExecuteScalarAsync(ct);

        if (exists is null)
        {
            _logger.LogWarning("DB init: {Db} no existe. Intentando crearla...", dbName);

            try
            {
                using var create = masterConn.CreateCommand();
                create.CommandText = $"CREATE DATABASE [{dbName}]";
                await create.ExecuteNonQueryAsync(ct);
                _logger.LogInformation("DB init: {Db} creada.", dbName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DB init: no se pudo crear {Db}. Ejecuta el script ADMIN una vez.", dbName);
                throw;
            }
        }

        builder.InitialCatalog = originalDb;
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
