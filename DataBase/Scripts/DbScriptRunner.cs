using Microsoft.Data.SqlClient;
using System.Text;

namespace InvenSmartApi.Database;

public sealed class DbScriptRunner
{
    private static IEnumerable<string> SplitBatches(string sql)
    {
        var sb = new StringBuilder();
        using var reader = new StringReader(sql);

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                var batch = sb.ToString().Trim();
                sb.Clear();
                if (!string.IsNullOrWhiteSpace(batch))
                    yield return batch;
            }
            else sb.AppendLine(line);
        }

        var last = sb.ToString().Trim();
        if (!string.IsNullOrWhiteSpace(last))
            yield return last;
    }

    public async Task RunFolderAsync(SqlConnection conn, string scriptsFolder, CancellationToken ct)
    {
        if (!Directory.Exists(scriptsFolder)) return;

        var files = Directory.GetFiles(scriptsFolder, "*.sql")
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase);

        foreach (var file in files)
        {
            var sql = await File.ReadAllTextAsync(file, ct);

            using var tx = conn.BeginTransaction();
            try
            {
                foreach (var batch in SplitBatches(sql))
                {
                    using var cmd = conn.CreateCommand();
                    cmd.Transaction = tx;
                    cmd.CommandText = batch;
                    cmd.CommandTimeout = 120;
                    await cmd.ExecuteNonQueryAsync(ct);
                }
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}
