using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;

namespace QaaS.Common.Generators.JsonGenerators;

/// <summary>
/// Injects fields into a JSON node.
/// </summary>
public class JsonNodeFieldInjector
{
    /// <summary>
    /// A dictionary of generation names to the generation enumerable.
    /// This is used to inject generation data into the JSON node.
    /// </summary>
    private readonly IDictionary<string, GenerationEnumerable> _generationsEnumerableDictionary;
    
    /// <summary>
    /// The JSON field replacements to commit.
    /// </summary>
    private readonly IList<JsonFieldReplacement> _jsonFieldReplacements;
    
    /// <summary>
    /// Creates a new instance of the <see cref="JsonNodeFieldInjector"/> class.
    /// </summary>
    public JsonNodeFieldInjector(IDictionary<string, GenerationEnumerable> generationsEnumerableDictionary,  
        IList<JsonFieldReplacement> jsonFieldReplacements)
    {
        _generationsEnumerableDictionary = generationsEnumerableDictionary;
        _jsonFieldReplacements = jsonFieldReplacements;
    }

    /// <summary>
    /// Replaces fields with injection in the provided JSON node.
    /// </summary>
    /// <param name="jsonNode">The JSON node to replace static fields in.</param>
    public void ReplaceFields(JsonNode jsonNode)
    {
        foreach (var jsonFieldReplacement in _jsonFieldReplacements)
        { 
            if (jsonFieldReplacement.ValueType == InjectionValueType.FromDataSource) ReplaceFieldsWithGenerationInjection(jsonNode, jsonFieldReplacement);
            else ReplaceFieldsWithManualInjection(jsonNode, jsonFieldReplacement);
        }
    }
    
    /// <summary>
    /// Replaces fields with generation injection in the provided JSON node.
    /// </summary>
    /// <param name="jsonNode">The JSON node to replace static fields in.</param>
    /// <param name="jsonFieldReplacement">The static field replacement configuration.</param>
    private void ReplaceFieldsWithGenerationInjection(JsonNode jsonNode,
        JsonFieldReplacement jsonFieldReplacement)
    {
        var generationExists = _generationsEnumerableDictionary.TryGetValue(jsonFieldReplacement.FromDataSource!.Name!, out var generationEnumerable);
        if (!generationExists)
            throw new ArgumentException($"Generation {jsonFieldReplacement.FromDataSource!.Name!} not passed to Json Generator");
  
        var generationGeneratedValue =
            generationEnumerable!.GetNextRecord(jsonFieldReplacement.FromDataSource.OutOfRangePolicy).Body!;
        jsonNode.Replace(jsonFieldReplacement.Path, generationGeneratedValue);
    }

    /// <summary>
    /// Replaces fields with manual injection in the provided JSON node.
    /// </summary>
    /// <param name="jsonNode">The JSON node to replace static fields in.</param>
    /// <param name="jsonFieldReplacement">The static field replacement configuration.</param>
    private void ReplaceFieldsWithManualInjection(JsonNode jsonNode, JsonFieldReplacement jsonFieldReplacement)
    {
        var manualValue = jsonFieldReplacement.GetManualValue();
        jsonNode.Replace(jsonFieldReplacement.Path, manualValue);
    }
}