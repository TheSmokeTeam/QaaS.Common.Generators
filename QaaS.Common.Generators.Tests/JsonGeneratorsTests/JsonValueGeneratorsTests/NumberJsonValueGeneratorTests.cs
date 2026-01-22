using System.Text.Json;
using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;
using QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;
using Serilog;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonValueGeneratorsTests;

public class NumberJsonValueGeneratorTests
{
    [SetUp]
    public void Setup()
    {
        Log.Logger = new LoggerConfiguration().
            WriteTo.Console()
            .CreateLogger();
    }
    
    private static IEnumerable<TestCaseData> _generateValueCaseData = new[]
    {
        new TestCaseData(null).SetName("WithoutAnySchemaProperties"), 
        new TestCaseData(new Dictionary<string, object> {
            { "minimum", 0 }
        }).SetName("WithMinimumSchemaProperties"),
        new TestCaseData(new Dictionary<string, object> {
            { "maximum", 100 }
        }).SetName("WithMaximumSchemaProperties"),
        new TestCaseData(new Dictionary<string, object> {
            { "minimum", 0 },
            { "maximum", 100 }
        }).SetName("WithMinimumAndMaximumSchemaProperties"), 
        new TestCaseData(new Dictionary<string, object> {
            { "exclusiveMinimum", true },
            { "exclusiveMaximum", true }
        }).SetName("WithExclusiveMinimumAndExclusiveMaximumSchemaProperties"), 
        new TestCaseData(new Dictionary<string, object> {
            { "minimum", -4 },
            { "exclusiveMinimum", true }
        }).SetName("WithMinimumAndExclusiveMinimumSchemaProperties"), 
        new TestCaseData(new Dictionary<string, object> {
            { "maximum", 100 },
            { "exclusiveMaximum", true }
        }).SetName("WithMaximumAndExclusiveMaximumSchemaProperties"), 
        new TestCaseData(new Dictionary<string, object> {
            { "minimum", 0 },
            { "maximum", 100 },
            { "exclusiveMinimum", true },
            { "exclusiveMaximum", false }
        }).SetName("WithAllSchemaProperties"),
        new TestCaseData(new Dictionary<string, object> {
            { "minimum", 100.1 },
            { "maximum", 100.9 }
        }).SetName("WithDoubleMinimumAndMaximum"),
        new TestCaseData(new Dictionary<string, object> {
            { "minimum", 100.1 },
            { "maximum", 100.9 },
            { "exclusiveMinimum", true },
            { "exclusiveMaximum", false }
        }).SetName("WithDoubleMinimumAndMaximumAndAllSchemaProperties")
    };


    [Test, TestCaseSource(nameof(_generateValueCaseData))]
    public void TestGenerateValue_CallGeneratorClassWithConfigurationProperties_GeneratedValueShouldFollowSchema
        (Dictionary<string, object>? schemaParameters)
    {
        // Arrange
        schemaParameters ??= new Dictionary<string, object>();
        var jsonSchemaObject = JsonNode.Parse(JsonSerializer.Serialize(schemaParameters)) as JsonObject;
        var generator = new NumberJsonValueGenerator(new Random().Next());
        
        var minimum = jsonSchemaObject!.GetJsonSchemaNumberMinimum() ?? Int32.MinValue;
        var maximum = jsonSchemaObject!.GetJsonSchemaNumberMaximum() ?? Int32.MaxValue - 1;
        var exclusiveMinimum = jsonSchemaObject!.GetJsonSchemaNumberExclusiveMinimum() ?? false;
        var exclusiveMaximum = jsonSchemaObject!.GetJsonSchemaNumberExclusiveMaximum() ?? false;
        
        // Act
        var generatedJsonValue = generator.GenerateValue(jsonSchemaObject!, Globals.rootPath);
        Log.Logger.Information("Generated value: {GeneratedValue}", generatedJsonValue);

        // Assert
        var value = generatedJsonValue.GetValue<double>();
        if (exclusiveMinimum) Assert.IsTrue(value > minimum);
        Assert.That(value >= minimum, Is.True);
        if (exclusiveMaximum) Assert.IsTrue(value < maximum);
        Assert.That(value <= maximum, Is.True);
    }
}