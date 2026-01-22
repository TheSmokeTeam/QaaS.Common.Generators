using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Common.Generators.FromDataSourcesGenerators;

namespace QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;

[Description("Supports generating data from multiple DataSources that contain data. " +
             "`DataSources`: Used. `SessionData`: Passed to used DataSources."),
 Display(Name = nameof(FromDataSources))]
public record FromDataSourceBasedConfiguration: BaseFromDataSourcesConfiguration;