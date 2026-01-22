using System.Text.Json.Nodes;

namespace QaaS.Common.Generators.JsonGenerators.JsonExtensions;

public static class JsonNodeSchemaExtensions
{
    /// <summary>
    /// Types of Json Schema.
    /// </summary>
    public const string Object = "object";
    public const string Array = "array";
    public const string String = "string";
    public const string Number = "number";
    public const string Integer = "integer";
    public const string Boolean = "boolean";
    public const string Null = "null";
    
    /// <summary>
    /// General properties of Json Schema.
    /// </summary>
    public const string Type = "type";
    public const string Const = "const";
    public const string Enum = "enum";
    public const string Definitions = "definitions";
    public const string Ref = "$ref";
    public const string Id = "id";

    public static IList<string> GetJsonSchemaTypes(this JsonObject jsonSchemaObject)
    {
        var jsonSchemaType = jsonSchemaObject[Type] ??
                             throw new ArgumentException("No 'Type' field could be found in given Json Schema");

        return jsonSchemaType switch
        {
            JsonArray jsonSchemaTypeArray => jsonSchemaTypeArray.Select(jsonSchemaTypeValue =>
            {
                if (jsonSchemaTypeValue is not JsonValue)
                    throw new ArgumentException("Not all 'Type' fields in given Json Schema are values");
                return jsonSchemaTypeValue.GetValue<string>();
            }).ToList(),
            JsonValue jsonSchemaTypeValue => new List<string> { jsonSchemaTypeValue.GetValue<string>() },
            _ => throw new ArgumentOutOfRangeException(nameof(jsonSchemaType), jsonSchemaType, 
                "Json Schema 'Type' value object type not supported")
        };
    }

    public static JsonNode? GetJsonSchemaConst(this JsonObject jsonSchemaObject)
        => jsonSchemaObject[Const];
    
    public static JsonArray? GetJsonSchemaEnum(this JsonObject jsonSchemaObject) 
        => jsonSchemaObject[Enum] as JsonArray;
    
    public static string? GetJsonSchemaRef(this JsonObject jsonSchemaObject) 
        => jsonSchemaObject[Ref]?.GetValue<string>();
    
    public static string? GetJsonSchemaId(this JsonObject jsonSchemaObject) 
        => jsonSchemaObject[Id]?.GetValue<string>();

    public static IDictionary<string, JsonObject> GetJsonSchemaDefinitions(this JsonObject jsonSchemaObject)
    {
        var definitions = new Dictionary<string, JsonObject>();
        if (jsonSchemaObject[Definitions] == null) return definitions;
        var jsonSchemaDefinitions = jsonSchemaObject[Definitions] as JsonObject;
        
        foreach (var jsonSchemaDefinition in jsonSchemaDefinitions!)
        {
            var jsonSchemaDefinitionKey = $"#/definitions/{jsonSchemaDefinition.Key}";
            if (jsonSchemaDefinition.Value is not JsonObject jsonSchemaDefinitionValue)
                throw new ArgumentNullException(jsonSchemaDefinitionKey, 
                    "Json Schema definition value cannot be null");
            definitions[jsonSchemaDefinitionKey] = jsonSchemaDefinitionValue;
        }

        return definitions;
    }
    
    
    /// <summary> 
    /// Properties of type "string".
    /// </summary>
    public const string MinLength = "minLength";
    public const string MaxLength = "maxLength";
    public const string Format = "format";

    public static uint? GetJsonSchemaStringMinLength(this JsonObject jsonSchemaObject)
    {
        if (jsonSchemaObject[MinLength] == null) return null;
        return Convert.ToUInt32(jsonSchemaObject[MinLength]?.GetValue<int>());
    }
    
    public static uint? GetJsonSchemaStringMaxLength(this JsonObject jsonSchemaObject)
    {
        if (jsonSchemaObject[MaxLength] == null) return null;
        return Convert.ToUInt32(jsonSchemaObject[MaxLength]?.GetValue<int>());
    }
    
    public static string? GetJsonSchemaStringFormat(this JsonObject jsonSchemaObject)
        => jsonSchemaObject[Format]?.GetValue<string>();

    
    /// <summary>
    /// Properties of types "integer" and "number".
    /// </summary>
    public const string Minimum = "minimum";
    public const string Maximum = "maximum";
    public const string ExclusiveMinimum = "exclusiveMinimum";
    public const string ExclusiveMaximum = "exclusiveMaximum";
    public const string MultipleOf = "multipleOf";


    public static double? GetJsonSchemaNumberMinimum(this JsonObject jsonSchemaObject)
    {
        var jsonSchemaNumberMinimum = jsonSchemaObject[Minimum];
        if (jsonSchemaNumberMinimum == null) return null;
        try
        {
            return jsonSchemaNumberMinimum.GetValue<double>();
        }
        catch (InvalidOperationException)
        {
            return Convert.ToDouble(jsonSchemaNumberMinimum.GetValue<int>());
        }
    }
    
