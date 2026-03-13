using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;
using QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

namespace QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;

/// <summary>
/// Represents a generator that creates JsonNode objects based a json schema draft4.
/// </summary>
public class SchemaDraft4JsonNodeGenerator : IJsonNodeGenerator
{
    private const int MaxUniqueItemGenerationAttempts = 1000;

    private ILogger Logger { get; set; }
    private string? JsonSchemaId { get; set; }
    private JsonObject BaseJsonObjectSchema { get; set; }
    private int Seed { get; set; }
    private Random Random { get; set; }
    private IJsonValueGeneratorFactory JsonValueGeneratorFactory { get; set; }
    private IDictionary<string, JsonObject> JsonSchemaDefinitions { get; set; }

    
    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaDraft4JsonNodeGenerator"/> class.
    /// </summary>
    /// <param name="logger">The logger to use for logging.</param>
    /// <param name="jsonValueGeneratorFactory"> The factory to use for creating JsonValueGenerators.</param>
    /// <param name="baseJsonNodeSchema">The base JsonNode to use as a schema.</param>
    /// <param name="seed">The seed to use for random number generation. If not provided, a random seed is generated.</param>
    public SchemaDraft4JsonNodeGenerator(ILogger logger, IJsonValueGeneratorFactory jsonValueGeneratorFactory, 
        JsonNode baseJsonNodeSchema, int? seed = null)
    {
        if (baseJsonNodeSchema is not JsonObject baseJsonObjectSchema)
            throw new ArgumentException("Json given should be of type 'JsonObject' to represent Json Schema");
        
        Logger = logger;
        BaseJsonObjectSchema = baseJsonObjectSchema;
        JsonValueGeneratorFactory = jsonValueGeneratorFactory;
        
        JsonSchemaId = BaseJsonObjectSchema.GetJsonSchemaId();
        JsonSchemaDefinitions = BaseJsonObjectSchema.GetJsonSchemaDefinitions();
        
        if (seed.HasValue)
        {
            Seed = (int) seed;
        }
        else
        {
            Seed = new Random().Next();
            Logger.LogDebug("Seed was not given, so random seed '{Seed}' was assigned", Seed);
        }

        Random = new Random(Seed);
    }

    /// <summary>
    /// Generates a new JsonNode object based on a Json Schema property.
    /// </summary>
    /// <returns>A new JsonNode object.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    public JsonNode Generate()
    {
        if (JsonSchemaId != null) 
            Logger.LogDebug("Generating objects from '{JsonSchemaId}' Json Schema",JsonSchemaId);
        else Logger.LogDebug("Generating objects from an anonymous Json Schema");
        
        var rootPath = "$";
        return Generate(BaseJsonObjectSchema, rootPath)!;
    } 

    /// <summary>
    /// Resolves $ref inside the Json Schema.
    /// Also removes $ref from the Json Schema so it won't have to be resolved again.
    /// TODO resolve http $ref?
    /// </summary>
    /// <param name="jsonSchemaObject">The JsonObject Schema to resolve</param>
    private void ResolveRef(JsonObject jsonSchemaObject)
    {
        var jsonSchemaRef = jsonSchemaObject.GetJsonSchemaRef();
        if (jsonSchemaRef == null) return;
            
        var jsonSchemaDefinition = JsonSchemaDefinitions[jsonSchemaRef];

        foreach (var jsonSchemaDefinitionKeyValueFieldPair in jsonSchemaDefinition)
        {
            if (!jsonSchemaObject.TryGetPropertyValue(jsonSchemaDefinitionKeyValueFieldPair.Key, out var placeholder))
            {
                var jsonSchemaDefinitionValue = jsonSchemaDefinitionKeyValueFieldPair.Value!.AsValue().DeepClone();
                jsonSchemaObject[jsonSchemaDefinitionKeyValueFieldPair.Key] = jsonSchemaDefinitionValue;
            }
        }
        
        Logger.LogDebug("Resolved '{ref}' $ref in '{refPathInSchema}' schema", jsonSchemaRef, 
            jsonSchemaObject.GetPath());
        jsonSchemaObject.Remove(JsonNodeSchemaExtensions.Ref);
    }

