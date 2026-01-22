using System.Text.Json.Nodes;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;

namespace QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

/// <inheritdoc />
public class IntegerJsonValueGenerator : BaseJsonValueGenerator
{
    protected Random Random { get; }

    public IntegerJsonValueGenerator(int seed)
    {
        Random = new Random(seed);
    }
    
    /// <inheritdoc />
    protected override object GenerateRawValue(JsonObject jsonSchemaObject)
    {
        var minimum = jsonSchemaObject.GetJsonSchemaNumberMinimum() ?? Int32.MinValue;
        var maximum = jsonSchemaObject.GetJsonSchemaNumberMaximum() ?? Int32.MaxValue - 1;

        if (minimum > maximum)
            throw new ArgumentException("Minimum cannot be greater than Maximum");
        
        var exclusiveMinimum = jsonSchemaObject.GetJsonSchemaNumberExclusiveMinimum() ?? false;
        var exclusiveMaximum = jsonSchemaObject.GetJsonSchemaNumberExclusiveMaximum() ?? false;
        
        if (minimum == maximum && (exclusiveMinimum || exclusiveMaximum))
            throw new ArgumentException("Minimum and maximum values are equal and at least one of them is exclusive");
        
        var multipleOf = jsonSchemaObject.GetJsonSchemaNumberMultipleOf();
        
        return GenerateValueNumerable(minimum, maximum, exclusiveMinimum, exclusiveMaximum, multipleOf);
    }

    /// <summary>
    /// Checks if the generated number value needs to be regenerated.
    /// </summary>
    /// <param name="generatedNumberValue">The generated number value.</param>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="exclusiveMinimum">Whether the minimum value is exclusive.</param>
    /// <param name="exclusiveMaximum">Whether the maximum value is exclusive.</param>
    /// <param name="multipleOf">The multiple of value.</param>
    /// <returns>True if the generated number value needs to be regenerated, false otherwise.</returns>
    private bool ReGenerateNumberValue(double generatedNumberValue, double minimum, double maximum, bool exclusiveMinimum, 
        bool exclusiveMaximum, double? multipleOf)
    {
        if (exclusiveMinimum && generatedNumberValue == minimum) return true;
        if (exclusiveMaximum && generatedNumberValue == maximum) return true;
        return multipleOf != null && generatedNumberValue % multipleOf != 0;
    }
    
    /// <summary>
    /// Generates a random integer value.
    /// </summary>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="exclusiveMinimum">Whether the minimum value is exclusive.</param>
    /// <param name="exclusiveMaximum">Whether the maximum value is exclusive.</param>
    /// <param name="multipleOf">The multiple of value.</param>
    /// <returns>A random integer value .</returns>
    protected virtual object GenerateValueNumerable(double minimum, double maximum, bool exclusiveMinimum, bool exclusiveMaximum, double? multipleOf)
    {
        int generatedNumberValue;
        int minimumInteger = (int) minimum, maximumInteger = (int) maximum;
        
        do {
            generatedNumberValue = Random.Next(minimumInteger, maximumInteger + 1);
        } while (ReGenerateNumberValue(generatedNumberValue, minimum, maximum, exclusiveMinimum, exclusiveMaximum, multipleOf));

        return generatedNumberValue;
    }
}