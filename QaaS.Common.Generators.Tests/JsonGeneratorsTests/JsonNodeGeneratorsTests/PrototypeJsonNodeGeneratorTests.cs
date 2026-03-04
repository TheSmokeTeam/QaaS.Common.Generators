using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;
using Serilog;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonNodeGeneratorsTests;

public class PrototypeJsonNodeGeneratorTests
{
    [SetUp]
    public void Setup() => Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

    private static IEnumerable<TestCaseData> _caseDatas = new[]
    {
        new TestCaseData(new JsonObject()).SetName("EmptyJsonObject"),
        new TestCaseData(new JsonArray()).SetName("EmptyJsonArray"),
        new TestCaseData(new JsonObject { ["name"] = "Alice", ["age"] = 21 }).SetName("JsonObject"),
        new TestCaseData(new JsonArray { "Alice", 21 }).SetName("JsonArray")
    };
  

    [Test, TestCaseSource(nameof(_caseDatas))]
    public void TestPrototypeGeneration_ConstructGeneratorAndGenerateJsonObject_ShouldReturnTheSameJson(
        JsonNode json)
    {
        // Arrange
        var logger = new LoggerFactory().AddSerilog(Log.Logger).CreateLogger<PrototypeJsonNodeGenerator>();
        var generator = new PrototypeJsonNodeGenerator(logger, json);
        
        // Act
        var generatedJson = generator.Generate();
        
        // Assert
        JsonNode.DeepEquals(json, generatedJson);
    }
}
