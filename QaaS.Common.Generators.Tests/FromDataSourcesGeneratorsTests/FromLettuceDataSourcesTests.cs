using System.Reflection;
using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Generators.FromDataSourcesGenerators;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Generators.Tests.FromDataSourcesGeneratorsTests;

public class FromLettuceDataSourcesTests
{
    private static readonly MethodInfo? ConvertDataSourceDataToGenerateDataMethod = 
        typeof(FromLettuceDataSources).GetMethod("ConvertDataSourceDataToGenerateData", 
            BindingFlags.Instance | BindingFlags.NonPublic);

    private static IEnumerable<TestCaseData> TestConvertDataSourceToGenerateDataCaseSource()
    {
        yield return new TestCaseData(Array.Empty<byte>(), null).SetName("EmptyLettuceBodyWithNullRoutingKey");
        yield return new TestCaseData(Array.Empty<byte>(), "not null routing key").SetName("EmptyLettuceBodyWithRoutingKey");
        yield return new TestCaseData(new byte[]{1,2,3,4,5}, null).SetName("LettuceBodyWithNullRoutingKey");
        yield return new TestCaseData(new byte[]{1,2,3,4,5}, "not null routing key").SetName("LettuceBodyWithRoutingKey");
    }
    
    [Test, TestCaseSource(nameof(TestConvertDataSourceToGenerateDataCaseSource))]
    public void TestConvertDataSourceDataToGenerateData_CallFunctionWithKnownLettuceJson_ShouldReturnExpectedData(
        byte[] lettuceBody, string? lettuceRoutingKey)
    {
        // Arrange
        var lettuce = new JsonObject
        {
            { "Body", Convert.ToBase64String(lettuceBody) },
            { "RoutingKey", lettuceRoutingKey }
        };
        var generator = new FromLettuceDataSources
        {
            Context = Globals.Context
        };
        var data = new Data<object>()
        {
            Body = lettuce
        };
        
        // Act
        var generatedData = (Data<object>)ConvertDataSourceDataToGenerateDataMethod!.Invoke(generator, 
            new object?[] { data, "testSource"})!;
        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(generatedData.Body, Is.EqualTo(lettuceBody));
            Assert.That(generatedData.MetaData?.RabbitMq?.RoutingKey, Is.EqualTo(lettuceRoutingKey));
        });
    }
}