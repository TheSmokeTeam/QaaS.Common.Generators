using System.Collections.Immutable;
using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;
using QaaS.Common.Generators.Tests.ConfigurationObjects;
using QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonParsersTests;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;
using Serilog;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonExtensionsTests;

public class JsonNodeExtensionsTests
{
    const string DataSourceName = "Test";

    private static readonly JsonNode BaseJson = new JsonObject
    {
        ["hello"] = "world",
        ["arrayExample"] = new JsonArray
        {
            "all possible examples",
            new JsonObject
            {
                ["IthinkItWorks"] = "yes daddy"
            }
        }
    };

    [SetUp]
    public void Setup()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console()
            .CreateLogger();
    }


    private static IEnumerable<TestCaseData> _replaceCaseData = new[]
    {
        new TestCaseData("$.hello", "SampleValue", new JsonObject
        {
            ["hello"] = "SampleValue",
            ["arrayExample"] = new JsonArray
            {
                "all possible examples",
                new JsonObject
                {
                    ["IthinkItWorks"] = "yes daddy"
                }
            }
        }).SetName("ReplaceOnRootWithString"),
        new TestCaseData("$.hello", 18, new JsonObject
        {
            ["hello"] = 18,
            ["arrayExample"] = new JsonArray
            {
                "all possible examples",
                new JsonObject
                {
                    ["IthinkItWorks"] = "yes daddy"
                }
            }
        }).SetName("ReplaceOnRootWithInteger"),
        new TestCaseData("$.hello", true, new JsonObject
        {
            ["hello"] = true,
            ["arrayExample"] = new JsonArray
            {
                "all possible examples",
                new JsonObject
                {
                    ["IthinkItWorks"] = "yes daddy"
                }
            }
        }).SetName("ReplaceOnRootWithBoolean"),
        new TestCaseData("$.hello", 420.69, new JsonObject
        {
            ["hello"] = 420.69,
            ["arrayExample"] = new JsonArray
            {
                "all possible examples",
                new JsonObject
                {
                    ["IthinkItWorks"] = "yes daddy"
                }
            }
        }).SetName("ReplaceOnRootWithNumber"),
        new TestCaseData("$.hello", new byte[] { 0x42, 0x06, 0x90, 0x13, 0x37, 0xFF }, new JsonObject
        {
            ["hello"] = "QgaQEzf/",
            ["arrayExample"] = new JsonArray
            {
                "all possible examples",
                new JsonObject
                {
                    ["IthinkItWorks"] = "yes daddy"
                }
            }
        }).SetName("ReplaceOnRootWithByteStream"),
        new TestCaseData("$.arrayExample", null, new JsonObject
        {
            ["hello"] = "world",
            ["arrayExample"] = null
        }).SetName("ReplaceOnRootWithNull"),
        new TestCaseData("$.arrayExample[1]", 42, new JsonObject
        {
            ["hello"] = "world",
            ["arrayExample"] = new JsonArray
            {
                "all possible examples",
                42
            }
        }).SetName("ReplaceOnArrayIndex"),
        new TestCaseData("$.arrayExample[1].IthinkItWorks", "no mommy", new JsonObject
        {
            ["hello"] = "world",
            ["arrayExample"] = new JsonArray
            {
                "all possible examples",
                new JsonObject
                {
                    ["IthinkItWorks"] = "no mommy"
                }
            }
        }).SetName("ReplaceOnArrayIndexSubObject"),
        new TestCaseData(
            "$.hello",
            new Person
            {
                Name = "Alice",
                Age = 21,
                City = "Anytown",
                Test = null,
                Enum = new List<string> { "test" },
                Floors = new Dictionary<string, int>
                {
                    ["test"] = 1
                }
            },
            new JsonObject
            {
                ["hello"] = new JsonObject
                {
                    ["Name"] = "Alice",
                    ["Age"] = 21,
                    ["City"] = "Anytown",
                    ["Test"] = null,
                    ["Enum"] = new JsonArray { "test" },
                    ["Floors"] = new JsonObject
                    {
                        ["test"] = 1
                    },
                },
                ["arrayExample"] = new JsonArray
                {
                    "all possible examples",
                    new JsonObject
                    {
                        ["IthinkItWorks"] = "yes daddy"
                    }
                }
            }).SetName("ReplaceWithComplexObject"),
    };


    [Test, TestCaseSource(nameof(_replaceCaseData))]
    public void TestReplaceFunction_UseReplaceMethodOnJsonObject_ShouldReplacePropertyCorrectly
        (string fieldPath, object value, JsonObject expectedJsonObject)
    {
        // Arrange
        var json = BaseJson.DeepClone();

        // Act
        json.Replace(fieldPath, value);
        Log.Information("AFTER REPLACEMENT: {json}", json.ToJsonString());

        // Assert
        Assert.That(JsonNode.DeepEquals(json, expectedJsonObject), Is.True);
    }

    [Test]
    public void TestReplaceFunction_UseReplaceMethodOnJsonObjectWithFieldThatDoesntExist_OutputShouldBeAnException()
    {
        // Arrange
        var json = BaseJson.DeepClone();
        var fieldPath = "$.doesnt.exist";
        var value = "testValue";

        // Act + Assert
        Assert.Throws<ArgumentException>(() => json.Replace(fieldPath, value));
    }

    private static IEnumerable<TestCaseData> TestGenerateCaseSource()
    {
        JsonNode json = new JsonObject
        {
            ["Name"] = "Alice",
            ["Age"] = "21",
            ["Address"] = new JsonObject
            {
                ["Street"] = "123 Main St",
                ["City"] = "Anytown"
            }
        };

        var dataSource = new DataSource()
        {
            Name = DataSourceName,
        };
        dataSource.SetGeneratedData(new List<Data<object>>() { new() { Body = json } });

        yield return new TestCaseData(
            new List<DataSource> { dataSource }.ToImmutableList(),
            json
        ).SetName("JsonObject");

        json = new JsonArray
        {
            1,
            "SampleValue",
            true,
            new JsonObject
            {
                ["example"] = "test"
            }
        };

        dataSource = new DataSource()
        {
            Name = DataSourceName,
        };
        dataSource.SetGeneratedData(new List<Data<object>>() { new() { Body = json } });

        yield return new TestCaseData(
            new List<DataSource> { dataSource }.ToImmutableList(),
            json
        ).SetName("JsonArray");
    }

    [Test, TestCaseSource(nameof(TestGenerateCaseSource))]
    public void TestJsonNodeFromDataSources_CallFunctionWithDataSource_ShouldRetrieveExpectedJson(
        IImmutableList<DataSource> dataSourceList, JsonNode expectedJson)
    {
        // Act 
        var retrievedJson =
            JsonNodeExtensions.GetJsonNodeFromDataSource(ImmutableArray.Create<SessionData>().ToImmutableList(),
                dataSourceList, DataSourceName);

        // Assert
        JsonNode.DeepEquals(retrievedJson, expectedJson);
    }

    [Test]
    public void TestJsonNodeFromDataSource_CallFunctionWithEmptyDataSourceList_ShouldThrowException()
    {
        // Arrange
        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            JsonNodeExtensions.GetJsonNodeFromDataSource(ImmutableArray.Create<SessionData>().ToImmutableList(),
                new List<DataSource>().ToImmutableList(), DataSourceName));
    }

    [Test]
    public void TestJsonNodeFromDataSource_CallFunctionWithDataSourceListValuedNull_ShouldThrowException()
    {
        // Arrange
        var dataSource = new DataSource().SetGeneratedData(null);
        var dataSourceList = new List<DataSource>
        {
            dataSource
        };

        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            JsonNodeExtensions.GetJsonNodeFromDataSource(ImmutableArray.Create<SessionData>().ToImmutableList(),
                dataSourceList.ToImmutableList(), DataSourceName));
    }

    [Test]
    public void TestJsonNodeFromDataSource_CallFunctionWithDataSourceListContainingEmptyList_ShouldThrowException()
    {
        // Arrange
        var dataSource = new DataSource().SetGeneratedData(new List<Data<object>>());
        var dataSourceList = new List<DataSource>
        {
            dataSource
        };

        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            JsonNodeExtensions.GetJsonNodeFromDataSource(ImmutableArray.Create<SessionData>().ToImmutableList(),
                dataSourceList.ToImmutableList(), DataSourceName));
    }

    [Test]
    public void
        TestJsonNodeFromDataSource_CallFunctionWithDataSourceListContainingMoreThanASingleKeyValuePairs_ShouldThrowException()
    {
        // Arrange
        var dataSource = new DataSource().SetGeneratedData(new List<Data<object>>()
        {
            new() { Body = "Value1" },
            new() { Body = "Value2" }
        });
        var dataSourceList = new List<DataSource>
        {
            dataSource
        };

        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            JsonNodeExtensions.GetJsonNodeFromDataSource(ImmutableArray.Create<SessionData>().ToImmutableList(),
                dataSourceList.ToImmutableList(), DataSourceName));
    }

    [Test]
    public void
        TestJsonNodeFromDataSource_CallFunctionWithDataSourceListContainingASingleDataWithoutJson_ShouldThrowException()
    {
        // Arrange
        var dataSource = new DataSource().SetGeneratedData(new List<Data<object>>()
        {
            new()
            {
                Body = "NotJsonObjectLol"
            }
        });
        var dataSourceList = new List<DataSource>
        {
            dataSource
        };

        // Act + Assert
        Assert.Throws<ArgumentException>(() =>
            JsonNodeExtensions.GetJsonNodeFromDataSource(ImmutableArray.Create<SessionData>().ToImmutableList(),
                dataSourceList.ToImmutableList(), DataSourceName));
    }
}
