using System.Text.Json.Nodes;
using DeepEqual.Syntax;
using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonExtensionsTests;

public class JsonNodeSchemaExtensionsTests
{
    [Test]
    public void TestGetPropertyOfType_CallMethodOnJsonSchemaObjectWithOneType_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["type"] = "object" };
        var expectedTypes = new List<string> { "object" };
        
        // Act
        var type = jsonSchemaObject.GetJsonSchemaTypes();
        
        // Assert
        type.ShouldDeepEqual(expectedTypes);
    }

    [Test]
    public void TestGetPropertyOfType_CallMethodOnJsonSchemaObjectWithMultipleTypes_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["type"] = new JsonArray("type1", "type2") };
        var expectedTypes = new List<string> { "type1", "type2" };
        
        // Act
        var type = jsonSchemaObject.GetJsonSchemaTypes();
        
        // Assert
        type.ShouldDeepEqual(expectedTypes);
    }
    
    [Test]
    public void TestGetPropertyOfType_CallMethodOnJsonSchemaObjectWithMultipleDifferentTypes_ShouldThrowAnException()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["type"] = new JsonArray {
            "type1", new JsonObject { ["type"] = "type2" }
        }};
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => jsonSchemaObject.GetJsonSchemaTypes());
    }
    
    [Test]
    public void TestGetPropertyOfConst_CallMethodOnJsonSchemaObjectWithConstValue_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["const"] = "placeholder" };
        var expectedConst = "placeholder";
        
        // Act
        var retrievedConst = jsonSchemaObject.GetJsonSchemaConst() as JsonValue;
        var valueOfConst = retrievedConst!.GetValue<string>();
        
        // Assert
        valueOfConst.ShouldDeepEqual(expectedConst);
    }
    
    [Test]
    public void TestGetPropertyOfConst_CallMethodOnJsonSchemaObjectWithConstObject_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["const"] = new JsonArray("placeholder", "Values")};
        var expectedConst = new JsonArray("placeholder", "Values");
        
        // Act
        var retrievedConst = jsonSchemaObject.GetJsonSchemaConst();
        
        // Assert
        JsonNode.DeepEquals(retrievedConst, expectedConst);
    }
    
    [Test]
    public void TestGetPropertyOfConst_CallMethodOnJsonSchemaObjectWithoutConst_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedConst = jsonSchemaObject.GetJsonSchemaConst();
        
        // Assert
        Assert.That(retrievedConst, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfEnum_CallMethodOnJsonSchemaObjectWithEnumArray_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["enum"] = new JsonArray("placeholder", "Values")};
        var expectedEnum = new JsonArray("placeholder", "Values");
        
        // Act
        var retrievedEnum = jsonSchemaObject.GetJsonSchemaEnum();
        
        // Assert
        JsonNode.DeepEquals(retrievedEnum, expectedEnum);
    }
    
    [Test]
    public void TestGetPropertyOfEnum_CallMethodOnJsonSchemaObjectWithoutEnum_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedEnum = jsonSchemaObject.GetJsonSchemaEnum();
        
        // Assert
        Assert.That(retrievedEnum, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfRef_CallMethodOnJsonSchemaObjectWithRef_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["$ref"] = "value" };
        var expectedRef = "value";
        
        // Act
        var retrievedRef = jsonSchemaObject.GetJsonSchemaRef();
        
        // Assert
        retrievedRef.ShouldDeepEqual(expectedRef);
    }
    
    [Test]
    public void TestGetPropertyOfRef_CallMethodOnJsonSchemaObjectWithoutRef_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedRef = jsonSchemaObject.GetJsonSchemaRef();
        
        // Assert
        Assert.That(retrievedRef, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfId_CallMethodOnJsonSchemaObjectWithId_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["id"] = "value" };
        var expectedValue = "value";
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaId();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfId_CallMethodOnJsonSchemaObjectWithoutId_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaId();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    
    [Test]
    public void TestGetPropertyOfDefinitions_CallMethodOnJsonSchemaObjectWithDefinitions_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["definitions"] = new JsonObject
        {
            ["testDefinition"] = new JsonObject { ["type"] = "object" },
            ["testDefinition2"] = new JsonObject { ["type"] = "array" },
        }};
        var expectedValue = new Dictionary<string, JsonObject>
        {
            ["#/definitions/testDefinition"] = new () { ["type"] = "object" },
            ["#/definitions/testDefinition2"] = new () { ["type"] = "array" }
        };

        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaDefinitions();
        
        
        // Assert
        retrievedValue.Keys.ShouldDeepEqual(expectedValue.Keys);
        for (var definitionIndex = 0; definitionIndex < retrievedValue.Values.Count; definitionIndex++)
        {
            JsonNode.DeepEquals(retrievedValue.Values.ElementAt(definitionIndex), 
                expectedValue.Values.ElementAt(definitionIndex));
        }         
    }
    
    
    [Test]
    public void TestGetPropertyOfDefinitions_CallMethodOnJsonSchemaObjectWithEmptyDefinitions_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["definitions"] = new JsonObject{}};
        var expectedValue = new Dictionary<string, JsonObject>{};

        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaDefinitions();
        
        
        // Assert
        retrievedValue.Keys.ShouldDeepEqual(expectedValue.Keys);
        for (var definitionIndex = 0; definitionIndex < retrievedValue.Values.Count; definitionIndex++)
        {
            JsonNode.DeepEquals(retrievedValue.Values.ElementAt(definitionIndex), 
                expectedValue.Values.ElementAt(definitionIndex));
        }         
    }
    
    
    [Test]
    public void TestGetPropertyOfDefinitions_CallMethodOnJsonSchemaObjectWithoutDefinitions_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        var expectedValue = new Dictionary<string, JsonObject>{};

        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaDefinitions();
        
        
        // Assert
        retrievedValue.Keys.ShouldDeepEqual(expectedValue.Keys);
        for (var definitionIndex = 0; definitionIndex < retrievedValue.Values.Count; definitionIndex++)
        {
            JsonNode.DeepEquals(retrievedValue.Values.ElementAt(definitionIndex), 
                expectedValue.Values.ElementAt(definitionIndex));
        }         
    }
    
    [Test]
    public void TestGetPropertyOfMinLength_CallMethodOnJsonSchemaObjectWithMinLength_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["minLength"] = 69 };
        uint expectedValue = 69;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaStringMinLength();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMinLength_CallMethodOnJsonSchemaObjectWithoutMinLength_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaStringMinLength();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfMaxLength_CallMethodOnJsonSchemaObjectWithMaxLength_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["maxLength"] = 69 };
        uint expectedValue = 69;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaStringMaxLength();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMaxLength_CallMethodOnJsonSchemaObjectWithoutMaxLength_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaStringMaxLength();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfFormat_CallMethodOnJsonSchemaObjectWithFormat_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["format"] = "test-format" };
        var expectedValue = "test-format";
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaStringFormat();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfFormat_CallMethodOnJsonSchemaObjectWithoutFormat_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaStringFormat();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfMinimum_CallMethodOnJsonSchemaObjectWithMinimumInteger_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["minimum"] = 69 };
        var expectedValue = 69.0;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberMinimum();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMinimum_CallMethodOnJsonSchemaObjectWithMinimumFloat_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["minimum"] = 69.420 };
        var expectedValue = 69.420;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberMinimum();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMinimum_CallMethodOnJsonSchemaObjectWithoutMinimum_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberMinimum();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfMaximum_CallMethodOnJsonSchemaObjectWithMaximumInteger_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["maximum"] = 69 };
        var expectedValue = 69.0;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberMaximum();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMaximum_CallMethodOnJsonSchemaObjectWithMaximumFloat_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["maximum"] = 69.420 };
        var expectedValue = 69.420;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberMaximum();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMaximum_CallMethodOnJsonSchemaObjectWithoutMaximum_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberMaximum();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfMultipleOf_CallMethodOnJsonSchemaObjectWithMultipleOfInteger_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["multipleOf"] = 69 };
        var expectedValue = 69.0;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberMultipleOf();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMultipleOf_CallMethodOnJsonSchemaObjectWithMultipleOfFloat_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["multipleOf"] = 69.420 };
        var expectedValue = 69.420;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberMultipleOf();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMultipleOf_CallMethodOnJsonSchemaObjectWithoutMultipleOf_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberMultipleOf();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfExclusiveMinimum_CallMethodOnJsonSchemaObjectWithExclusiveMinimum_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["exclusiveMinimum"] = true };
        var expectedValue = true;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberExclusiveMinimum();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfExclusiveMinimum_CallMethodOnJsonSchemaObjectWithoutExclusiveMinimum_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberExclusiveMinimum();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfExclusiveMaximum_CallMethodOnJsonSchemaObjectWithExclusiveMaximum_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["exclusiveMaximum"] = false };
        var expectedValue = false;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberExclusiveMaximum();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfExclusiveMaximum_CallMethodOnJsonSchemaObjectWithoutExclusiveMaximum_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaNumberExclusiveMaximum();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfUniqueItems_CallMethodOnJsonSchemaObjectWithUniqueItems_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["uniqueItems"] = true };
        var expectedValue = true;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaArrayUniqueItems();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfUniqueItems_CallMethodOnJsonSchemaObjectWithoutUniqueItems_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaArrayUniqueItems();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfMinItems_CallMethodOnJsonSchemaObjectWithMinItems_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["minItems"] = 69 };
        uint expectedValue = 69;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaArrayMinItems();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMinItems_CallMethodOnJsonSchemaObjectWithoutMinItems_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaArrayMinItems();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfMaxItems_CallMethodOnJsonSchemaObjectWithMaxItems_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["maxItems"] = 69 };
        uint expectedValue = 69;
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaArrayMaxItems();
        
        // Assert
        retrievedValue.ShouldDeepEqual(expectedValue);
    }
    
    [Test]
    public void TestGetPropertyOfMaxItems_CallMethodOnJsonSchemaObjectWithoutMaxItems_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject {};
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaArrayMaxItems();
        
        // Assert
        Assert.That(retrievedValue, Is.Null);
    }
    
    [Test]
    public void TestGetPropertyOfItems_CallMethodOnJsonSchemaObjectWithoutItems_ShouldThrowAnException()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { };
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => jsonSchemaObject.GetJsonSchemaArrayItems());
    }
    
    [Test]
    public void TestGetPropertyOfItems_CallMethodOnJsonSchemaObjectWithItemsOfWrongType_ShouldThrowAnException()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["items"] = "wrong-type"};
        
        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => jsonSchemaObject.GetJsonSchemaArrayItems());
    }
    
    [Test]
    public void TestGetPropertyOfItems_CallMethodOnJsonSchemaObjectWithItemsWhichIncludesSomethingDifferentThanObject_ShouldThrowAnException()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["items"] = new JsonArray {
            new JsonObject { ["type"] = "number"},
            420
        }};
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => jsonSchemaObject.GetJsonSchemaArrayItems());
    }
    
    [Test]
    public void TestGetPropertyOfItems_CallMethodOnJsonSchemaObjectWithItemsWithOneItem_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["items"] = new JsonObject { ["type"] = "number"} };
        var expectedValue = new List<JsonObject> {
            new () { ["type"] = "number"}
        };
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaArrayItems();
        
        // Assert
        for (var definitionIndex = 0; definitionIndex < retrievedValue.Count; definitionIndex++)
        {
            JsonNode.DeepEquals(retrievedValue.ElementAt(definitionIndex), expectedValue.ElementAt(definitionIndex));
        }   
    }
    
    [Test]
    public void TestGetPropertyOfItems_CallMethodOnJsonSchemaObjectWithItemsWithMultipleItems_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["items"] = new JsonArray {
            new JsonObject { ["type"] = "number"},
            new JsonObject { ["type"] = "string"}
        }};
        
        var expectedValue = new List<JsonObject> {
            new () { ["type"] = "number"},
            new () { ["type"] = "string"}
        };
        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaArrayItems();
        
        // Assert
        for (var definitionIndex = 0; definitionIndex < retrievedValue.Count; definitionIndex++)
        {
            JsonNode.DeepEquals(retrievedValue.ElementAt(definitionIndex), expectedValue.ElementAt(definitionIndex));
        }    
    }
    
    [Test]
    public void TestGetPropertyOfProperties_CallMethodOnJsonSchemaObjectWithoutProperties_ShouldThrowAnException()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { };
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => jsonSchemaObject.GetJsonSchemaObjectProperties());
    }
    
    [Test]
    public void TestGetPropertyOfProperties_CallMethodOnJsonSchemaObjectWithPropertiesOfWrongType_ShouldThrowAnException()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["properties"] = "wrong-type"};
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => jsonSchemaObject.GetJsonSchemaObjectProperties());
    }
    
    [Test]
    public void TestGetPropertyOfProperties_CallMethodOnJsonSchemaObjectWithPropertiesWhichIncludesSomethingDifferentThanObject_ShouldThrowAnException()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["properties"] = new JsonObject
        {
            ["testPropertyOne"] = new JsonObject { ["type"] = "number"},
            ["testPropertyTwo"] = 420
        }};
        
        // Act + Assert
        Assert.Throws<ArgumentException>(() => jsonSchemaObject.GetJsonSchemaObjectProperties());
    }
    
    [Test]
    public void TestGetPropertyOfProperties_CallMethodOnJsonSchemaObjectWithProperties_ShouldReturnRelevantValue()
    {
        // Arrange
        var jsonSchemaObject = new JsonObject { ["properties"] = new JsonObject
        {
            ["testPropertyOne"] = new JsonObject { ["type"] = "number"},
            ["testPropertyTwo"] = new JsonObject { ["type"] = "string"},
        }};
        
        
        var expectedValue = new Dictionary<string, JsonObject>
        {
            ["testPropertyOne"] = new () { ["type"] = "number" },
            ["testPropertyTwo"] = new () { ["type"] = "string" }
        };

        
        // Act
        var retrievedValue = jsonSchemaObject.GetJsonSchemaObjectProperties();
        
        
        // Assert
        retrievedValue.Keys.ShouldDeepEqual(expectedValue.Keys);
        for (var definitionIndex = 0; definitionIndex < retrievedValue.Values.Count; definitionIndex++)
        {
            JsonNode.DeepEquals(retrievedValue.Values.ElementAt(definitionIndex), 
                expectedValue.Values.ElementAt(definitionIndex));
        }
    }
}