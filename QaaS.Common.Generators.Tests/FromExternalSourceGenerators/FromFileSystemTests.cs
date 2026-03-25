using System.Collections;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;
using QaaS.Common.Generators.FromExternalSourceGenerators;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;


namespace QaaS.Common.Generators.Tests.FromExternalSourceGenerators;

public class FromFileSystemTests
{
    private static readonly string TestsDirectoryPath = Path.Join("TestData");
    private static readonly string DirectoryRelativePath = Path.Join("TextFiles");

    public static IEnumerable<TestCaseData> TestGenerateCaseSource()
    {
        var config1 = new FromFileSystemConfig
        {
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            FileSystem = new FileSystemConfig
            {
                Path = DirectoryRelativePath
            }
        };
        var allTestDataObjects = new List<Data<object>>()
        {
            new() { Body = "notTest"u8.ToArray() },
            new() { Body = "test1"u8.ToArray() },
            new() { Body = "test2"u8.ToArray() },
            new() { Body = "test3"u8.ToArray() },
            new() { Body = "test4"u8.ToArray() },
        };
        yield return new TestCaseData(config1, allTestDataObjects).SetName("TakeAllFilesFromDirectory");
        var config2 = new FromFileSystemConfig
        {
            FileSystem = new FileSystemConfig
            {
                Path = DirectoryRelativePath
            },
            Count = 2,
            DataArrangeOrder = DataArrangeOrder.AsciiAsc
        };
        var twoTestDataObjects = new List<Data<object>>()
        {
            new() { Body = "notTest"u8.ToArray() },
            new() { Body = "test1"u8.ToArray() }
        };
        yield return new TestCaseData(config2, twoTestDataObjects).SetName("TakeOnlyTwoFilesFromDirectory");
        var config3 = new FromFileSystemConfig
        {
            FileSystem = new FileSystemConfig
            {
                Path = DirectoryRelativePath
            },
            DataUuidRegexExpression = "test.*",
            DataArrangeOrder = DataArrangeOrder.AsciiAsc
        };
        var fourTestDataObjects = new List<Data<object>>()
        {
            new() { Body = "test1"u8.ToArray() },
            new() { Body = "test2"u8.ToArray() },
            new() { Body = "test3"u8.ToArray() },
            new() { Body = "test4"u8.ToArray() }
        };
        yield return new TestCaseData(config3, fourTestDataObjects).SetName(
            "TakeOnlyFilesStartingWithTestFromDirectory");
    }

    [Test, TestCaseSource(nameof(TestGenerateCaseSource))]
    public void TestGenerate_CallFunctionWithBuiltFileSystemAndConfiguration_ShouldReturnExpectedOutput(
        FromFileSystemConfig config, IEnumerable<Data<object>> expectedOutput)
    {
        // Arrange
        var mockGenerator = new Mock<FromFileSystem>();
        mockGenerator.Protected().Setup<KeyValuePair<string, IFileSystem>>("BuildFileSystem")
            .Returns(new KeyValuePair<string, IFileSystem>(TestsDirectoryPath, new FileSystem()));
        mockGenerator.CallBase = true;
        var generator = mockGenerator.Object;
        generator.Configuration = config;
        generator.Context = Globals.Context;

        // Act
        var output = generator.Generate(new List<SessionData>().ToImmutableList(),
            new List<DataSource>().ToImmutableList()).ToList();

        // Assert
        var outputList = expectedOutput.ToList();
        if (outputList.Count != output.Count)
        {
            Assert.Fail($"Output count ({output.Count}) not equal to expected count ({outputList.Count})");
            return;
        }

        var areEqual = true;
        for (var dataIndex = 0; dataIndex < output.Count; dataIndex++)
        {
            var expectedData = outputList[dataIndex];
            var data = output[dataIndex];
            if (StructuralComparisons.StructuralEqualityComparer.Equals(expectedData.Body, data.Body)) continue;
            Globals.Logger.LogWarning(
                "data at index {DataIndex} not equal between output and expected output", dataIndex);
            areEqual = false;
        }

        Assert.That(areEqual);
    }

