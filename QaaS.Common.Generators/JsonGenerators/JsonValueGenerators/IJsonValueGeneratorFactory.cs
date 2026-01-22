namespace QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

/// <summary>
/// Factory for creating <see cref="IJsonValueGenerator"/> instances.
/// </summary>
public interface IJsonValueGeneratorFactory
{ 
    /// <summary>
    /// Gets a <see cref="IJsonValueGenerator"/> instance for the given type.
    /// </summary>
    /// <param name="type">The type of the value to generate.</param>
    /// <param name="seed">The seed to use for the generator.</param>
    /// <returns>A <see cref="IJsonValueGenerator"/> instance for the given type.</returns>
    IJsonValueGenerator GetJsonValueGenerator(string type, int seed);
}