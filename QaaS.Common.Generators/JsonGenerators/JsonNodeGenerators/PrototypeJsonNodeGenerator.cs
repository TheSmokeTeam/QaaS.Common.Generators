using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;

/// <summary>
/// Represents a generator that creates JsonNode objects by cloning a base JsonNode.
/// </summary>
public class PrototypeJsonNodeGenerator: IJsonNodeGenerator
{
    private ILogger Logger { get; set; }
    private JsonNode BaseJsonNode { get; }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="PrototypeJsonNodeGenerator"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="baseJsonNode">The base JsonNode to clone.</param>
    public PrototypeJsonNodeGenerator(ILogger logger, JsonNode baseJsonNode)
    {
        Logger = logger;
        BaseJsonNode = baseJsonNode;
    }
    
    /// <summary>
    /// Generates a new JsonNode object by cloning the base JsonNode.
    /// </summary>
    /// <returns>A new JsonNode object.</returns>
    /// <exception cref="ArgumentException">Thrown when the cloning process results in a null JsonNode.</exception>
    public JsonNode Generate() => BaseJsonNode.DeepClone();
}