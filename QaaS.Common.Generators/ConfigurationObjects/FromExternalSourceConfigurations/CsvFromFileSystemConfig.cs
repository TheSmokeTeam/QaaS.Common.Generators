using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Common.Generators.CsvGenerators;
using QaaS.Common.Generators.FromExternalSourceGenerators;
using QaaS.Framework.Configurations.CustomValidationAttributes;

namespace QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;

[Description("Retrieves CSV rows from files in a configured path in the file system. " +
             "`DataSources`: Not used. `SessionData`: Not used."),
 Display(Name = nameof(CsvFromFileSystem))]
public record CsvFromFileSystemConfig : FromFileSystemConfig, ICsvGeneratorConfiguration
{
    [Required, MinLength(1),
     Description("The delimiter used between columns in the CSV content."),
     DefaultValue(",")]
    public string Delimiter { get; set; } = ",";

    [Description("True when the first non-empty row contains the column names."),
     DefaultValue(true)]
    public bool HasHeaderRecord { get; set; } = true;

    [UniqueItemsInEnumerable,
     Description("Column names to use when `HasHeaderRecord` is false. If omitted, names are auto-generated as `Column1`, `Column2`, and so on.")]
    public string[]? ColumnNames { get; set; }

    [Description("True to skip rows whose fields are all empty or whitespace."),
     DefaultValue(true)]
    public bool SkipEmptyRows { get; set; } = true;

    [Description("True to trim surrounding whitespace from parsed fields."),
     DefaultValue(false)]
    public bool TrimWhiteSpace { get; set; }
}
