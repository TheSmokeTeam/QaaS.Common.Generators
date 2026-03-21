using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Common.Generators.CsvGenerators;
using QaaS.Common.Generators.FromDataSourcesGenerators;
using QaaS.Framework.Configurations.CustomValidationAttributes;

namespace QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;

[Description("Supports generating structured row objects from multiple DataSources that contain CSV text. " +
             "`DataSources`: Used. Each item must be a UTF-8 encoded byte array or string that contains CSV content. " +
             "`SessionData`: Passed to used DataSources."),
 Display(Name = nameof(FromCsvDataSources))]
public record FromCsvDataSourcesConfiguration : BaseFromDataSourcesConfiguration, ICsvGeneratorConfiguration
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
