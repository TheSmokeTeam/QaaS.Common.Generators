using System.Text.Json.Nodes;

namespace QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

/// <inheritdoc />
public abstract class BaseJsonValueGenerator : IJsonValueGenerator
{
    /// <inheritdoc />
    public JsonValue GenerateValue(JsonObject jsonSchemaObject, string jsonSchemaPath)
    {
        JsonValue jsonValue;
        try
        {
            var value = GenerateRawValue(jsonSchemaObject);
            jsonValue = JsonValue.Create(value) ?? 
                throw new InvalidOperationException($"Cannot create JsonValue from value '{value}'");
        }
        catch (Exception exception)
        {
            throw new ArgumentException($"Couldn't generate value correctly at schema path of '{jsonSchemaPath}'", exception);
        }
        return jsonValue;
    }

    /// <summary>
    /// Generates a raw value from the given Json schema.
    /// </summary>
    /// <param name="jsonSchemaObject">The JSON schema object to generate a value for</param>
    /// <returns>Relevant value for JsonValue generation</returns>
    protected abstract object GenerateRawValue(JsonObject jsonSchemaObject);
}