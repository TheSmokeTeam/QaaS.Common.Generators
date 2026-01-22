using QaaS.Common.Generators.JsonGenerators.JsonExtensions;

namespace QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

/// <summary>
/// This class is a factory for generating JsonValueGenerator objects.
/// </summary>
public class JsonValueGeneratorFactory : IJsonValueGeneratorFactory
{
    /// <summary>
    /// Singleton instance of <see cref="JsonValueGeneratorFactory"/> class.
    /// </summary>
    private static JsonValueGeneratorFactory? _instance;

    /// <summary>
    /// Dictionary to store the generated JsonValueGenerator objects.
    /// The key is a tuple of the type and seed, and the value is the corresponding JsonValueGenerator object.
    /// </summary>
    private readonly IDictionary<(string, int), IJsonValueGenerator> _generators;
    
    /// <summary>
    /// Private constructor to prevent direct instantiation.
    /// </summary>
    private JsonValueGeneratorFactory()
    {
        _generators = new Dictionary<(string, int), IJsonValueGenerator>();
    }

    /// <summary>
    /// Returns the singleton instance of the <see cref="JsonValueGeneratorFactory"/> class.
    /// If the instance does not exist, it creates a new one.
    /// </summary>
    /// <returns>The singleton instance of the <see cref="JsonValueGeneratorFactory"/> class.</returns>
    public static JsonValueGeneratorFactory GetInstance() => _instance ??= new JsonValueGeneratorFactory();
    
    /// <summary>
    /// Returns a JsonValueGenerator object based on the given type and seed.
    /// If the JsonValueGenerator object does not exist, it creates a new one.
    /// </summary>
    /// <param name="type">The type of the JsonValueGenerator object.</param>
    /// <param name="seed">The seed for the random number generator.</param>
    /// <returns>The JsonValueGenerator object.</returns>
    public IJsonValueGenerator GetJsonValueGenerator(string type, int seed)
    {
        var generatorKeyOfTypeAndSeed = (type, seed);
        if (!_generators.ContainsKey(generatorKeyOfTypeAndSeed)) 
            _generators.Add(generatorKeyOfTypeAndSeed, BuildJsonValueGenerator(type, seed));
        return _generators[generatorKeyOfTypeAndSeed];
    }
    
    /// <summary>
    /// Builds a JsonValueGenerator object based on the given type and seed.
    /// </summary>
    /// <param name="type">The type of the JsonValueGenerator object.</param>
    /// <param name="seed">The seed for the random number generator.</param>
    /// <returns>The JsonValueGenerator object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the type is not supported.</exception>
    private IJsonValueGenerator BuildJsonValueGenerator(string type, int seed)
    {
        return type switch
        {  
            JsonNodeSchemaExtensions.String => new StringJsonValueGenerator(seed),
            JsonNodeSchemaExtensions.Integer => new IntegerJsonValueGenerator(seed),
            JsonNodeSchemaExtensions.Number => new NumberJsonValueGenerator(seed),
            JsonNodeSchemaExtensions.Boolean => new BooleanJsonValueGenerator(seed),
            JsonNodeSchemaExtensions.Null => new NullJsonValueGenerator(),
            _ => throw new ArgumentException("Json Schema type Value generation not supported", nameof(type))
        };
    }
}