    /// <summary>
    /// Generates a JsonNode based on the provided JsonObject Schema.
    /// Returns the first given value by the generation order of const -> enum -> type.
    /// </summary>
    /// <param name="jsonSchemaObject">The JsonObject Schema to generate from.</param>
    /// <param name="jsonSchemaPath">Path of the schema block inside of the Json Schema.</param>
    /// <returns>A JsonNode.</returns>
    private JsonNode? Generate(JsonObject jsonSchemaObject, string jsonSchemaPath)
    {
        ResolveRef(jsonSchemaObject);
        
        var constValue = jsonSchemaObject.GetJsonSchemaConst();
        if (constValue != null) 
            return constValue.DeepClone();
        
        var enumValues  = jsonSchemaObject.GetJsonSchemaEnum();
        if (enumValues != null && enumValues.Count != 0)
            return enumValues[Random.Next(enumValues.Count)]?.DeepClone();

        var availableTypes = jsonSchemaObject.GetJsonSchemaTypes();
        var type = availableTypes[Random.Next(availableTypes.Count)];
        
        return type switch 
        {
            JsonNodeSchemaExtensions.Object => GenerateJsonObject(jsonSchemaObject, jsonSchemaPath),
            JsonNodeSchemaExtensions.Array => GenerateJsonArray(jsonSchemaObject, jsonSchemaPath),
            _ => JsonValueGeneratorFactory.GetJsonValueGenerator(type, Seed).GenerateValue(jsonSchemaObject, jsonSchemaPath)
        };
    }


    /// <summary>
    /// Generates a JsonObject based on the provided JsonObject Schema.
    /// </summary>
    /// <param name="jsonSchemaObject">The JsonObject Schema to generate from.</param>
    /// <param name="jsonSchemaPath">Path of the schema block inside of the Json Schema.</param>
    /// <returns>A JsonObject.</returns>
    private JsonNode GenerateJsonObject(JsonObject jsonSchemaObject, string jsonSchemaPath)
    {
        var jsonObject = new JsonObject();

        var jsonObjectPropertiesObject = jsonSchemaObject.GetJsonSchemaObjectProperties();

        foreach (var jsonObjectProperty in jsonObjectPropertiesObject)
        {
            jsonObject.Add(jsonObjectProperty.Key, 
                Generate(jsonObjectProperty.Value, jsonSchemaPath + $".{jsonObjectProperty.Key}"));
        }

        return jsonObject;
    }
    
    /// <summary>
    /// Generates a JsonArray based on the provided JsonObject Schema.
    /// </summary>
    /// <param name="jsonSchemaArray">The JsonObject Schema to generate from.</param>
    /// <param name="jsonSchemaPath">Path of the schema block inside of the Json Schema.</param>
    /// <returns>A JsonArray.</returns>
    private JsonNode GenerateJsonArray(JsonObject jsonSchemaArray, string jsonSchemaPath)
    {
        var jsonArraySchemaItems = jsonSchemaArray.GetJsonSchemaArrayItems().ToList();

        var jsonArrayMinItems = (int) (jsonSchemaArray.GetJsonSchemaArrayMinItems() ?? (uint) jsonArraySchemaItems.Count);
        var jsonArrayMaxItems = (int) (jsonSchemaArray.GetJsonSchemaArrayMaxItems() ?? (uint) jsonArraySchemaItems.Count);
        
        var jsonArrayLength = jsonArrayMinItems == jsonArrayMaxItems ?
            jsonArrayMinItems : Random.Next(jsonArrayMinItems, jsonArrayMaxItems + 1);

        if (jsonArrayLength == 0) return new JsonArray();
        
        var jsonSchemaArrayUniqueItems = jsonSchemaArray.GetJsonSchemaArrayUniqueItems() ?? false;
        if (!jsonSchemaArrayUniqueItems) return GenerateJsonArrayUniqueItemsDisabled(jsonArraySchemaItems, jsonArrayLength, jsonSchemaPath);
        
        Logger.LogWarning("UniqueItems is enabled for '{SchemaPath}' in Json Schema, " +
                          "might be stuck if constant provided", jsonSchemaArray.GetPath());
        
        return GenerateJsonArrayUniqueItemsEnabled(jsonArraySchemaItems, jsonArrayLength, jsonSchemaPath);
    }

