using System.Text.Json.Nodes;

namespace QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

/// <inheritdoc />
public class NullJsonValueGenerator : IJsonValueGenerator
{
    /// <inheritdoc />
    public JsonValue? GenerateValue(JsonObject jsonSchemaObject, string jsonSchemaPath) => null;
}