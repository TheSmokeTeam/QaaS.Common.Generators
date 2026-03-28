using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;
using QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

namespace QaaS.Common.Generators.JsonGenerators;

/// <summary>
/// Generates JSON data that conforms to a configured JSON Schema Draft 4 document.
/// </summary>
/// <qaas-docs group="Structured payloads" subgroup="Schema-based JSON" />
public class JsonSchemaDraft4 : BaseJsonGenerator<JsonSchemaConfiguration>
{
    /// <inheritdoc />
    protected override IJsonNodeGenerator ConstructJsonNodeGenerator(JsonNode jsonNode) =>
        new SchemaDraft4JsonNodeGenerator(Context.Logger, JsonValueGeneratorFactory.GetInstance(), jsonNode, Configuration.Seed);
}