    /// <summary>
    /// Generates a JsonArray with unique items based on the provided JsonObject Schemas.
    /// </summary>
    /// <param name="jsonArraySchemaItems">The JsonObject Schemas to generate from.</param>
    /// <param name="jsonArrayLength">The length of the JsonArray to generate.</param>
    /// <param name="jsonSchemaPath">Path of the schema block inside of the Json Schema.</param>
    /// <returns>A JsonArray with unique items.</returns>
    private JsonArray GenerateJsonArrayUniqueItemsEnabled(IEnumerable<JsonObject> jsonArraySchemaItems, int jsonArrayLength, string jsonSchemaPath)
    {
        var jsonArray = new JsonArray();
        var jsonArrayUniqueItems = new HashSet<string>(StringComparer.Ordinal);
        
        for (var jsonArrayItemIndex = 0; jsonArrayItemIndex < jsonArrayLength; jsonArrayItemIndex++)
        {
            var jsonArrayItemSchema = jsonArraySchemaItems.ElementAt(Random.Next(jsonArraySchemaItems.Count()));
            var jsonArrayItemGeneration = Generate(jsonArrayItemSchema, jsonSchemaPath + $"[{jsonArrayItemIndex}]");
            var jsonArrayItemGenerationSerialized = jsonArrayItemGeneration?.ToJsonString() ?? "null";
            var generationAttempts = 0;

            while (!jsonArrayUniqueItems.Add(jsonArrayItemGenerationSerialized))
            {
                generationAttempts++;
                if (generationAttempts >= MaxUniqueItemGenerationAttempts)
                    throw new InvalidOperationException(
                        $"Could not generate {jsonArrayLength} unique items for schema path {jsonSchemaPath}. " +
                        $"Generation kept producing duplicate value {jsonArrayItemGenerationSerialized}.");

                jsonArrayItemGeneration = Generate(jsonArrayItemSchema, jsonSchemaPath + $"[{jsonArrayItemIndex}]");
                jsonArrayItemGenerationSerialized = jsonArrayItemGeneration?.ToJsonString() ?? "null";
            }
            
            jsonArray.Add(jsonArrayItemGeneration);
        }

        return jsonArray;
    }

    /// <summary>
    /// Generates a JsonArray with non-unique items based on the provided JsonObject Schemas.
    /// </summary>
    /// <param name="jsonArraySchemaItems">The JsonObject Schemas to generate from.</param>
    /// <param name="jsonArrayLength">The length of the JsonArray to generate.</param>
    /// <param name="jsonSchemaPath">Path of the schema block inside of the Json Schema.</param>
    /// <returns>A JsonArray with non-unique items.</returns>
    private JsonArray GenerateJsonArrayUniqueItemsDisabled(IEnumerable<JsonObject> jsonArraySchemaItems, int jsonArrayLength, string jsonSchemaPath)
    {
        var jsonArray = new JsonArray();
        for (var jsonArrayItemIndex = 0; jsonArrayItemIndex < jsonArrayLength; jsonArrayItemIndex++)
        {
            var jsonArrayItemGeneration = 
                Generate(jsonArraySchemaItems
                        .ElementAt(Random.Next(jsonArraySchemaItems.Count())), 
                    jsonSchemaPath + $"[{jsonArrayItemIndex}]");
            jsonArray.Add(jsonArrayItemGeneration);
        }
        return jsonArray;
    }
}
