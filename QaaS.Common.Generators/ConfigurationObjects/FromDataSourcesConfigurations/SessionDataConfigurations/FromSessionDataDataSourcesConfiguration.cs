using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Common.Generators.FromDataSourcesGenerators;

namespace QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations.SessionDataConfigurations;

[Description("Supports generating data from multiple DataSources that contain SessionData files. " +
              "`DataSources`: All Used. Must all be serialized (`byte[]`) `SessionData` items." +
              " `SessionData`: Passed to used DataSources."),
 Display(Name = nameof(FromSessionDataDataSources))]
public record FromSessionDataDataSourcesConfiguration
{
    [Required, Description("name of the session to load the outputs/inputs of as part of the generated data")]
    public string? SessionName { get; set; }
    
    [Required, MinLength(1), Description("All of the communnication data items in the given session data to " +
                           "load as part of the generated data by the order listed here")]
    public CommunicationDataName[]? CommunicationDataList { get; set; }
}