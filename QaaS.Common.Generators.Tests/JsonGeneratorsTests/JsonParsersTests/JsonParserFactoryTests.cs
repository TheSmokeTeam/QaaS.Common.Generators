using System.Reflection;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators.JsonParsers;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonParsersTests;

public class JsonParserFactoryTests
{

    private static IEnumerable<TestCaseData> _buildParserCaseData = new[]
    {
        new TestCaseData(JsonParserType.Xml, null, typeof(JsonParserToXml)),
        new TestCaseData(JsonParserType.Binary, new SpecificTypeConfig
        {
            TypeFullName = typeof(Person).FullName,
            AssemblyName = Assembly.GetExecutingAssembly().GetName().Name
        }, typeof(JsonParserToBinary)),
        new TestCaseData(JsonParserType.ProtobufMessage, new SpecificTypeConfig
        {
            TypeFullName = typeof(PersonTestNamespace.Person).FullName,
            AssemblyName = Assembly.GetExecutingAssembly().GetName().Name
        }, typeof(JsonParserToProtobufMessage)),
    };
  

    [Test, TestCaseSource(nameof(_buildParserCaseData))]
    public void 
        TestBuildParser_CallFactoryBuildMethodWithEnum_OutputTypeShouldBeEqualToExpectedType
        (JsonParserType jsonParserType, SpecificTypeConfig? specificTypeConfig, Type expectedType)
    {
        // Arrange
        var factory = JsonParserFactory.GetInstance();
        
        // Act
        var factoryOutput = factory.GetJsonParser(jsonParserType, specificTypeConfig);
        
        // Assert
        Assert.That(factoryOutput.GetType(), Is.EqualTo(expectedType));
    }

    [Test]
    public void 
        TestBuildParser_CallFactoryBuildMethodWithUndefinedEnum_OutputShouldBeAnException()
    {
        // Arrange
        var factory = JsonParserFactory.GetInstance();
        var jsonParserType = JsonParserType.Json;
        var specificTypeConfig = new SpecificTypeConfig { TypeFullName = "Placeholder", AssemblyName = "Placeholder" };
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => factory.GetJsonParser(jsonParserType, specificTypeConfig));
    }
}