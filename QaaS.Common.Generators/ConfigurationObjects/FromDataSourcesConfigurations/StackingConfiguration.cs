using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Common.Generators.FromDataSourcesGenerators;
using QaaS.Framework.Configurations.CustomValidationAttributes;

namespace QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;

[Description("Supports generating data from multiple DataSources by configured items per generator. " +
             "`DataSources`: Used. `SessionData`: Passed to used DataSources."),
 Display(Name = nameof(Stacking))]
public record StackingConfiguration
{
    [Range(0 , int.MaxValue), RequiredIfAny(nameof(LoopFinishedGenerators), true),
     Description("The number of items to generate out of the given data sources (If count is bigger than" +
                 " available number of items an exception will be thrown), " +
                 "if no value is given generates the number of items in the given data sources")]
    public int? Count { get; set; }
    
    [Required, MinLength(1),
    Description("The number of items to generate out of the given data sources in turn")]
    public int[]? ItemsPerGenerator { get; set; }
    
    [DefaultValue(false), Description("When true, generators will restart when there are no more items in them")]
    public bool LoopFinishedGenerators { get; set; } = false;
}