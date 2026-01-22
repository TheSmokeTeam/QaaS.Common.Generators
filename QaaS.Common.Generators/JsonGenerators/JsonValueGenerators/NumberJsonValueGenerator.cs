using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

/// <inheritdoc />
public class NumberJsonValueGenerator : IntegerJsonValueGenerator
{
    public NumberJsonValueGenerator(int seed) : base(seed){}

    /// <summary>
    /// Generates a random double value.
    /// </summary>
    /// <param name="minimum">The minimum value.</param>
    /// <param name="maximum">The maximum value.</param>
    /// <param name="exclusiveMinimum">Whether the minimum value is exclusive.</param>
    /// <param name="exclusiveMaximum">Whether the maximum value is exclusive.</param>
    /// <param name="multipleOf">The multiple of value.</param>
    /// <returns>A random double value.</returns>
    /// <exception cref="ArgumentException">Thrown if multipleOf is not null.</exception>
    protected override object GenerateValueNumerable(double minimum, double maximum, bool exclusiveMinimum, bool exclusiveMaximum, double? multipleOf)
    {
        double generatedNumberValue;
        if (multipleOf != null) 
            throw new ArgumentException($"{nameof(multipleOf)} is not supported for {nameof(NumberJsonValueGenerator)}");
        do
        {
            generatedNumberValue = minimum + Random.NextDouble() * (maximum - minimum);
        } while ((exclusiveMinimum && generatedNumberValue == minimum) || (exclusiveMaximum && generatedNumberValue == maximum));

        return generatedNumberValue;
    }
}