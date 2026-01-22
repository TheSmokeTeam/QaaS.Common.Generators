using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;
using QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

namespace QaaS.Common.Generators.JsonGenerators;

public class JsonSchemaDraft4 : BaseJsonGenerator<JsonSchemaConfiguration>
{
    /// <inheritdoc />
    protected override IJsonNodeGenerator ConstructJsonNodeGenerator(JsonNode jsonNode) =>
        new SchemaDraft4JsonNodeGenerator(Context.Logger, JsonValueGeneratorFactory.GetInstance(), jsonNode, Configuration.Seed);
}