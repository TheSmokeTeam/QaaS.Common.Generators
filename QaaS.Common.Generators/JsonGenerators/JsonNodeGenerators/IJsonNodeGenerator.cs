using System.Text.Json.Nodes;

namespace QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;

/// <summary>
/// Represents a generator that creates JsonNode objects.
/// </summary>
public interface IJsonNodeGenerator
{
    /// <summary>
    /// Generates a new JsonNode object.
    /// </summary>
    /// <returns>A new JsonNode object.</returns>
    JsonNode Generate();
}
