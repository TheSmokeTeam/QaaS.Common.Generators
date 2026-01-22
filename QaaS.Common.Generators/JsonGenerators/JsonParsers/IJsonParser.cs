using System.Text.Json.Nodes;

namespace QaaS.Common.Generators.JsonGenerators.JsonParsers;

/// <summary>
/// Defines a method for parsing JSON.
/// </summary>
public interface IJsonParser
{
    /// <summary>
    /// Parses a JsonNode into an object of the specified type.
    /// </summary>
    /// <param name="jsonNode">The JsonNode object to parse.</param>
    /// <returns>An object of the specified type.</returns>
    object Parse(JsonNode jsonNode);
}