using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace QaaS.Common.Generators.CsvGenerators;

internal interface ICsvGeneratorConfiguration
{
    string Delimiter { get; }
    bool HasHeaderRecord { get; }
    string[]? ColumnNames { get; }
    bool SkipEmptyRows { get; }
    bool TrimWhiteSpace { get; }
}

internal static class CsvRowParser
{
    public static IEnumerable<Dictionary<string, string?>> Parse(
        byte[] csvContent,
        ICsvGeneratorConfiguration configuration,
        string sourceName)
    {
        using var stream = new MemoryStream(csvContent, writable: false);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        foreach (var row in Parse(reader, configuration, sourceName))
        {
            yield return row;
        }
    }

    public static IEnumerable<Dictionary<string, string?>> Parse(
        string csvContent,
        ICsvGeneratorConfiguration configuration,
        string sourceName)
    {
        using var reader = new StringReader(csvContent);
        foreach (var row in Parse(reader, configuration, sourceName))
        {
            yield return row;
        }
    }

    public static string AppendRowIndexToStorageKey(string storageKey, int rowIndex) => $"{storageKey}#{rowIndex}";

    private static IEnumerable<Dictionary<string, string?>> Parse(
        TextReader reader,
        ICsvGeneratorConfiguration configuration,
        string sourceName)
    {
        using var parser = new TextFieldParser(reader)
        {
            TextFieldType = FieldType.Delimited,
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = configuration.TrimWhiteSpace
        };
        parser.SetDelimiters(configuration.Delimiter);

        string[]? headers = null;
        var dataRowIndex = 0;
        while (!parser.EndOfData)
        {
            string[]? fields;
            try
            {
                fields = parser.ReadFields();
            }
            catch (MalformedLineException exception)
            {
                throw new ArgumentException(
                    $"CSV source {sourceName} contains a malformed row near line {parser.ErrorLineNumber}.",
                    exception);
            }

            if (fields is null || ShouldSkipRow(fields, configuration.SkipEmptyRows))
            {
                continue;
            }

            if (headers is null)
            {
                headers = configuration.HasHeaderRecord
                    ? BuildHeadersFromRecord(fields, sourceName)
                    : BuildHeadersForHeaderlessCsv(fields, configuration, sourceName);

                if (configuration.HasHeaderRecord)
                {
                    continue;
                }
            }

            ValidateFieldCount(headers.Length, fields.Length, sourceName, dataRowIndex + 1);
            var row = new Dictionary<string, string?>(headers.Length, StringComparer.Ordinal);
            for (var fieldIndex = 0; fieldIndex < headers.Length; fieldIndex++)
            {
                row.Add(headers[fieldIndex], fields[fieldIndex]);
            }

            dataRowIndex++;
            yield return row;
        }
    }

    private static bool ShouldSkipRow(IEnumerable<string> fields, bool skipEmptyRows)
        => skipEmptyRows && fields.All(string.IsNullOrWhiteSpace);

    private static string[] BuildHeadersFromRecord(string[] headerFields, string sourceName)
        => ValidateHeaders(headerFields, sourceName);

    private static string[] BuildHeadersForHeaderlessCsv(
        string[] firstRowFields,
        ICsvGeneratorConfiguration configuration,
        string sourceName)
    {
        var headers = configuration.ColumnNames?.Length > 0
            ? configuration.ColumnNames.ToArray()
            : Enumerable.Range(1, firstRowFields.Length).Select(index => $"Column{index}").ToArray();

        if (headers.Length != firstRowFields.Length)
        {
            throw new ArgumentException(
                $"CSV source {sourceName} contains {firstRowFields.Length} columns in its first row but " +
                $"{headers.Length} configured column names were supplied.");
        }

        return ValidateHeaders(headers, sourceName);
    }

    private static string[] ValidateHeaders(IEnumerable<string> headers, string sourceName)
    {
        var validatedHeaders = headers.ToArray();
        if (validatedHeaders.Length == 0)
        {
            throw new ArgumentException($"CSV source {sourceName} does not contain any columns.");
        }

        var headerNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var header in validatedHeaders)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                throw new ArgumentException($"CSV source {sourceName} contains an empty column name.");
            }

            if (!headerNames.Add(header))
            {
                throw new ArgumentException(
                    $"CSV source {sourceName} contains duplicate column name `{header}`.");
            }
        }

        return validatedHeaders;
    }

    private static void ValidateFieldCount(int expectedFieldCount, int actualFieldCount, string sourceName, int rowIndex)
    {
        if (expectedFieldCount == actualFieldCount)
        {
            return;
        }

        throw new ArgumentException(
            $"CSV source {sourceName} row {rowIndex} contains {actualFieldCount} columns but expected {expectedFieldCount}.");
    }
}
