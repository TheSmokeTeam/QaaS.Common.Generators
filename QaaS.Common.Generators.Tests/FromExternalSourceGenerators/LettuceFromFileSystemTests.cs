using System.Reflection;
using System.Text;
using NUnit.Framework;
using QaaS.Common.Generators.FromExternalSourceGenerators;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Generators.Tests.FromExternalSourceGenerators;

public class LettuceFromFileSystemTests
{
    private readonly LettuceFromFileSystem _generatorInstance = new();
    private readonly MethodInfo _processFileContentsMethod = typeof(LettuceFromFileSystem).GetMethod(
        "ProcessFileContents", BindingFlags.NonPublic | BindingFlags.Instance)!;
    
    public static IEnumerable<TestCaseData> TestGenerateCaseSource()
    {
        var encodedBody = Convert.ToBase64String("Test1"u8.ToArray());
        var routingKey = "test";
        var jsonContent1 = $"{{ \"Body\": \"{encodedBody}\",  \"RoutingKey\": \"{routingKey}\" }}";
        const string lettuceContent1 = "Test1";
        yield return new TestCaseData(jsonContent1, lettuceContent1, routingKey).SetName("SingleJsonWithContentAndKey");

        const string jsonContent2 = "{ \"Gyat\": \"Test1\" }";
        const string lettuceContent2 = "";
        yield return new TestCaseData(jsonContent2, lettuceContent2, null).SetName("NotLettuceFormat");
    }

    [Test, TestCaseSource(nameof(TestGenerateCaseSource))]
    public void TestGenerate_CallFunctionWithBuiltFileSystemAndConfiguration_ShouldReturnExpectedOutput(
        string jsonString, string lettuceBody, string? routingKey)
    {
        // Arrange +  Act
        const string expiration = "someExpiration";
        var arguments = new object[]
        {
            Encoding.UTF8.GetBytes(jsonString), "",  new MetaData
            {
                RabbitMq = new RabbitMq
                {
                    Expiration = expiration
                }
            }
        };
        var result = (byte[])_processFileContentsMethod.Invoke(_generatorInstance, arguments)!;

        // Assert
        var outputString = Encoding.UTF8.GetString(result);
        Assert.AreEqual(outputString, lettuceBody);
        Assert.AreEqual(routingKey, ((MetaData)arguments[2]).RabbitMq?.RoutingKey);
        Assert.AreEqual(expiration, ((MetaData)arguments[2]).RabbitMq?.Expiration);
    }
    
    public static IEnumerable<TestCaseData> TestGenerateInvalidContents()
    {
        var content1 = "invalid"u8.ToArray();
        var expectedExceptionType1 = "JsonReaderException";
        yield return new TestCaseData(content1, expectedExceptionType1).SetName("NotJson");
        var content2 = "{ \"Body\": \"Test1\" }"u8.ToArray();
        var expectedExceptionType2 = "FormatException";
        yield return new TestCaseData(content2, expectedExceptionType2).SetName("NotBase64");
    }
    
    [Test, TestCaseSource(nameof(TestGenerateInvalidContents))]
    public void TestGenerate_CallFunctionWithFileThatIsNotLettuce_ShouldRaiseException(byte[] fileContent, string exceptionType)
    {
        // Arrange
        var actualMessage = "";
        ArgumentException? argumentException = null;
        
        // Act
        try
        {
            _ = _processFileContentsMethod.Invoke(_generatorInstance,
                new object[] { fileContent, "name", new MetaData() })!;
        }
        catch (TargetInvocationException exception)
        {
            actualMessage = exception.InnerException!.Message;
            argumentException = (exception.InnerException as ArgumentException)!;
        }
        
        // Assert
        Assert.That(argumentException, Is.Not.Null);
        Assert.That(actualMessage, Does.Contain(exceptionType));
    }
}
