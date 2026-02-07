using System.Text.RegularExpressions;

namespace InvenSmartApi.Infrastructure.Database;

public static class SqlBatchSplitter
{
    // Splits batches by lines containing only "GO"
    private static readonly Regex GoRegex = new(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

    public static IEnumerable<string> Split(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            yield break;

        var parts = GoRegex.Split(sql);

        foreach (var p in parts)
            yield return p.Trim();
    }
}
