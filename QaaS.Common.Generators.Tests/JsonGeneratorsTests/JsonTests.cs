using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json.Nodes;
using DeepEqual.Syntax;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.Tests.ConfigurationObjects;
using QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonParsersTests;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests;

public class JsonTests
{

    private static DataSource LoadDataSource(string dataSourceName, object value)
    {
        var dataSource = new DataSource
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

        var jsonParameter = new JsonObject
        {
            ["Name"] = "REDA",
            ["Age"] = "21",
            ["Address"] = new JsonObject
            {
                ["Street"] = "123 Main St",
                ["City"] = "Anytown"
            }
        };

        yield return new TestCaseData(
            new JsonConfiguration
            {
                Count = 1,
                JsonDataSourceName = dataSourceName
            },
            new List<DataSource> { LoadDataSource(dataSourceName, jsonParameter) },
            new List<JsonNode> { jsonParameter }
        ).SetName("SingleJson");

        yield return new TestCaseData(
            new JsonConfiguration
            {
                Count = 3,
                JsonDataSourceName = dataSourceName
            },
            new List<DataSource> { LoadDataSource(dataSourceName, jsonParameter) },
            new List<JsonNode> { jsonParameter, jsonParameter, jsonParameter }
        ).SetName("MultipleJsons");

        var expectedJson = jsonParameter.DeepClone();
        expectedJson["Name"] = "LittleJohn";

        yield return new TestCaseData(
            new JsonConfiguration
            {
                Count = 1,
                JsonFieldReplacements = new List<JsonFieldReplacement>
                {
                    new JsonFieldReplacement
                    {
                        Path = "$.Name",
                        ValueType = InjectionValueType.String,
                        String = new ManualValue<string> { Value = "LittleJohn" }
                    }
                },
                JsonDataSourceName = dataSourceName
            },
            new List<DataSource> { LoadDataSource(dataSourceName, jsonParameter) },
            new List<JsonNode> { expectedJson }
        ).SetName("SingleJsonWithInjection");

        expectedJson = jsonParameter.DeepClone();
        expectedJson["Address"]!["Street"] = "WhoCares";

        yield return new TestCaseData(
            new JsonConfiguration
            {
                Count = 3,
                JsonFieldReplacements = new List<JsonFieldReplacement>
                {
                    new JsonFieldReplacement
                    {
                        Path = "$.Address.Street",
                        ValueType = InjectionValueType.String,
                        String = new ManualValue<string> { Value = "WhoCares" }
                    }
                },
                JsonDataSourceName = dataSourceName

            },
            new List<DataSource> { LoadDataSource(dataSourceName, jsonParameter) },
            new List<JsonNode> { expectedJson, expectedJson, expectedJson }
        ).SetName("MultipleJsonsWithInjection");
    }

    [Test, TestCaseSource(nameof(TestGenerateCaseSource))]
    public void TestGenerate_CallGenerateFunctionWithJsonDataSources_ShouldReturnExpectedOutputs
    (JsonConfiguration jsonConfiguration, IEnumerable<DataSource> dataSourceList,
        List<JsonNode> expectedJsonNodeOutputs)
    {
        // Arrange
        var generator = new JsonGenerators.Json
        {
            Context = Globals.Context,
            Configuration = jsonConfiguration
        };

        // Act
        var output = generator.Generate(
            new List<SessionData>().ToImmutableList(),
            dataSourceList.ToImmutableList()
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
            ["Name"] = "John",
            ["Age"] = 30,
            ["City"] = "new york",
            ["Test"] = null,
            ["Enum"] = new JsonArray { "REDA", "REDA", "REDA" },
            ["Floors"] = new JsonObject
            {
                ["F1"] = 1,
                ["F2"] = 2,
                ["F3"] = 3
            }
        };

        var jsonConfiguration = new JsonConfiguration
        {
            Count = 1,
            JsonFieldReplacements = new List<JsonFieldReplacement>
            {
                new()
                {
                    Path = "$.Name",
                    ValueType = InjectionValueType.String,
                    String = new ManualValue<string> { Value = "REDA" }
                }
            },
            OutputObjectType = JsonParserType.Binary,
            OutputObjectTypeConfiguration = new SpecificTypeConfig
            {
                TypeFullName = typeof(Person).FullName,
                AssemblyName = Assembly.GetExecutingAssembly().GetName().Name
            },
            JsonDataSourceName = dataSourceName
        };

        var expectedOutput = new Person
        {
            Name = "REDA",
            Age = 30,
            City = "new york",
            Test = null!,
            Enum = new List<string> { "REDA", "REDA", "REDA" },
            Floors = new Dictionary<string, int>
            {
                ["F1"] = 1,
                ["F2"] = 2,
                ["F3"] = 3
            }
        };

        var generator = new JsonGenerators.Json { Context = Globals.Context,Configuration = jsonConfiguration};

        // Act
        var output = generator.Generate(
            new List<SessionData>().ToImmutableList(),
            new List<DataSource> { LoadDataSource(dataSourceName, jsonParameter) }
                .ToImmutableList()
        ).ToList();

        // Assert
        (output.First().Body as Person).ShouldDeepEqual(expectedOutput);
    }
}