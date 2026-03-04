using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json.Nodes;
using DeepEqual.Syntax;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators;
using QaaS.Common.Generators.Tests.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;
using QaaS.Framework.Serialization;


namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests;

public class JsonSchemaDraft4Tests
{
    private static DataSource LoadDataSource(string dataSourceName, object value)
    {
        var dataSource = new DataSource()
        {
            Name = dataSourceName,
        };
        return dataSource.SetGeneratedData(new List<Data<object>>()
        {
            new() { Body = value }
        });
    }

    private static IEnumerable<TestCaseData> TestGenerateCaseSource()
    {
        const string dataSourceName = "Test";

        var jsonSchemaParameter = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                ["Name"] = new JsonObject
                {
                    ["const"] = "Alice"
                },
                ["Age"] = new JsonObject
                {
                    ["const"] = 21
                },
                ["Address"] = new JsonObject
                {
                    ["const"] = new JsonObject
                    {
                        ["Street"] = "123 Main St",
                        ["City"] = "" +
                                   "Anytown"
                    }
                }
            }
        };
        
        var firstExpectedJsonResult = new JsonObject
        {
            ["Name"] = "Alice",
            ["Age"] = 21,
            ["Address"] = new JsonObject
            {
                ["Street"] = "123 Main St",
                ["City"] = "Anytown"
            }
        };
        
        yield return new TestCaseData(
            new JsonSchemaConfiguration
            {
                Count = 1,
                JsonDataSourceName = dataSourceName
            },
            new List<DataSource> { LoadDataSource(dataSourceName, jsonSchemaParameter) }, 
            new List<JsonNode> { firstExpectedJsonResult }
        ).SetName("SingleJson");
        
        yield return new TestCaseData(
            new JsonSchemaConfiguration
            {
                Count = 3,
                JsonDataSourceName = dataSourceName
            },
            new List<DataSource> { LoadDataSource(dataSourceName, jsonSchemaParameter) }, 
            new List<JsonNode> { firstExpectedJsonResult, firstExpectedJsonResult, firstExpectedJsonResult }
        ).SetName("MultipleJsons");

        var secondExpectedJsonResult = firstExpectedJsonResult.DeepClone();
        secondExpectedJsonResult["Name"] = "LittleJohn";
        
        yield return new TestCaseData(
            new JsonSchemaConfiguration
            {
                Count = 1,
                JsonFieldReplacements = new List<JsonFieldReplacement>
                {
                    new JsonFieldReplacement
                    {
                        Path = "$.Name",
                        ValueType = InjectionValueType.String,
                        String = new ManualValue<string> { Value = "LittleJohn"}
                    }
                },
                JsonDataSourceName = dataSourceName
            },
            new List<DataSource> { LoadDataSource(dataSourceName, jsonSchemaParameter) }, 
            new List<JsonNode> { secondExpectedJsonResult }
        ).SetName("SingleJsonWithInjection");
        
        secondExpectedJsonResult= firstExpectedJsonResult.DeepClone();
        secondExpectedJsonResult["Address"]!["Street"] = "WhoCares";
        
        yield return new TestCaseData(
            new JsonSchemaConfiguration
            {
                Count = 3,
                JsonFieldReplacements = new List<JsonFieldReplacement>
                {
                    new()
                    {
                        Path = "$.Address.Street",
                        ValueType = InjectionValueType.String,
                        String = new ManualValue<string> { Value = "WhoCares" }
                    }
                },
                JsonDataSourceName = dataSourceName
            },
            new List<DataSource> { LoadDataSource(dataSourceName, jsonSchemaParameter) }, 
            new List<JsonNode> { secondExpectedJsonResult, secondExpectedJsonResult, secondExpectedJsonResult }
        ).SetName("MultipleJsonsWithInjection");
    }
    
    [Test, TestCaseSource(nameof(TestGenerateCaseSource))]
    public void TestGenerate_CallGenerateFunctionWithJsonDataSource_ShouldReturnExpectedOutputs
        (JsonSchemaConfiguration JsonSchemaConfiguration, IEnumerable<DataSource> dataSources, List<JsonNode> expectedJsonNodeOutputs)
    {
        // Arrange
        var generator = new JsonSchemaDraft4 { Context = Globals.Context,Configuration = JsonSchemaConfiguration};

        // Act
        var output = generator.Generate(
            new List<SessionData>().ToImmutableList(), 
            dataSources.ToImmutableList()
        ).ToList();

        // Assert
        for (var expectedJsonNodeOutputIndex = 0;
             expectedJsonNodeOutputIndex < expectedJsonNodeOutputs.Count;
             expectedJsonNodeOutputIndex++)
        {
            var expectedJsonNodeOutput = expectedJsonNodeOutputs[expectedJsonNodeOutputIndex].ToJsonString();
            Globals.Context.Logger.LogInformation("Expected: {ExpectedJsonNodeOutput}", expectedJsonNodeOutput);
            var outputJsonNode = (output[expectedJsonNodeOutputIndex].Body as JsonNode)!.ToJsonString();
            Globals.Context.Logger.LogInformation("Actual: {OutputJsonNode}", outputJsonNode);
            Assert.That(expectedJsonNodeOutput, Is.EqualTo(outputJsonNode));
        }
    }
    
    [Test]
    public void TestGenerate_CallGenerateFunctionWithJsonDataSourceAndParsing_ShouldReturnExpectedValue()
    {
        // Arrange
        const string dataSourceName = "Test";

        var jsonParameter = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                ["Name"] = new JsonObject { ["const"] = "John" },
                ["Age"] = new JsonObject { ["const"] = 30 },
                ["City"] = new JsonObject { ["const"] = "new york" },
                ["Test"] = new JsonObject { ["type"] = "null" },
                ["Enum"] = new JsonObject { ["const"] = new JsonArray { "SampleValue", "SampleValue", "SampleValue" } },
                ["Floors"] = new JsonObject { ["const"] = new JsonObject {
                    ["F1"] = 1,
                    ["F2"] = 2,
                    ["F3"] = 3
                }}
            }
        };
        
        var jsonSchemaConfiguration = new JsonSchemaConfiguration
        {
            Count = 1,
            JsonFieldReplacements = new List<JsonFieldReplacement> {
                new ()
                {
                    Path = "$.Name",
                    ValueType = InjectionValueType.String,
                    String = new ManualValue<string> { Value = "Alice"}
                }
            },
            OutputObjectType = JsonParserType.Binary,
            OutputObjectTypeConfiguration = new SpecificTypeConfig
            {
                TypeFullName = typeof(JsonParsersTests.Person).FullName,
                AssemblyName = Assembly.GetExecutingAssembly().GetName().Name
            },
            JsonDataSourceName = dataSourceName
        };

        var expectedOutput = new JsonParsersTests.Person
        {
            Name = "Alice",
            Age = 30,
            City = "new york",
            Test = null!,
            Enum = new List<string> { "SampleValue", "SampleValue", "SampleValue" },
            Floors = new Dictionary<string, int>
            {
                ["F1"] = 1,
                ["F2"] = 2,
                ["F3"] = 3
            }
        };
        
        var generator = new JsonSchemaDraft4 { Context = Globals.Context,Configuration = jsonSchemaConfiguration};

        // Act
        var output = generator.Generate(
            new List<SessionData>().ToImmutableList(), 
            new List<DataSource> { LoadDataSource(dataSourceName, jsonParameter) }
                .ToImmutableList()
        ).ToList();

        // Assert
        (output.First().Body as JsonParsersTests.Person).ShouldDeepEqual(expectedOutput);
    }
}
