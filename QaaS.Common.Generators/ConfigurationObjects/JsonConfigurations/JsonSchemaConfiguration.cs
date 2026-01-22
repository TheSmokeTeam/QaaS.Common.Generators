using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using QaaS.Common.Generators.JsonGenerators;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

[Description("Supports generating data from Json (Schema) data sources. `DataSources`: " +
             "Used, should only be a single one and must  contain data deserialized into" +
             $" {nameof(SerializationType.Json)} ({nameof(JsonNode)})." +
             " `SessionData`: Passed to used DataSources."),
 Display(Name = nameof(JsonSchemaDraft4))]
public record JsonSchemaConfiguration : JsonConfiguration
{
    [Description("The generation seed, used for random Json Schema value generation")]
    public int? Seed { get; set; } = null;
}