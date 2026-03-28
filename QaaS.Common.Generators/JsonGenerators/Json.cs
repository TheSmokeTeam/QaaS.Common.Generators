using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;

namespace QaaS.Common.Generators.JsonGenerators;

/// <summary>
/// Generates JSON data from a configured prototype document.
/// </summary>
/// <qaas-docs group="Structured payloads" subgroup="Prototype JSON" />
public class Json : BaseJsonGenerator<JsonConfiguration>
{
    /// <inheritdoc />
    protected override IJsonNodeGenerator ConstructJsonNodeGenerator(JsonNode jsonNode) =>
        new PrototypeJsonNodeGenerator(Context.Logger, jsonNode);
}
