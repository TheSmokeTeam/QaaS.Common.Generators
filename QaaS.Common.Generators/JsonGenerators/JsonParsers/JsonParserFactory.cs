using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.JsonGenerators.JsonParsers;

/// <summary>
/// This class is responsible for creating and managing instances of <see cref="IJsonParser"/>.
/// </summary>  
public class JsonParserFactory
{
    /// <summary>
    /// Singleton instance of <see cref="JsonParserFactory"/> class.
    /// </summary>
    private static JsonParserFactory? _instance;
    
    /// <summary>
    /// A dictionary that stores instances of <see cref="IJsonParser"/>.
    /// </summary>
    private readonly IDictionary<(JsonParserType, string), IJsonParser> _parsers;
    
    /// <summary>
    /// Private constructor to prevent direct instantiation.
    /// </summary>
    private JsonParserFactory()
    {
        _parsers = new Dictionary<(JsonParserType, string), IJsonParser>();
    }

    /// <summary>
    /// Returns the singleton instance of the <see cref="JsonParserFactory"/> class.
    /// If the instance does not exist, it creates a new one.
    /// </summary>
    /// <returns>The singleton instance of the <see cref="JsonParserFactory"/> class.</returns>
    public static JsonParserFactory GetInstance() => _instance ??= new JsonParserFactory();
    
    /// <summary>
    /// Retrieves an instance of <see cref="IJsonParser"/> based on the provided <see cref="JsonParserType"/>.
    /// </summary>
    /// <param name="type">The type of <see cref="IJsonParser"/> to retrieve</param>
    /// <param name="specificTypeConfig">The parsed-to object type configuration</param>
    /// <returns>An instance of <see cref="IJsonParser"/></returns>
    public IJsonParser GetJsonParser(JsonParserType type, SpecificTypeConfig? specificTypeConfig)
    {
        var typeFullName = specificTypeConfig != null ? specificTypeConfig.TypeFullName! : "null";
        var jsonParserToObjectTypeTuple = (type, typeFullName);
        if (!_parsers.ContainsKey(jsonParserToObjectTypeTuple)) 
            _parsers.Add(jsonParserToObjectTypeTuple, BuildJsonParser(type, specificTypeConfig));
        return _parsers[jsonParserToObjectTypeTuple];
    }

    /// <summary>
    /// Builds an instance of <see cref="IJsonParser"/> based on the provided <see cref="JsonParserType"/>.
    /// </summary>
    /// <param name="type">The type of <see cref="IJsonParser"/> to build</param>
    /// <param name="specificTypeConfig">The parsed-to object type configuration</param>
    /// <returns>An instance of <see cref="IJsonParser"/></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided <see cref="JsonParserType"/> is not supported</exception>
    private IJsonParser BuildJsonParser(JsonParserType type, SpecificTypeConfig? specificTypeConfig)
    {
        return type switch
        {
            JsonParserType.Binary => new JsonParserToBinary(specificTypeConfig!),
            JsonParserType.ProtobufMessage => new JsonParserToProtobufMessage(specificTypeConfig!),
            JsonParserType.Xml => new JsonParserToXml(),
            _ => throw new ArgumentException("Json Parser type not supported", nameof(type))
        };
    }
}