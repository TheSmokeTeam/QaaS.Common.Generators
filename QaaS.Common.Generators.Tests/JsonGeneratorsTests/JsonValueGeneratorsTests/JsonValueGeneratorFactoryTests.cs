using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;
using QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonValueGeneratorsTests;

public class JsonValueGeneratorFactoryTests
{
    private const int Seed = 69; 

    private static IEnumerable<TestCaseData> _buildGeneratorCaseData = new[]
    {
        new TestCaseData(JsonNodeSchemaExtensions.String, typeof(StringJsonValueGenerator)),
        new TestCaseData(JsonNodeSchemaExtensions.Number, typeof(NumberJsonValueGenerator)),
        new TestCaseData(JsonNodeSchemaExtensions.Integer, typeof(IntegerJsonValueGenerator)),
        new TestCaseData(JsonNodeSchemaExtensions.Boolean, typeof(BooleanJsonValueGenerator)),
        new TestCaseData(JsonNodeSchemaExtensions.Null, typeof(NullJsonValueGenerator))
    };
  

    [Test, TestCaseSource(nameof(_buildGeneratorCaseData))]
    public void 
        TestBuildGenerator_CallFactoryBuildMethodWithDefinedString_OutputTypeShouldBeEqualToExpectedType
        (string jsonValueGeneratorType, Type expectedType)
    {
        // Arrange
        var factory = JsonValueGeneratorFactory.GetInstance();
        
        // Act
        var factoryOutput = factory.GetJsonValueGenerator(jsonValueGeneratorType, Seed);
        
        // Assert
        Assert.That(factoryOutput.GetType(), Is.EqualTo(expectedType));
    }

    [Test]
    public void 
        TestBuildGenerator_CallFactoryBuildMethodWithUndefinedKey_OutputShouldBeAnException()
    {
        // Arrange
        var factory = JsonValueGeneratorFactory.GetInstance();
        var jsonValueGeneratorType = "PlaceholderName";
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => factory.GetJsonValueGenerator(jsonValueGeneratorType, Seed));
    }
}