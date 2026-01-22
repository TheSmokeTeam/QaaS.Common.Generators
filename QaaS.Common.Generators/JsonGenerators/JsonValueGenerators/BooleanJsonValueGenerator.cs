using System.Text.Json.Nodes;

namespace QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

/// <inheritdoc />
public class BooleanJsonValueGenerator(int seed) : BaseJsonValueGenerator
{
    private Random Random { get; } = new(seed);

    /// <inheritdoc />
    protected override object GenerateRawValue(JsonObject jsonSchemaObject) => Random.Next(2) == 0;
} 