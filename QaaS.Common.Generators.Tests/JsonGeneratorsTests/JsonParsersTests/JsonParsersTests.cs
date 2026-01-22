using System.Reflection;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DeepEqual.Syntax;
using NUnit.Framework;
using QaaS.Common.Generators.JsonGenerators.JsonParsers;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.Tests.JsonGeneratorsTests.JsonParsersTests;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string City { get; set; }
    public string Test { get; set; }
    public List<string> Enum { get; set; }
    public Dictionary<string, int> Floors { get; set; }
}

public class JsonParsersTests
{
    private static readonly JsonNode JsonNode = JsonNode.Parse(@"{
            ""Name"": ""John"",
            ""Age"": 30,
            ""City"": ""new york"",
            ""Test"": null,
            ""Enum"": [""REDA"", ""REDA"", ""REDA""],
            ""Floors"": {
                ""F1"": 1,
                ""F2"": 2,
                ""F3"": 3
            }
        }")!;
    
    [Test]
    public void TestBinaryParser_UseParsingMethodWithGivenJsonNode_ParsingSucceededAndObjectEqualByValue()
    {
        //  Arrange
        var expectedSerializedObject = new Person
        {
            Name = "John",
            Age = 30,
            City = "new york",
            Test = null,
            Enum = new List<string>
            {
                "REDA",
                "REDA",
                "REDA"
            },
            Floors = new Dictionary<string, int>
            {
                {"F1", 1},
                {"F2", 2},
                {"F3", 3}
            }
        };

        var jsonParser = new JsonParserToBinary(new SpecificTypeConfig
        {
            TypeFullName = typeof(Person).FullName,
            AssemblyName = Assembly.GetExecutingAssembly().GetName().Name
        });
        
        // Act
        var jsonDeserializedObject = jsonParser.Parse(JsonNode);
        
        // Assert
        jsonDeserializedObject.ShouldDeepEqual(expectedSerializedObject);
    }
    
    [Test]
    public void TestProtobufMessageParser_UseParsingMethodWithGivenJsonNode_ParsingSucceededAndObjectEqualByValue()
    {
        //  Arrange
        var expectedSerializedObject = new PersonTestNamespace.Person
        {
            Name = "John",
            Age = 30,
            City = "new york",
            Test = "",
            Enum = { "REDA", "REDA", "REDA" },
            Floors = { { "F1", 1 }, { "F2", 2 }, { "F3", 3 } }
        };
        
        var jsonParser = new JsonParserToProtobufMessage(new SpecificTypeConfig
        {
            TypeFullName = typeof(PersonTestNamespace.Person).FullName,
            AssemblyName = Assembly.GetExecutingAssembly().GetName().Name
        });
            
        // Act
        var jsonDeserializedObject = jsonParser.Parse(JsonNode);
        
        // Assert
        jsonDeserializedObject.ShouldDeepEqual(expectedSerializedObject);
    }
    
    [Test]
    public void TestProtobufMessageParser_UseParsingMethodWithGivenJsonNodeALOTofTIMES_ParsingSucceededAndObjectEqualByValue()
    {
        // Arrange
        var parsingCount = 100000;
        var protobufMessageResults = new List<PersonTestNamespace.Person>();
        
        var expectedSerializedObject = new PersonTestNamespace.Person
        {
            Name = "John",
            Age = 30,
            City = "new york",
            Test = "",
            Enum = { "REDA", "REDA", "REDA" },
            Floors = { { "F1", 1 }, { "F2", 2 }, { "F3", 3 } }
        };
        
        var jsonParser = new JsonParserToProtobufMessage(new SpecificTypeConfig
        {
            TypeFullName = typeof(PersonTestNamespace.Person).FullName,
            AssemblyName = Assembly.GetExecutingAssembly().GetName().Name
        });
            
        // Act
        var jsonDeserializedObject = jsonParser.Parse(JsonNode);
        
        // Assert
        jsonDeserializedObject.ShouldDeepEqual(expectedSerializedObject);
        
        // Act
        for (var indexCount = 0; indexCount < parsingCount; indexCount++)
        {
            protobufMessageResults.Add((jsonParser.Parse(JsonNode) as PersonTestNamespace.Person)!);
        }
        
        // Assert
        for (var indexCount = 0; indexCount < parsingCount; indexCount++)
        {
            protobufMessageResults[indexCount].ShouldDeepEqual(expectedSerializedObject);
        }
    }
    
    [Test]
    public void TestXmlParser_UseParsingMethodWithGivenJsonNode_ParsingSucceededAndObjectEqualByValue()
    {
        // Arrange
        var jsonNode = new JsonObject { { "Person", JsonNode.DeepClone() } };

        var expectedSerializedObject = new XDocument(
            new XElement("Person",
                new XElement("Name", "John"),
                new XElement("Age", 30),
                new XElement("City", "new york"),
                new XElement("Test"),
                new XElement("Enum", "REDA"),
                new XElement("Enum", "REDA"),
                new XElement("Enum", "REDA"),
                new XElement("Floors",
                    new XElement("F1", 1),
                    new XElement("F2", 2),
                    new XElement("F3", 3)
                )
            )
        );

        var jsonParser = new JsonParserToXml();
        
        // Act
        var jsonDeserializedObject = (XNode) jsonParser.Parse(jsonNode);
        
        // Assert
        Assert.That(XNode.DeepEquals(jsonDeserializedObject, expectedSerializedObject));
    }
    
    [Test]
    public void TestXmlParser_UseParsingMethodWithGivenJsonNodeALOTofTIMES_ParsingSucceededAndObjectsEqualByValue()
    {
        // Arrange
        var parsingCount = 100000;
        var xDocumentResults = new List<XNode>();
        
        var jsonNode = new JsonObject { { "Person", JsonNode.DeepClone() } };

        var expectedSerializedObject = new XDocument(
            new XElement("Person",
                new XElement("Name", "John"),
                new XElement("Age", 30),
                new XElement("City", "new york"),
                new XElement("Test"),
                new XElement("Enum", "REDA"),
                new XElement("Enum", "REDA"),
                new XElement("Enum", "REDA"),
                new XElement("Floors",
                    new XElement("F1", 1),
                    new XElement("F2", 2),
                    new XElement("F3", 3)
                )
            )
        );

        var jsonParser = new JsonParserToXml();
        
        // Act
        for (var indexCount = 0; indexCount < parsingCount; indexCount++)
        {
            xDocumentResults.Add((jsonParser.Parse(jsonNode) as XNode)!);
        }
        
        // Assert
        for (var indexCount = 0; indexCount < parsingCount; indexCount++)
        {
            Assert.That(XNode.DeepEquals(xDocumentResults[indexCount], expectedSerializedObject));
        }
    }
}