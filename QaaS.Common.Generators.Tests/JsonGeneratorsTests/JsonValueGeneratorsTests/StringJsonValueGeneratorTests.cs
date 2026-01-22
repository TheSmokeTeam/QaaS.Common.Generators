using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;
using QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;
using Serilog;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonValueGeneratorsTests;

public class StringJsonValueGeneratorTests
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
        new TestCaseData(null, null).SetName("WithoutAnySchemaProperties"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["minLength"] = 10,
            ["maxLength"] = 15,
        }, null).SetName("WithMinLengthAndMaxLength"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["minLength"] = 10,
        }, null).SetName("WithMinLength"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["maxLength"] = 15,
        }, null).SetName("WithMaxLength"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["format"] = "uuid"
        }, "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$").SetName("WithUuidFormat"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["format"] = "uuid",
            ["Placeholder"] = "praise god"
        }, "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$").SetName("WithUuidFormatWithExtraIrrelevantProperties"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["format"] = "email"
        }, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$").SetName("WithEmailFormat"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["format"] = "ipv4"
        }, @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$").SetName("WithIpv4Format"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["format"] = "ipv6"
        }, @"^(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))$").SetName("WithIpv6Format"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["format"] = "hostname"
        }, @"^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)*[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?$").SetName("WithHostnameFormat"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["format"] = "time"
        }, @"^(?:[01]\d|2[0-3]):[0-5]\d:[0-5]\d(?:\.\d+)?\+00:00$").SetName("WithTimeFormat"),
        new TestCaseData(new Dictionary<string, object>
        {
            ["format"] = "date"
        }, @"^[0-9]{4}-(0[1-9]|1[0-2])-(0[1-9]|[1-2][0-9]|3[01])$").SetName("WithDateFormat"),
        new TestCaseData(new Dictionary<string, object>
        {
        ["format"] = "date-time"
        }, @"^(?:[1-9]\d{3}-(?:(?:0[1-9]|1[0-2])-(?:0[1-9]|1\d|2\d|3[01])|W(?:0[1-9]|[1-4]\d|5[0-3])(?:-[1-7])?)T(?:[01]\d|2[0-3]):[0-5]\d:[0-5]\d(?:\.\d+)?(?:\+00:00|Z))$").SetName("WithDateTimeFormat"),
    };

    [Test, TestCaseSource(nameof(_generateValueCaseData))]
    public void TestGenerateValue_CallGeneratorClassWithConfigurationProperties_GeneratedValueShouldFollowSchemaPropertiesAndRegexFormat
        (Dictionary<string, object>? schemaParameters, string? regexFormat = null)
    { 
        // Arrange
        schemaParameters ??= new Dictionary<string, object>();
        var jsonSchemaObject = JsonNode.Parse(JsonSerializer.Serialize(schemaParameters)) as JsonObject;
        var generator = new StringJsonValueGenerator(new Random().Next());
        
        var minLength = (int) (jsonSchemaObject!.GetJsonSchemaStringMinLength() ?? 
                               StringJsonValueGenerator.DefaultStringMinimumLength);
        var maxLength = (int) (jsonSchemaObject!.GetJsonSchemaStringMaxLength() ?? 
                               StringJsonValueGenerator.DefaultStringMaximumLength);
        
        // Act
        var generatedJsonValue = generator.GenerateValue(jsonSchemaObject!, Globals.rootPath);
        Log.Logger.Information("Generated value: {GeneratedValue}", generatedJsonValue);

        // Assert
        var value = generatedJsonValue.GetValue<string>();
         if (regexFormat != null) Assert.That(Regex.IsMatch(value, regexFormat), Is.True);
        else
        {
            Assert.IsTrue(value.Length >= minLength);
            Assert.IsTrue(value.Length <= maxLength);
        }
    }
    
    [Test]
    public void TestGenerateValue_CallGeneratorClassWithUnknownFormatType_OutputShouldBeAnException()
    { 
        // Arrange
        var schemaParameters = new Dictionary<string, object>
        {
            ["format"] = "Placeholder format",
            ["ExtraIrrelevant"] = "Property"
        };
        var jsonSchemaObject = JsonNode.Parse(JsonSerializer.Serialize(schemaParameters)) as JsonObject;
        var generator = new StringJsonValueGenerator(new Random().Next());
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => generator.GenerateValue(jsonSchemaObject!, Globals.rootPath));
    }
}