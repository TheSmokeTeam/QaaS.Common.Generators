using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

/// <summary>
/// Data Source configuration.
/// </summary>
public record FromDataSource
{
    [Required, Description("The name of the data source to use for the value of the field")]
    public string? Name { get; set; }

    [Description("The policy to use if the data source is out of range")]
    public OutOfRangePolicy OutOfRangePolicy { get; set; } = OutOfRangePolicy.Null;
}