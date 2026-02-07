using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace InvenSmartApi.Infrastructure.Database;

public sealed class DbInitializerHostedService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbInitializerHostedService> _logger;
    private readonly DatabaseInitializerOptions _options;

    public DbInitializerHostedService(
        IConfiguration configuration,
        ILogger<DbInitializerHostedService> logger,
        IOptions<DatabaseInitializerOptions> options)
    {
        _configuration = configuration;
        _logger = logger;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("DatabaseInitializer disabled.");
            return;
        }

        var rawConnStr = _configuration.GetConnectionString("ConnectionDb");
        if (string.IsNullOrWhiteSpace(rawConnStr))
            throw new InvalidOperationException("Missing ConnectionStrings:ConnectionDb.");

        var csb = new SqlConnectionStringBuilder(rawConnStr);

        if (string.IsNullOrWhiteSpace(csb.InitialCatalog))
            throw new InvalidOperationException("ConnectionDb must include Database/Initial Catalog.");

        var dbName = csb.InitialCatalog;

        _logger.LogInformation("DB init started. Target DB: {DbName}", dbName);

        // 1) Ensure server connectivity (try open)
        await EnsureServerConnectivityAsync(rawConnStr, cancellationToken);

        // 2) Create database if missing (connect to master)
        if (_options.CreateDatabaseIfMissing)
        {
            await EnsureDatabaseExistsAsync(csb, dbName, cancellationToken);
        }

        // 3) Ensure required objects in target database
        await EnsureRequiredObjectsAsync(rawConnStr, cancellationToken);

        _logger.LogInformation("DB init finished OK.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task EnsureServerConnectivityAsync(string connStr, CancellationToken ct)
    {
        try
        {
            using var conn = new SqlConnection(connStr);
            conn.ConnectionString = connStr; // explicit
            await conn.OpenAsync(ct);
            await conn.CloseAsync();
            _logger.LogInformation("DB connectivity OK.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DB connectivity FAILED. Check ConnectionDb.");
            throw; // fail fast: app should not run without DB
        }
    }

    private async Task EnsureDatabaseExistsAsync(SqlConnectionStringBuilder csb, string dbName, CancellationToken ct)
    {
        var masterCsb = new SqlConnectionStringBuilder(csb.ConnectionString)
        {
            InitialCatalog = "master"
        };

        var sql = $@"
IF DB_ID(N'{EscapeSqlLiteral(dbName)}') IS NULL
BEGIN
    CREATE DATABASE [{dbName}];
END
";

        using var conn = new SqlConnection(masterCsb.ConnectionString);
        await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandTimeout = _options.CommandTimeoutSeconds;
        cmd.CommandText = sql;

        _logger.LogInformation("Ensuring database exists...");
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private async Task EnsureRequiredObjectsAsync(string connStr, CancellationToken ct)
    {
        var scripts = DatabaseScripts.GetAll();

        using var conn = new SqlConnection(connStr);
        await conn.OpenAsync(ct);

        foreach (var script in scripts)
        {
            _logger.LogInformation("Applying script: {Name}", script.Name);

            foreach (var batch in SqlBatchSplitter.Split(script.Sql))
            {
                if (string.IsNullOrWhiteSpace(batch)) continue;

                using var cmd = conn.CreateCommand();
                cmd.CommandTimeout = _options.CommandTimeoutSeconds;
                cmd.CommandText = batch;

                await cmd.ExecuteNonQueryAsync(ct);
            }
        }
    }

    private static string EscapeSqlLiteral(string value) => value.Replace("'", "''");
}
