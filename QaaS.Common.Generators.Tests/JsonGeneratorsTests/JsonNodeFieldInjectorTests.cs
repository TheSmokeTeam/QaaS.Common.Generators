using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators;
using QaaS.Framework.SDK.Session.DataObjects;
using Serilog;
using DateTime = QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations.DateTime;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests;

public class JsonNodeFieldInjectorTests
{
    [SetUp]
    public void Setup() => Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    
    private static readonly JsonNode JsonNode = new JsonObject
    {
        ["name"] = "John",
        ["age"] = 13,
        ["salary"] = 69.420,
        ["is_man"] = true,
        ["hash"] = "random hash",
        ["zero"] = 0
    };
    
    private static IEnumerable<TestCaseData> TestGenerateCaseSource()
    {
        var generationsEnumerableDictionary = new Dictionary<string, GenerationEnumerable>();
        
        var jsonFieldReplacements = new List<JsonFieldReplacement>();
        var injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        var expectedJson = JsonNode.DeepClone();
        
        yield return new TestCaseData(injector, expectedJson).SetName("NoJsonFieldReplacements");
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.name",
                ValueType = InjectionValueType.String,
                String = new ManualValue<string> { Value = "Alice" }
            }
        };
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        expectedJson = JsonNode.DeepClone();
        expectedJson["name"] = "Alice";
        
        yield return new TestCaseData(injector, expectedJson).SetName("SingleStaticJsonFieldReplacements");
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.name",
                ValueType = InjectionValueType.DateTime,
                DateTime = new DateTime
                {
                    Year = 1969,
                    Month = 11,
                    Day = 13,
                    Hour = 4,
                    Minute = 20,
                    Second = 30,
                    Millisecond = 420
                }
            }
        };
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        expectedJson = JsonNode.DeepClone();
        expectedJson["name"] = "1969-11-13T04:20:30.4200000Z";
        
        yield return new TestCaseData(injector, expectedJson).SetName("SingleStaticJsonFieldReplacementsOfDateTime");
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.name",
                ValueType = InjectionValueType.UnixEpochTime,
                UnixEpochTime = new UnixEpochTime
                {
                    Year = 1969,
                    Month = 11,
                    Day = 13,
                    Hour = 4,
                    Minute = 20,
                    Second = 30,
                    Millisecond = 420,
                    UnixEpochScaleType = UnixEpochScaleType.Milliseconds
                }
            }
        };
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        expectedJson = JsonNode.DeepClone();
        expectedJson["name"] = -4217969580;
        
        yield return new TestCaseData(injector, expectedJson).SetName("SingleStaticJsonFieldReplacementsOfUnixEpochTime");
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.name",
                ValueType = InjectionValueType.String,
                String = new ManualValue<string> { Value = "Alice" },
                Integer = new ManualValue<int> { Value = 16 },
                Double = new ManualValue<double> {Value = 1337.1337},
                Boolean = new ManualValue<bool> { Value = false},
                ByteArray = new ManualValue<byte[]> { Value = new byte[] { 0x42, 0x06, 0x90, 0x13, 0x37, 0xFF } },
                FromDataSource = new FromDataSource { Name = "Irrelevant",}
            }
        };
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        expectedJson = JsonNode.DeepClone();
        expectedJson["name"] = "Alice";
        
        yield return new TestCaseData(injector, expectedJson)
            .SetName("SingleStaticJsonFieldReplacementsWithMultipleConfigurations");
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.zero",
                ValueType = InjectionValueType.Null
            }
        };
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        expectedJson = JsonNode.DeepClone();
        expectedJson["zero"] = null;
        
        yield return new TestCaseData(injector, expectedJson).SetName("SingleNullStaticJsonFieldReplacements");
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.zero",
                ValueType = InjectionValueType.Null,
                String = new ManualValue<string> { Value = "Alice" },
                Integer = new ManualValue<int> { Value = 16 },
                Double = new ManualValue<double> {Value = 1337.1337},
                Boolean = new ManualValue<bool> { Value = false},
                ByteArray = new ManualValue<byte[]> { Value = new byte[] { 0x42, 0x06, 0x90, 0x13, 0x37, 0xFF } },
                FromDataSource = new FromDataSource { Name = "Irrelevant",}
            }
        };
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        expectedJson = JsonNode.DeepClone();
        expectedJson["zero"] = null;
        
        yield return new TestCaseData(injector, expectedJson)
            .SetName("SingleNullStaticJsonFieldReplacementsWithMultipleConfigurations");
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.name",
                ValueType = InjectionValueType.String,
                String = new ManualValue<string> { Value = "Adi" }
            },
            new ()
            {
                Path = "$.age",
                ValueType = InjectionValueType.Integer,
                Integer = new ManualValue<int> { Value = 16 }
            },
            new ()
            {
                Path = "$.salary",
                ValueType = InjectionValueType.Double,
                Double = new ManualValue<double> { Value = 1337.1337 }
            },
            new ()
            {
                Path = "$.is_man",
                ValueType = InjectionValueType.Boolean,
                Boolean = new ManualValue<bool> { Value = false }
            },
            new ()
            {
                Path = "$.hash",
                ValueType = InjectionValueType.ByteArray,
                ByteArray = new ManualValue<byte[]> { Value = new byte[] { 0x42, 0x06, 0x90, 0x13, 0x37, 0xFF } }
            },
            new ()
            {
                Path = "$.zero",
                ValueType = InjectionValueType.Null
            }
        };
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        expectedJson = new JsonObject
        {
            ["name"] = "Adi",
            ["age"] = 16,
            ["salary"] = 1337.1337,
            ["is_man"] = false,
            ["hash"] = "QgaQEzf/",
            ["zero"] = null
        };

        yield return new TestCaseData(injector, expectedJson).SetName("MultipleStaticJsonFieldReplacements");

        var dataEnumerable = new List<Data<object>>
        {
            new () { Body = "GeneratedValue1" },
            new () { Body = 1337.1337 },
            new () { Body = new byte[] { 0x42, 0x06, 0x90, 0x13, 0x37, 0xFF } }
        };
        
        generationsEnumerableDictionary = new Dictionary<string, GenerationEnumerable>
        {
            ["GenerationPlaceHolderName"] = new (dataEnumerable)
        };

        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.name",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource { Name = "GenerationPlaceHolderName" }
            }
        };
        
        expectedJson = JsonNode.DeepClone();
        expectedJson["name"] = "GeneratedValue1";
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        
        yield return new TestCaseData(injector, expectedJson).SetName("SingleGenerationJsonFieldReplacements");
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.salary",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource { Name = "GenerationPlaceHolderName" }
            },
            new()
            {
                Path = "$.hash",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource { Name = "GenerationPlaceHolderName" }
            }
        };
        
        expectedJson = JsonNode.DeepClone();
        expectedJson["salary"] = 1337.1337;
        expectedJson["hash"] = "QgaQEzf/";
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        
        yield return new TestCaseData(injector, expectedJson).SetName("MultipleGenerationJsonFieldReplacements");

        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.salary",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource { Name = "GenerationPlaceHolderName" }
            },
            new()
            {
                Path = "$.hash",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource { Name = "GenerationPlaceHolderName" }
            }
        };
        
        expectedJson = JsonNode.DeepClone();
        expectedJson["salary"] = null;
        expectedJson["hash"] = null;
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        
        yield return new TestCaseData(injector, expectedJson).SetName("MultipleGenerationJsonFieldReplacementsLoopPolicyFalse");
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.name",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource
                {
                    Name = "GenerationPlaceHolderName", 
                    OutOfRangePolicy = OutOfRangePolicy.Loop
                }
            },
            new()
            {
                Path = "$.salary",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource
                {
                    Name = "GenerationPlaceHolderName", 
                    OutOfRangePolicy = OutOfRangePolicy.Loop
                }
            },
            new()
            {
                Path = "$.hash",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource
                {
                    Name = "GenerationPlaceHolderName", 
                    OutOfRangePolicy = OutOfRangePolicy.Loop
                }
            }
        };
        
        expectedJson = JsonNode.DeepClone();
        expectedJson["name"] = "GeneratedValue1";
        expectedJson["salary"] = 1337.1337;
        expectedJson["hash"] = "QgaQEzf/";
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        
        yield return new TestCaseData(injector, expectedJson).SetName("MultipleGenerationJsonFieldReplacementsLoopPolicyTrue");
        
        
        jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new()
            {
                Path = "$.name",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource
                {
                    Name = "GenerationPlaceHolderName", 
                    OutOfRangePolicy = OutOfRangePolicy.Loop
                }
            },
            new()
            {
                Path = "$.salary",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource
                {
                    Name = "GenerationPlaceHolderName", 
                    OutOfRangePolicy = OutOfRangePolicy.Loop
                }
            },
            new()
            {
                Path = "$.hash",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource
                {
                    Name = "GenerationPlaceHolderName", 
                    OutOfRangePolicy = OutOfRangePolicy.Loop
                }
            },
            new ()
            {
                Path = "$.age",
                ValueType = InjectionValueType.Integer,
                Integer = new ManualValue<int> { Value = 16 }
            },
            new ()
            {
                Path = "$.is_man",
                ValueType = InjectionValueType.Boolean,
                Boolean = new ManualValue<bool> { Value = false }
            },
            new ()
            {
                Path = "$.zero",
                ValueType = InjectionValueType.Null
            }
        };
        
        expectedJson = new JsonObject
        {
            ["name"] = "GeneratedValue1",
            ["age"] = 16,
            ["salary"] = 1337.1337,
            ["is_man"] = false,
            ["hash"] = "QgaQEzf/",
            ["zero"] = null
        };
        
        injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        
        yield return new TestCaseData(injector, expectedJson).SetName("MultipleGenerationAndStaticJsonFieldReplacementsLoopPolicyTrue");
    }

    [Test, TestCaseSource(nameof(TestGenerateCaseSource))]
    public void TestFieldInjector_CallReplaceFieldsOnJsonNode_ShouldReplaceFieldsCorrectly(
        JsonNodeFieldInjector injector, JsonNode expectedJson)
    {
        // Arrange
        var testedJson = JsonNode.DeepClone();

        // Act
        Log.Information("Before replacement: {json}", testedJson.ToJsonString());
        injector.ReplaceFields(testedJson);
        Log.Information("After replacement: {json}", testedJson.ToJsonString());

        // Assert
        Assert.That(testedJson.ToJsonString(), Is.EqualTo(expectedJson.ToJsonString()));
    }

    [Test]
    public void TestFieldInjector_ConstructInjectorAndCallReplaceFieldsOnJsonNodeWithMissingStaticValues_ShouldThrowAnException()
    {
        // Arrange
        var generationsEnumerableDictionary = new Dictionary<string, GenerationEnumerable>();
        
        var jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new ()
            {
                Path = "$.salary",
                ValueType = InjectionValueType.Double,
                String = new ManualValue<string> { Value = "NotTheRightValueOfType" }
            },
        };
        var injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        var testedJson = JsonNode.DeepClone();
        
        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => injector.ReplaceFields(testedJson));
    }
    
    [Test]
    public void TestFieldInjector_ConstructInjectorAndCallReplaceFieldsOnJsonNodeWithMissingGeneration_ShouldThrowAnException()
    {
        // Arrange
        var generationsEnumerableDictionary = new Dictionary<string, GenerationEnumerable>();
        
        var jsonFieldReplacements = new List<JsonFieldReplacement>
        {
            new ()
            {
                Path = "$.salary",
                ValueType = InjectionValueType.FromDataSource,
                FromDataSource = new FromDataSource { Name = "NotTheRightGenerationName" }
            },
        };
        var injector = new JsonNodeFieldInjector(generationsEnumerableDictionary, jsonFieldReplacements);
        var testedJson = JsonNode.DeepClone();
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => injector.ReplaceFields(testedJson));
    }
}