    [Test,
     TestCase(2), TestCase(3), TestCase(4), TestCase(5)]
    public void
        TestGenerateLazily_CallFunctionWithBuiltFileSystemAndConfiguration_ShouldLoadDataLazily(
            int numberOfItemsToGenerate)
    {
        // Arrange
        var mockGenerator = new Mock<FromFileSystem>();
        mockGenerator.Protected().Setup<KeyValuePair<string, IFileSystem>>("BuildFileSystem")
            .Returns(new KeyValuePair<string, IFileSystem>(TestsDirectoryPath, new FileSystem()));
        mockGenerator.CallBase = true;
        var generator = mockGenerator.Object;
        generator.Configuration = new FromFileSystemConfig
        {
            FileSystem = new FileSystemConfig
            {
                Path = DirectoryRelativePath
            },
            Count = numberOfItemsToGenerate,
            DataArrangeOrder = DataArrangeOrder.Unordered
        };
        generator.Context = Globals.Context;
        mockGenerator.Protected().Setup<SerializedLoadedData>("LoadData", ItExpr.IsAny<KeyValuePair<string, string>>())
            .Returns(new SerializedLoadedData
            {
                FullKey = "test",
                Content = [0],
                MetaData = null
            });

        // Verify that LoadData is not called during setup
        mockGenerator.Protected().Verify("LoadData", Times.Never(), ItExpr.IsAny<KeyValuePair<string, string>>());

        // Act + Assert
        var (testResults, allPassed, itemCount, sideEffectCounter) =
            Globals.RunLazinessTest<FromFileSystemConfig, FromFileSystem>(
                generator,
                generator.Generate,
                numberOfItemsToGenerate,
                _ =>
                {
                    // No additional setup needed for this test
                });

        Globals.AssertLazinessProperties(testResults, itemCount, sideEffectCounter, numberOfItemsToGenerate, allPassed);
    }

    [Test,
     TestCase(2), TestCase(3), TestCase(4), TestCase(5)]
    public void TestLoadObjectsMetadata_CallFunctionWithBuiltFileSystemAndConfiguration_ShouldLoadDataLazily(int numberOfItemsToGenerate)
    {
        // Arrange
        var mockGenerator = new Mock<FromFileSystem>();
        mockGenerator.Protected().Setup<KeyValuePair<string, IFileSystem>>("BuildFileSystem")
            .Returns(new KeyValuePair<string, IFileSystem>(TestsDirectoryPath, new FileSystem()));
        mockGenerator.CallBase = true;
        var generator = mockGenerator.Object;
        generator.Configuration = new FromFileSystemConfig
        {
            FileSystem = new FileSystemConfig
            {
                Path = DirectoryRelativePath
            },
            Count = numberOfItemsToGenerate,
            DataArrangeOrder = DataArrangeOrder.Unordered
        };
        generator.Context = Globals.Context;
        mockGenerator.Protected().Setup<SerializedLoadedData>("LoadData", ItExpr.IsAny<KeyValuePair<string, string>>())
            .Returns(new SerializedLoadedData
            {
                FullKey = "test",
                Content = [0],
                MetaData = null
            });

        // Verify that LoadData is not called during setup
        mockGenerator.Protected().Verify("LoadData", Times.Never(), ItExpr.IsAny<KeyValuePair<string, string>>());

        // Act & Assert
        var (testResults, allPassed, itemCount, sideEffectCounter) =
            Globals.RunLazinessTest<FromFileSystemConfig, FromFileSystem>(
                generator,
                generator.Generate,
                numberOfItemsToGenerate,
                _ =>
                {
                    // No additional setup needed for this test
                });

        Globals.AssertLazinessProperties(testResults, itemCount, sideEffectCounter, numberOfItemsToGenerate, allPassed);

        // Additional verification: Test the LoadObjectsMetadata method directly using reflection
        // This ensures that the lazy evaluation works for the metadata loading itself
        var loadObjectsMetadataMethod = typeof(FromFileSystem).GetMethod("LoadObjectsMetadata",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.That(loadObjectsMetadataMethod, Is.Not.Null);
        var metadataResult = loadObjectsMetadataMethod.Invoke(generator, null) as
            IEnumerable<KeyValuePair<string, string>>;
        var metadataEntries = metadataResult?.ToList();

        // Verify that the metadata loading happened correctly
        Assert.That(metadataEntries, Is.Not.Null);
        Assert.That(metadataEntries!.Count, Is.EqualTo(5));
    }
}
