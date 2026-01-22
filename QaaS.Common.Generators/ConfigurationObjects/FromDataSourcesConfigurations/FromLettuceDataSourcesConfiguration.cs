using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using QaaS.Common.Generators.FromDataSourcesGenerators;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;

[Description("Supports generating data from multiple DataSources that contain lettuce files. " +
             "`DataSources`: Used, must all contain Lettuce formatted data deserialized into" +
             $" {nameof(SerializationType.Json)} ({nameof(JsonNode)})." +
             " `SessionData`: Passed to used DataSources."),
 Display(Name = nameof(FromLettuceDataSources))]
public record FromLettuceDataSourcesConfiguration: BaseFromDataSourcesConfiguration;