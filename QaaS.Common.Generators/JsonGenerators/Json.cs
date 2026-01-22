using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;

namespace QaaS.Common.Generators.JsonGenerators;

public class Json : BaseJsonGenerator<JsonConfiguration>
{
    /// <inheritdoc />
    protected override IJsonNodeGenerator ConstructJsonNodeGenerator(JsonNode jsonNode) =>
        new PrototypeJsonNodeGenerator(Context.Logger, jsonNode);
}