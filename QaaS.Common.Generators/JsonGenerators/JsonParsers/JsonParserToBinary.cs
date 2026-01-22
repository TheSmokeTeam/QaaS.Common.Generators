using System.Text.Json;
using System.Text.Json.Nodes;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.JsonGenerators.JsonParsers;

/// <inheritdoc />
public class JsonParserToBinary : IJsonParser
{
    /// <summary>
    /// The type of the object to be parsed.
    /// </summary>
    private readonly Type _typeObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParserToBinary"/> class.
    /// </summary>
    /// <param name="specificTypeConfig">The parsed-to object type configuration</param>
    /// <exception cref="ArgumentException">Thrown when the type doesn't exist in the assembly.</exception>
    public JsonParserToBinary(SpecificTypeConfig specificTypeConfig)
    {
        _typeObject = specificTypeConfig.GetConfiguredType();
    }
    
    /// <inheritdoc />
    public object Parse(JsonNode jsonNode)
    {
        return jsonNode.Deserialize(_typeObject)!;
    }
}