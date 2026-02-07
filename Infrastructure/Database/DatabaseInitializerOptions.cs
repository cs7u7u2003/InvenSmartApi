namespace InvenSmartApi.Infrastructure.Database;

public sealed class DatabaseInitializerOptions
{
    public bool Enabled { get; set; } = true;
    public bool CreateDatabaseIfMissing { get; set; } = true;
    public int CommandTimeoutSeconds { get; set; } = 60;
}
