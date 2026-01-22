using System.Text.Json;
using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonValueGeneratorsTests;

public class NullJsonValueGeneratorTests
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
    public void TestGenerateValue_CallGeneratorClassWithConfigurationProperties_GeneratedValueShouldBeNull
        (Dictionary<string, object>? schemaParameters)
    {
        // Arrange
        schemaParameters ??= new Dictionary<string, object>();
        var jsonSchemaObject = JsonNode.Parse(JsonSerializer.Serialize(schemaParameters)) as JsonObject;
        var generator = new NullJsonValueGenerator();

        // Act
        var result = generator.GenerateValue(jsonSchemaObject!, Globals.rootPath);

        // Assert
        Assert.That(result, Is.Null);
    }
}