    public static double? GetJsonSchemaNumberMaximum(this JsonObject jsonSchemaObject) 
    {
        var jsonSchemaNumberMaximum = jsonSchemaObject[Maximum];
        if (jsonSchemaNumberMaximum == null) return null;
        try
        {
            return jsonSchemaNumberMaximum.GetValue<double>();
        }
        catch (InvalidOperationException)
        {
            return Convert.ToDouble(jsonSchemaNumberMaximum.GetValue<int>());
        }
    }
    
    public static bool? GetJsonSchemaNumberExclusiveMinimum(this JsonObject jsonSchemaObject) 
        => jsonSchemaObject[ExclusiveMinimum]?.GetValue<bool>();
    
    public static bool? GetJsonSchemaNumberExclusiveMaximum(this JsonObject jsonSchemaObject) 
        => jsonSchemaObject[ExclusiveMaximum]?.GetValue<bool>();
    
    public static double? GetJsonSchemaNumberMultipleOf(this JsonObject jsonSchemaObject) 
    {
        var jsonSchemaNumberMultipleOf = jsonSchemaObject[MultipleOf];
        if (jsonSchemaNumberMultipleOf == null) return null;
        try
        {
            return jsonSchemaNumberMultipleOf.GetValue<double>();
        }
        catch (InvalidOperationException)
        {
            return Convert.ToDouble(jsonSchemaNumberMultipleOf.GetValue<int>());
        }
    }
    
    /// <summary>
    /// Properties of type "array".
    /// </summary>
    public const string MinItems = "minItems";
    public const string MaxItems = "maxItems";
    public const string UniqueItems = "uniqueItems";
    public const string Items = "items";

    public static uint? GetJsonSchemaArrayMinItems(this JsonObject jsonSchemaArray) 
    {
        if (jsonSchemaArray[MinItems] == null) return null;
        return Convert.ToUInt32(jsonSchemaArray[MinItems]?.GetValue<int>());
    }
    
    public static uint? GetJsonSchemaArrayMaxItems(this JsonObject jsonSchemaArray) 
    {
        if (jsonSchemaArray[MaxItems] == null) return null;
        return Convert.ToUInt32(jsonSchemaArray[MaxItems]?.GetValue<int>());
    }
    
    public static bool? GetJsonSchemaArrayUniqueItems(this JsonObject jsonSchemaArray) 
        => jsonSchemaArray[UniqueItems]?.GetValue<bool>();
    
    public static IList<JsonObject> GetJsonSchemaArrayItems(this JsonObject jsonSchemaArray)
    {
        var jsonSchemaObjectItems = jsonSchemaArray[Items] ?? throw new ArgumentException("No 'Items' field could be found in given Json Schema Array");
        return jsonSchemaObjectItems switch
        {
            JsonArray jsonSchemaObjectItemsArray => jsonSchemaObjectItemsArray.Select(jsonSchemaItemValue =>
            {
                if (jsonSchemaItemValue is not JsonObject jsonSchemaItemValueObject)
                    throw new ArgumentException("Not all 'Type' fields in given Json Schema are values");
                return jsonSchemaItemValueObject;
            }).ToList(),
            JsonObject jsonSchemaObjectItemsObject => new [] { jsonSchemaObjectItemsObject },
            _ => throw new ArgumentOutOfRangeException(nameof(jsonSchemaObjectItems), 
                jsonSchemaObjectItems, "Json Schema Array 'Items' type not supported")
        };
    }
    
    /// <summary>
    /// Properties of type "object".
    /// </summary>
    public const string Properties = "properties";

    public static IDictionary<string, JsonObject> GetJsonSchemaObjectProperties(this JsonObject jsonSchemaObject)
    {
        var jsonSchemaObjectProperties = jsonSchemaObject[Properties] ?? throw new ArgumentException("No 'Properties' field could be found in given Json Schema Object");
        if (jsonSchemaObjectProperties is not JsonObject jsonSchemaObjectPropertiesObject)
            throw new ArgumentException("'Properties' field in given Json Schema Object is not an object itself");

        var jsonSchemaObjectPropertiesDictionary = new Dictionary<string, JsonObject>();
        foreach (var jsonSchemaObjectProperty in jsonSchemaObjectPropertiesObject)
        {
            if (jsonSchemaObjectProperty.Value is not JsonObject jsonSchemaObjectPropertyValue)
                throw new ArgumentException("Not all 'Properties' fields in given Json Schema Object are not an object themselves");
            jsonSchemaObjectPropertiesDictionary[jsonSchemaObjectProperty.Key] = jsonSchemaObjectPropertyValue;
        }
        return jsonSchemaObjectPropertiesDictionary;
    }
}