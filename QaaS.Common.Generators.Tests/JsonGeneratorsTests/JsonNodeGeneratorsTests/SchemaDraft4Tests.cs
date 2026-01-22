using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;
using QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;
using Serilog;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonNodeGeneratorsTests;

public class SchemaDraft4Tests
{
    private const string TestValue = "TestValue";
    private IJsonValueGeneratorFactory _jsonValueGeneratorFactory;
    private ILogger<SchemaDraft4Tests> _logger;

    [SetUp]
    public void SetUp()
    {
        var testJsonValue = JsonValue.Create(TestValue);

        var mockJsonValueGenerator = new Mock<IJsonValueGenerator>();
        mockJsonValueGenerator.Setup(expression =>
                expression.GenerateValue(It.IsAny<JsonObject>(), It.IsAny<string>()))
            .Returns(() => testJsonValue.DeepClone() as JsonValue);

        var mockJsonValueGeneratorFactory = new Mock<IJsonValueGeneratorFactory>();
        mockJsonValueGeneratorFactory.Setup(expression =>
                expression.GetJsonValueGenerator(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(mockJsonValueGenerator.Object);

        _jsonValueGeneratorFactory = mockJsonValueGeneratorFactory.Object;

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        _logger = new LoggerFactory().AddSerilog(Log.Logger).CreateLogger<SchemaDraft4Tests>();
    }

    private static IEnumerable<TestCaseData> _jsonGenerationCaseDatas = new[]
    {
        new TestCaseData(new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["testProperty1"] = new JsonObject { ["type"] = "irrelevantValue" },
                    ["testProperty2"] = new JsonObject { ["type"] = "irrelevantValue" },
                    ["testProperty3"] = new JsonObject { ["type"] = "irrelevantValue" },
                    ["testConstantProperty"] = new JsonObject { ["const"] = 69 }
                }
            },
            new List<JsonNode> { new JsonObject
                {
                    ["testProperty1"] = TestValue,
                    ["testProperty2"] = TestValue,
                    ["testProperty3"] = TestValue,
                    ["testConstantProperty"] = 69
                }
            }
        ).SetName("JsonObjectWithConstant"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["testArray"] = new JsonObject
                    {
                        ["type"] = "array",
                        ["minItems"] = 2,
                        ["maxItems"] = 2,
                        ["items"] = new JsonObject { ["type"] = "irrelevantValue" }
                    },
                    ["testProperty"] = new JsonObject { ["type"] = "irrelevantValue" }
                }
            },
            new List<JsonNode> { new JsonObject
                {
                    ["testArray"] = new JsonArray { TestValue, TestValue},
                    ["testProperty"] = TestValue
                } 
            }
        ).SetName("JsonObjectThatIncludesJsonArray"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["minItems"] = 3,
                ["maxItems"] = 3,
                ["items"] = new JsonObject { ["type"] = "irrelevantValue" }
            },
            new List<JsonNode> {
                new JsonArray { TestValue, TestValue, TestValue}
            }
        ).SetName("JsonArray"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["items"] = new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["testProperty1"] = new JsonObject { ["type"] = "irrelevantValue" },
                        ["testProperty2"] = new JsonObject { ["type"] = "irrelevantValue" },
                        ["testProperty3"] = new JsonObject { ["type"] = "irrelevantValue" },
                        ["testConstantProperty"] = new JsonObject { ["const"] = true }
                    }
                }
            },
            new List<JsonNode> { 
                new JsonArray { 
                    new JsonObject
                    {
                        ["testProperty1"] = TestValue,
                        ["testProperty2"] = TestValue,
                        ["testProperty3"] = TestValue,
                        ["testConstantProperty"] = true
                    }
                } 
            }
        ).SetName("JsonArrayThatIncludesJsonObjectWithConstant"),
        new TestCaseData(new JsonObject
            {
                ["const"] = new JsonObject
                {
                    ["this is a test object"] = "and it works fine"
                }
            },
            new List<JsonNode> { new JsonObject
                {
                    ["this is a test object"] = "and it works fine"
                } 
            }
        ).SetName("ConstantWithJsonObject"),
        new TestCaseData(new JsonObject
            {
                ["const"] = new JsonArray {"this is a test object", "and it works fine" }
            },
            new List<JsonNode> { new JsonArray {"this is a test object", "and it works fine" } }
        ).SetName("ConstantWithJsonArray"),
        new TestCaseData(new JsonObject
            {
                ["type"] = new JsonArray { "object", "array" },
                ["properties"] = new JsonObject
                {
                    ["testProperty1"] = new JsonObject { ["type"] = "irrelevantValue" },
                    ["testProperty2"] = new JsonObject { ["type"] = "irrelevantValue" },
                    ["testProperty3"] = new JsonObject { ["type"] = "irrelevantValue" }
                },
                ["items"] = new JsonObject
                {
                    ["type"] = "irrelevantValue"
                },
                ["minItems"] = 3,
                ["maxItems"] = 3,
            },
            new List<JsonNode>
            {
                new JsonObject
                {
                    ["testProperty1"] = TestValue,
                    ["testProperty2"] = TestValue,
                    ["testProperty3"] = TestValue
                },
                new JsonArray { TestValue, TestValue, TestValue }
            }
        ).SetName("MultipleJsonStructureTypes"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["testProperty"] = new JsonObject { 
                        ["enum"] = new JsonArray
                        {
                            null,
                            true,
                            false,
                            1,
                            1.0,
                            "test",
                            new JsonObject { ["test"] = "test" }
                        }
                    }
                }
            },
            new List<JsonNode>
            {
                new JsonObject { ["testProperty"] = null },
                new JsonObject { ["testProperty"] = true },
                new JsonObject { ["testProperty"] = false },
                new JsonObject { ["testProperty"] = 1 },
                new JsonObject { ["testProperty"] = 1.0 },
                new JsonObject { ["testProperty"] = "test" },
                new JsonObject { ["testProperty"] = new JsonObject { ["test"] = "test" } }
            }
        ).SetName("JsonObjectWithEnum"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["testProperty1"] = new JsonObject { ["type"] = "irrelevantValue" },
                    ["testProperty2"] = new JsonObject
                    {
                        ["type"] = "irrelevantValue",
                        ["$ref"] = "#/definitions/testRef"
                    },
                    ["testProperty3"] = new JsonObject { ["$ref"] = "#/definitions/testRef" },
                    ["testProperty4"] = new JsonObject
                    {
                        ["$ref"] = "#/definitions/testRef",
                        ["const"] = "OverrideRefValue"
                    },
                    ["testArray"] = new JsonObject
                    {
                        ["type"] = "array",
                        ["minItems"] = 3,
                        ["maxItems"] = 3,
                        ["items"] = new JsonObject
                        {
                            ["$ref"] = "#/definitions/testRefArray"
                        }
                    }
                },
                ["definitions"] = new JsonObject
                {
                    ["testRef"] = new JsonObject { ["const"] = "RefValue" },
                    ["testRefArray"] = new JsonObject { ["const"] = "RefValueArray" }
                }
            },
            new List<JsonNode>
            {
                new JsonObject
                {
                    ["testProperty1"] = TestValue,
                    ["testProperty2"] = "RefValue",
                    ["testProperty3"] = "RefValue",
                    ["testProperty4"] = "OverrideRefValue",
                    ["testArray"] = new JsonArray { "RefValueArray", "RefValueArray", "RefValueArray" }
                }
            }
        ).SetName("JsonObjectFromDefinitionsAndRefs")
    };
    

    [Test, TestCaseSource(nameof(_jsonGenerationCaseDatas))]
    public void TestGenerateFromJsonSchema_ConstructGeneratorAndGenerateJsonNodeFromGivenSchema_ShouldBeExpectedStructure(
        JsonObject jsonSchemaObject, List<JsonNode> expectedJsonObjects)
    {
        // Arrange
        var schemaJsonNodeGenerator = new SchemaDraft4JsonNodeGenerator(_logger, _jsonValueGeneratorFactory, jsonSchemaObject);
        var jsonNodesStrings = expectedJsonObjects.Select(json => json.ToJsonString()).ToList();

        // Act
        var resultJsonObject = schemaJsonNodeGenerator.Generate();
        _logger.LogInformation("Result: {JsonObject}", resultJsonObject.ToJsonString());

        // Assert
        Assert.That(jsonNodesStrings, Does.Contain(resultJsonObject.ToJsonString()));
    }
    
    private static IEnumerable<TestCaseData> _jsonArrayRulesGenerationCaseDatas = new[]
    {
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["minItems"] = 0,
                ["maxItems"] = 0,
                ["items"] = new JsonObject { ["type"] = "string" }
            },
            0,
            0,
            false
        ).SetName("EmptyJsonArray"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["minItems"] = 0,
                ["maxItems"] = 0,
                ["uniqueItems"] = true,
                ["items"] = new JsonObject { ["type"] = "string" }
            },
            0,
            0,
            false
        ).SetName("EmptyJsonArrayUniqueItems"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["items"] = new JsonObject { ["type"] = "string" }
            },
            1,
            1,
            false
        ).SetName("JsonArrayWithDefaultItemCount"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["minItems"] = 1,
                ["maxItems"] = 1,
                ["uniqueItems"] = true,
                ["items"] = new JsonObject { ["type"] = "string" }
            },
            1,
            1,
            false
        ).SetName("JsonArrayWithOneItemUniqueItems"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["minItems"] = 1,
                ["maxItems"] = 10,
                ["uniqueItems"] = false,
                ["items"] = new JsonObject { ["type"] = "string" }
            },
            1,
            10,
            false
        ).SetName("JsonArrayWithMultipleItems"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["minItems"] = 1,
                ["maxItems"] = 10,
                ["uniqueItems"] = true,
                ["items"] = new JsonObject { ["type"] = "string" }
            },
            1,
            10,
            true
        ).SetName("JsonArrayWithMultipleItemsUniqueItems"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["items"] = new JsonArray
                {
                    new JsonObject { ["type"] = "string" },
                    new JsonObject { ["type"] = "integer" },
                }
            },
            2,
            2,
            false
        ).SetName("JsonArrayWithDefaultItemCountMultipleItemSchemas"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["minItems"] = 1,
                ["maxItems"] = 10,
                ["items"] = new JsonArray
                {
                    new JsonObject { ["type"] = "string" },
                    new JsonObject { ["type"] = "integer" },
                }
            },
            1,
            10,
            false
        ).SetName("JsonArrayWithMultipleItemsMultipleItemSchemas"),
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["minItems"] = 1,
                ["maxItems"] = 10,
                ["uniqueItems"] = true,
                ["items"] = new JsonArray
                {
                    new JsonObject { ["type"] = "string" },
                    new JsonObject { ["type"] = "integer" },
                }
            },
            1,
            10,
            true
        ).SetName("JsonArrayWithMultipleItemsMultipleItemSchemasUniqueItems"),
    };
    
    [Test, TestCaseSource(nameof(_jsonArrayRulesGenerationCaseDatas))]
    public void TestGenerateFromJsonSchema_ConstructGeneratorAndGenerateJsonArrayFromGivenSchema_ShouldBeExpectedStructureAndRules(
        JsonObject jsonSchemaObject, int minItems = 1, int maxItems = 1, bool uniqueItems = false)
    {
        // Arrange
        var schemaJsonNodeGenerator = new SchemaDraft4JsonNodeGenerator(_logger, JsonValueGeneratorFactory.GetInstance(), jsonSchemaObject);

        // Act
        var resultJsonArray = schemaJsonNodeGenerator.Generate() as JsonArray;
        _logger.LogInformation("Result: {JsonObject}", resultJsonArray!.ToJsonString());

        // Assert
        Assert.That(resultJsonArray, Has.Count.GreaterThanOrEqualTo(minItems));
        Assert.That(resultJsonArray, Has.Count.LessThanOrEqualTo(maxItems));
        
        if (!uniqueItems) return;
        var resultJsonArrayString = resultJsonArray.Select(item => item!.ToJsonString()).ToList();
        Assert.That(resultJsonArrayString.Distinct().Count(), Is.EqualTo(resultJsonArrayString.Count));
    }

    private static IEnumerable<TestCaseData> _jsonEnumAndMultipleTypeRulesGenerationCaseDatas = new[]
    {
        new TestCaseData(new JsonObject
            {
                ["type"] = "array",
                ["minItems"] = 0,
                ["maxItems"] = 0,
                ["items"] = new JsonObject { ["type"] = "string" }
            },
            0,
            0,
            false
        ).SetName("EmptyJsonArray")
    };
}
