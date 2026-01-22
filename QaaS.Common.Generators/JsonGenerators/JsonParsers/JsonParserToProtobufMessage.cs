using System.Text.Json.Nodes;
using Newtonsoft.Json;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.JsonGenerators.JsonParsers;

/// <inheritdoc />
public class JsonParserToProtobufMessage : IJsonParser
{
    /// <summary>
    /// The type of the object to be parsed.
    /// </summary>
    private readonly Type _typeObject;
    
    /// <summary>
    /// The settings for the json serializer.
    /// Used for handling null proto serialization.
    /// </summary>
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParserToProtobufMessage"/> class.
    /// </summary>
    /// <param name="specificTypeConfig">The parsed-to object type configuration</param>
    /// <exception cref="ArgumentException">Thrown when the type doesn't exist in the assembly.</exception>
    public JsonParserToProtobufMessage(SpecificTypeConfig specificTypeConfig)
    {
        _typeObject = specificTypeConfig.GetConfiguredType();
    }
    
    /// <inheritdoc />
    public object Parse(JsonNode jsonNode)
    {
        /*
         * GOD I'M SORRY FOR ANYONE WHO IS READING THIS.
         * One day PLEASE fix this and remove Newtonsoft.Json implementation.
         * REDA and me (REDA) couldn't do it, maybe you will be able to.
         */
        return JsonConvert.DeserializeObject(jsonNode.ToJsonString(), _typeObject, _jsonSerializerSettings) 
               ?? throw new InvalidOperationException($"Couldn't parse json generation to type {_typeObject}" +
                                                      $"with JsonConvert.DeserializeObject (Newtonsoft.Json)");
    }
}