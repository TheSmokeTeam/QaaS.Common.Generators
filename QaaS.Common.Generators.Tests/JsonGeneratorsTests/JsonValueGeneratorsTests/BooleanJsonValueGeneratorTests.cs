using System.Text.Json;
using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonValueGeneratorsTests;

public class BooleanJsonValueGeneratorTests
{
    private static IEnumerable<TestCaseData> _generateValueCaseData = new[]
    {
        new TestCaseData(null).SetName("WithoutAnySchemaProperties"),
        new TestCaseData(new Dictionary<string, object>
        {
            { "Doesn't Matter", "Ooof" }
        }).SetName("WithRandomSchemaProperties"),
    };
    
    [Test, TestCaseSource(nameof(_generateValueCaseData))]
    public void TestGenerateValue_CallGeneratorClassWithConfigurationProperties_GeneratedValueShouldBeBoolean
        (Dictionary<string, object>? schemaParameters)
    {
        // Arrange
        schemaParameters ??= new Dictionary<string, object>();
        var jsonSchemaObject = JsonNode.Parse(JsonSerializer.Serialize(schemaParameters)) as JsonObject;
        var generator = new BooleanJsonValueGenerator(new Random().Next());
 
        // Assert + Act
        var generatedJsonValue = generator.GenerateValue(jsonSchemaObject!, Globals.rootPath);

        // Assert
        generatedJsonValue.GetValue<bool>(); 
    }
}