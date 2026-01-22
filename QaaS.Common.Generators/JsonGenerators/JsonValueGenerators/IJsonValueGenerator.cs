using System.Text.Json.Nodes;

namespace QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

/// <summary>
/// Defines a contract for generating Json Values.
/// </summary>
public interface IJsonValueGenerator
{
    /// <summary>
    /// Generates a Json Value based on the provided Json schema object.
    /// </summary>
    /// <param name="jsonSchemaObject">The Json schema object to generate the value from.</param>
    /// <param name="jsonSchemaPath">Path of the schema block inside of the Json Schema.</param>
    /// <returns>A Json Value.</returns>
    JsonValue? GenerateValue(JsonObject jsonSchemaObject, string jsonSchemaPath);
}