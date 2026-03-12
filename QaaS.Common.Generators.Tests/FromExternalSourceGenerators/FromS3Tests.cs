using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;
using QaaS.Common.Generators.FromExternalSourceGenerators;
using QaaS.Framework.Protocols.Utils.S3Utils;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;


namespace QaaS.Common.Generators.Tests.FromExternalSourceGenerators;

public class FromS3Tests
{
    private readonly MethodInfo _getStorageKeyFromDataMethodInfo =
        typeof(FromS3).GetMethod("GetStorageKeyFromData", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly byte[] EmptyContent = [];

    private static IEnumerable<TestCaseData> LoadMetadataFirstParityCaseSource()
    {
        const string prefix = "metadata/";
        var mixedObjects = new List<KeyValuePair<S3Object, byte[]>>
        {
            new(new S3Object
            {
                Key = $"{prefix}payload.json",
                ETag = "\"etag-json\"",
                Size = 18,
                StorageClass = "STANDARD"
            }, Encoding.UTF8.GetBytes("{\"kind\":\"json\"}")),
            new(new S3Object
            {
                Key = $"{prefix}payload.bin",
                ETag = "\"etag-bin\"",
                Size = 4,
                StorageClass = "GLACIER"
            }, [0, 1, 2, 255]),
            new(new S3Object
            {
                Key = $"{prefix}payload.txt",
                ETag = "\"etag-text\"",
                Size = 11,
                StorageClass = "STANDARD_IA"
            }, Encoding.UTF8.GetBytes("hello world"))
        };

        yield return new TestCaseData(
            new FromS3Config
            {
                S3 = new S3Config { StorageBucket = "Test", Prefix = prefix },
                DataArrangeOrder = DataArrangeOrder.AsciiAsc,
                StorageMetaData = StorageMetaData.FullPath
            },
            mixedObjects).SetName("LoadMetadataFirstParity_FullPathStorage");

        yield return new TestCaseData(
            new FromS3Config
            {
                S3 = new S3Config { StorageBucket = "Test", Prefix = prefix },
                DataArrangeOrder = DataArrangeOrder.AsciiAsc,
                StorageMetaData = StorageMetaData.RelativePath
            },
            mixedObjects).SetName("LoadMetadataFirstParity_RelativePathStorage");

        yield return new TestCaseData(
            new FromS3Config
            {
                S3 = new S3Config { StorageBucket = "Test", Prefix = prefix },
                DataArrangeOrder = DataArrangeOrder.AsciiAsc,
                StorageMetaData = StorageMetaData.ItemName
            },
            mixedObjects).SetName("LoadMetadataFirstParity_ItemNameStorage");

        yield return new TestCaseData(
            new FromS3Config
            {
                S3 = new S3Config { StorageBucket = "Test", Prefix = prefix },
                DataArrangeOrder = DataArrangeOrder.AsciiAsc,
                StorageMetaData = StorageMetaData.None
            },
            mixedObjects).SetName("LoadMetadataFirstParity_NoStorageMetadata");
    }

    public static IEnumerable<TestCaseData> TestGetStorageKeysCaseSource()
    {
        yield return new TestCaseData("S3/Prefix/Bucket/Info", "/Bucket/Info",
            StorageMetaData.RelativePath, "S3/Prefix");
        yield return new TestCaseData("S3/Prefix/Bucket/Info", "S3/Prefix/Bucket/Info", StorageMetaData.FullPath, "");
        yield return new TestCaseData("S3/Prefix/Bucket/Info", "Info", StorageMetaData.ItemName, "");
        yield return new TestCaseData("S3/Prefix/Bucket/Info", null, StorageMetaData.None, "");
    }

    private static IEnumerable<TestCaseData> TestGenerateLoadMetadataFirstTestCaseSource()
    {
        const string fakePrefix = "SomePath/Path/";
        var expectedOutputData = new Data<object> { Body = EmptyContent };
        var expectedOutputList1Data = new List<Data<object>> { expectedOutputData };
        var expectedOutputList3Data = new List<Data<object>>
            { expectedOutputData, expectedOutputData, expectedOutputData };
        var noPrefixConfig = new FromS3Config
        {
            S3 = new S3Config { StorageBucket = "Test" },
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            LoadMetadataFirst = true
        };
        yield return new TestCaseData(noPrefixConfig, new List<KeyValuePair<S3Object, byte[]>>(),
                new List<Data<object>>())
            .SetName("EmptyBucketStorageNoPrefix");
        var prefixConfig = new FromS3Config
        {
            S3 = new S3Config
            {
                StorageBucket = "Test",
                Prefix = fakePrefix
            },
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            LoadMetadataFirst = true
        };
        yield return new TestCaseData(prefixConfig, new List<KeyValuePair<S3Object, byte[]>>(),
                new List<Data<object>>())
            .SetName("EmptyBucketStorageWithPrefix");
        var mockData3 = new List<KeyValuePair<S3Object, byte[]>>
        {
            new(new S3Object { Key = "test" }, EmptyContent)
        };
        yield return new TestCaseData(noPrefixConfig, mockData3, expectedOutputList1Data
        ).SetName("OneItemInS3EmptyPrefix");
        var mockData4 = new List<KeyValuePair<S3Object, byte[]>>
        {
            new(new S3Object { Key = $"{fakePrefix}test" }, EmptyContent),
        };
        yield return new TestCaseData(prefixConfig, mockData4, expectedOutputList1Data
        ).SetName("OneItemInS3WithPrefix");
        yield return new TestCaseData(noPrefixConfig,
            new List<KeyValuePair<S3Object, byte[]>>
            {
                new(new S3Object { Key = "test" }, EmptyContent),
                new(new S3Object { Key = "test2" }, EmptyContent),
                new(new S3Object { Key = "test3" }, EmptyContent)
            }, expectedOutputList3Data).SetName("MultipleItemsInS3EmptyPrefix");
        yield return new TestCaseData(noPrefixConfig,
            new List<KeyValuePair<S3Object, byte[]>>
            {
                new(new S3Object { Key = $"{fakePrefix}test" }, EmptyContent),
                new(new S3Object { Key = $"{fakePrefix}test2" }, EmptyContent),
                new(new S3Object { Key = $"{fakePrefix}test3" }, EmptyContent)
            }, expectedOutputList3Data).SetName("MultipleItemsInS3WithPrefix");
        var countOneConfig = new FromS3Config
        {
            S3 = new S3Config { StorageBucket = "Test" },
            Count = 1,
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            LoadMetadataFirst = true
        };
        yield return new TestCaseData(countOneConfig,
            new List<KeyValuePair<S3Object, byte[]>>
            {
                new(new S3Object { Key = "test" }, EmptyContent),
                new(new S3Object { Key = "test2" }, EmptyContent),
                new(new S3Object { Key = "test3" }, EmptyContent)
            }, expectedOutputList1Data).SetName("MultipleItemsInS3OnlyOneReturnsBecauseCount");
        var regexConfig = new FromS3Config
        {
            S3 = new S3Config { StorageBucket = "Test" },
            DataUuidRegexExpression = "realtest.*",
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            LoadMetadataFirst = true
        };
        yield return new TestCaseData(regexConfig,
            new List<KeyValuePair<S3Object, byte[]>>
            {
                new(new S3Object { Key = "realtest" }, EmptyContent),
                new(new S3Object { Key = "nottest2" }, EmptyContent),
                new(new S3Object { Key = "nottest3" }, EmptyContent)
            }, expectedOutputList1Data).SetName("MultipleItemsInS3OnlyOneReturnsBecauseRegex");
    }

    [Test, TestCaseSource(nameof(TestGenerateLoadMetadataFirstTestCaseSource))]
    public void
        TestGenerate_CallFunctionWithBuildS3AndConfigurationLoadMetadataFirst_ShouldReturnExpectedOutput(
            FromS3Config config,
            List<KeyValuePair<S3Object, byte[]?>> s3MockData, IEnumerable<Data<object>> expectedOutput)
    {
        // Arrange
        var mockS3Client = new Mock<IS3Client>();
        mockS3Client.Setup(m =>
            m.ListAllObjectsInS3Bucket(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                true)).ReturnsAsync(s3MockData.Select(data => data.Key));
        mockS3Client.Setup(m =>
                m.GetObjectFromObjectMetadata(It.IsAny<S3Object>(), It.IsAny<string>()))
            .Returns(new KeyValuePair<S3Object, byte[]?>(new S3Object { Key = "test" }, EmptyContent));
        var mockGenerator = new Mock<FromS3>();
        mockGenerator.Protected().Setup<IS3Client>("BuildS3Client")
            .Returns(mockS3Client.Object);
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
            if (!expectedData.Body!.Equals(data.Body))
            {
                Globals.Logger.LogWarning(
                    "data at index {DataIndex} not equal between output and expected output", dataIndex);
                areEqual = false;
            }
        }

        Assert.That(areEqual);
    }

    public static IEnumerable<TestCaseData> TestGenerateCaseSource()
    {
        var fakePrefix = "SomePath/Path/";
        var expectedOutputData = new Data<object> { Body = EmptyContent };
        var expectedOutputList1Data = new List<Data<object>> { expectedOutputData };
        var expectedOutputList3Data = new List<Data<object>>
            { expectedOutputData, expectedOutputData, expectedOutputData };
        var noPrefixConfig = new FromS3Config
        {
            S3 = new S3Config { StorageBucket = "Test" },
            DataArrangeOrder = DataArrangeOrder.AsciiAsc
        };
        yield return new TestCaseData(noPrefixConfig, new List<KeyValuePair<S3Object, byte[]>>(),
                new List<Data<object>>())
            .SetName("EmptyBucketStorageNoPrefix");
        var prefixConfig = new FromS3Config
        {
            S3 = new S3Config
            {
                StorageBucket = "Test",
                Prefix = fakePrefix
            },
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
        };
        yield return new TestCaseData(prefixConfig, new List<KeyValuePair<S3Object, byte[]>>(),
                new List<Data<object>>())
            .SetName("EmptyBucketStorageWithPrefix");
        var mockData3 = new List<KeyValuePair<S3Object, byte[]>>
        {
            new(new S3Object { Key = "test" }, EmptyContent)
        };
        yield return new TestCaseData(noPrefixConfig, mockData3, expectedOutputList1Data
        ).SetName("OneItemInS3EmptyPrefix");
        var mockData4 = new List<KeyValuePair<S3Object, byte[]>>
        {
            new(new S3Object { Key = $"{fakePrefix}test" }, EmptyContent),
        };
        yield return new TestCaseData(prefixConfig, mockData4, expectedOutputList1Data
        ).SetName("OneItemInS3WithPrefix");
        yield return new TestCaseData(noPrefixConfig,
            new List<KeyValuePair<S3Object, byte[]>>
            {
                new(new S3Object { Key = "test" }, EmptyContent),
                new(new S3Object { Key = "test2" }, EmptyContent),
                new(new S3Object { Key = "test3" }, EmptyContent)
            }, expectedOutputList3Data).SetName("MultipleItemsInS3EmptyPrefix");
        yield return new TestCaseData(noPrefixConfig,
            new List<KeyValuePair<S3Object, byte[]>>
            {
                new(new S3Object { Key = $"{fakePrefix}test" }, EmptyContent),
                new(new S3Object { Key = $"{fakePrefix}test2" }, EmptyContent),
                new(new S3Object { Key = $"{fakePrefix}test3" }, EmptyContent)
            }, expectedOutputList3Data).SetName("MultipleItemsInS3WithPrefix");
        var countOneConfig = new FromS3Config
        {
            S3 = new S3Config { StorageBucket = "Test" },
            Count = 1,
            DataArrangeOrder = DataArrangeOrder.AsciiAsc
        };
        yield return new TestCaseData(countOneConfig,
            new List<KeyValuePair<S3Object, byte[]>>
            {
                new(new S3Object { Key = "test" }, EmptyContent),
                new(new S3Object { Key = "test2" }, EmptyContent),
                new(new S3Object { Key = "test3" }, EmptyContent)
            }, expectedOutputList1Data).SetName("MultipleItemsInS3OnlyOneReturnsBecauseCount");
        var regexConfig = new FromS3Config
        {
            S3 = new S3Config { StorageBucket = "Test" },
            DataUuidRegexExpression = "realtest.*",
            DataArrangeOrder = DataArrangeOrder.AsciiAsc
        };
        yield return new TestCaseData(regexConfig,
            new List<KeyValuePair<S3Object, byte[]>>
            {
                new(new S3Object { Key = "realtest" }, EmptyContent),
                new(new S3Object { Key = "nottest2" }, EmptyContent),
                new(new S3Object { Key = "nottest3" }, EmptyContent)
            }, expectedOutputList1Data).SetName("MultipleItemsInS3OnlyOneReturnsBecauseRegex");
    }

    [Test, TestCaseSource(nameof(TestGenerateCaseSource))]
    public void TestLoadSerializedDataSource_CallFunctionWithBuildS3AndConfiguration_ShouldReturnExpectedOutput(
        FromS3Config config,
        List<KeyValuePair<S3Object, byte[]?>> s3MockData, IEnumerable<Data<object>> expectedOutput)
    {
        // Arrange
        var mockS3Client = new Mock<IS3Client>();
        mockS3Client.Setup(m =>
            m.ListAllObjectsInS3Bucket(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                true)).ReturnsAsync(s3MockData.Select(data => data.Key));
        mockS3Client.Setup(m =>
                m.GetAllObjectsInS3BucketUnOrdered(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<bool>()))
            .Returns(s3MockData);
        var mockGenerator = new Mock<FromS3>();
        mockGenerator.Protected().Setup<IS3Client>("BuildS3Client")
            .Returns(mockS3Client.Object);
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
            if (!expectedData.Body!.Equals(data.Body))
            {
                Globals.Logger.LogWarning(
                    "data at index {DataIndex} not equal between output and expected output", dataIndex);
                areEqual = false;
            }
        }

        Assert.That(areEqual);
    }

    [Test, TestCaseSource(nameof(LoadMetadataFirstParityCaseSource))]
    public void TestGenerate_LoadMetadataFirst_ShouldMatchDirectLoadAcrossDifferentS3ObjectTypes(
        FromS3Config baseConfig,
        List<KeyValuePair<S3Object, byte[]>> s3MockData)
    {
        var metadataFirstOutput = GenerateFromS3(baseConfig with { LoadMetadataFirst = true }, s3MockData);
        var directLoadOutput = GenerateFromS3(baseConfig with { LoadMetadataFirst = false }, s3MockData);

        Assert.That(metadataFirstOutput.Count, Is.EqualTo(directLoadOutput.Count));

        for (var index = 0; index < metadataFirstOutput.Count; index++)
        {
            var metadataFirstData = metadataFirstOutput[index];
            var directLoadData = directLoadOutput[index];

            Assert.That(metadataFirstData.Body, Is.EqualTo(directLoadData.Body),
                $"Body mismatch at index {index} for key {metadataFirstData.MetaData?.Storage?.Key}");
            Assert.That(metadataFirstData.MetaData?.Storage?.Key, Is.EqualTo(directLoadData.MetaData?.Storage?.Key),
                $"Storage metadata mismatch at index {index}");
        }
    }

    [Test, TestCaseSource(nameof(TestGetStorageKeysCaseSource))]
    public void TestGetStorageKeyFromData_CallFunctionWithBuildS3AndConfiguration_ShouldReturnExpectedOutput(
        string input, string? expectedOutput, StorageMetaData storageMetaData, string prefix)
    {
        var generator = new FromS3
        {
            Configuration = new FromS3Config
            {
                StorageMetaData = storageMetaData,
                S3 = new S3Config { StorageBucket = "Test", Prefix = prefix },
                DataArrangeOrder = DataArrangeOrder.AsciiAsc
            },
            Context = Globals.Context
        };
        var output = _getStorageKeyFromDataMethodInfo.Invoke(generator, [input]);

        Assert.That((string?)output == expectedOutput);
    }

    [Test,
     TestCase(2), TestCase(3), TestCase(4), TestCase(5)]
    public void TestGenerateLazily_CallFunctionWithBuiltS3AndConfiguration_ShouldLoadDataLazily(
        int numberOfItemsToGenerate)
    {
        // Arrange
        var mockS3 = new Mock<IS3Client>();
        mockS3.Setup(m =>
                m.ListAllObjectsInS3Bucket(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true))
            .ReturnsAsync(Enumerable.Range(0, numberOfItemsToGenerate)
                .Select(i => new S3Object { Key = $"test{i}" }));

        mockS3.Setup(m =>
                m.GetObjectFromObjectMetadata(It.IsAny<S3Object>(), It.IsAny<string>()))
            .Returns(new KeyValuePair<S3Object, byte[]?>(new S3Object { Key = "test" }, []));

        var mockGenerator = new Mock<FromS3>();
        mockGenerator.Protected().Setup<IS3Client>("BuildS3Client").Returns(mockS3.Object);
        mockGenerator.CallBase = true;

        var generator = mockGenerator.Object;

        generator.Configuration = new FromS3Config
        {
            S3 = new S3Config { StorageBucket = "Test" },
            Count = numberOfItemsToGenerate,
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            LoadMetadataFirst = true
        };

        generator.Context = Globals.Context;

        mockGenerator.Protected()
            .Setup<SerializedLoadedData>("LoadData",
                ItExpr.IsAny<KeyValuePair<string, KeyValuePair<S3Object, byte[]>>>())
            .Returns(new SerializedLoadedData
            {
                FullKey = "test",
                Content = [],
                MetaData = null
            });

        // Act & Assert
        var (testResults, allPassed, itemCount, sideEffectCounter) = Globals.RunLazinessTest<FromS3Config, FromS3>(
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
    public void TestLoadObjectsMetadata_CallFunctionWithBuiltS3AndConfiguration_ShouldLoadDataLazily(int numberOfItemsToGenerate)
    {
        // Arrange
        var mockS3 = new Mock<IS3Client>();
        var dummyS3Objects = Enumerable.Range(0, numberOfItemsToGenerate)
            .Select(i => new S3Object { Key = $"test{i}" })
            .ToList();

        // Mock the actual S3 client methods that would be called
        mockS3.Setup(m => m.ListAllObjectsInS3Bucket(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .ReturnsAsync(dummyS3Objects);

        mockS3.Setup(m => m.GetAllObjectsInS3BucketUnOrdered(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Returns(dummyS3Objects.Select(obj => new KeyValuePair<S3Object, byte[]?>(obj, [0])));

        var mockGenerator = new Mock<FromS3>();
        mockGenerator.Protected().Setup<IS3Client>("BuildS3Client").Returns(mockS3.Object);
        mockGenerator.CallBase = true;

        var generator = mockGenerator.Object;

        generator.Configuration = new FromS3Config
        {
            S3 = new S3Config
            {
                StorageBucket = "Test",
                Prefix = "",
                Delimiter = "",
                SkipEmptyObjects = true
            },
            Count = numberOfItemsToGenerate,
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            LoadMetadataFirst = true
        };

        generator.Context = Globals.Context;

        // Setup LoadData to avoid actual file operations
        mockGenerator.Protected()
            .Setup<SerializedLoadedData>("LoadData",
                ItExpr.IsAny<KeyValuePair<string, KeyValuePair<S3Object, byte[]>>>())
            .Returns(new SerializedLoadedData
            {
                FullKey = "test",
                Content = [],
                MetaData = null
            });

        // Act & Assert
        var (testResults, allPassed, itemCount, sideEffectCounter) = Globals.RunLazinessTest<FromS3Config, FromS3>(
            generator,
            generator.Generate,
            numberOfItemsToGenerate,
            mock =>
            {
                // Verify that S3 operations are not called during setup
                // Since these are extension methods, we can't directly verify them
                // But we can verify that the mock was set up correctly
            });

        Globals.AssertLazinessProperties(testResults, itemCount, sideEffectCounter, numberOfItemsToGenerate, allPassed);

        // Additional verification: Test the LoadObjectsMetadata method directly using reflection
        // This ensures that the lazy evaluation works for the metadata loading itself
        var loadObjectsMetadataMethod = typeof(FromS3).GetMethod("LoadObjectsMetadata",
            BindingFlags.NonPublic | BindingFlags.Instance);
        var metadataResult =
            loadObjectsMetadataMethod.Invoke(generator, null) as
                IEnumerable<KeyValuePair<string, KeyValuePair<S3Object, byte[]>>>;

        // Verify that the metadata loading happened correctly
        Assert.That(metadataResult, Is.Not.Null);
        Assert.That(metadataResult.Count(), Is.EqualTo(numberOfItemsToGenerate));
        
    }

    private static List<Data<object>> GenerateFromS3(
        FromS3Config config,
        IEnumerable<KeyValuePair<S3Object, byte[]>> s3MockData)
    {
        var objectDataByKey = s3MockData.ToDictionary(item => item.Key.Key, item => item);
        var mockS3Client = new Mock<IS3Client>();

        mockS3Client.Setup(m =>
                m.ListAllObjectsInS3Bucket(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<bool>()))
            .ReturnsAsync(s3MockData.Select(item => item.Key).ToList());

        mockS3Client.Setup(m =>
                m.GetObjectFromObjectMetadata(It.IsAny<S3Object>(), It.IsAny<string>()))
            .Returns((S3Object s3Object, string _) =>
            {
                var objectData = objectDataByKey[s3Object.Key];
                return new KeyValuePair<S3Object, byte[]?>(objectData.Key, objectData.Value);
            });

        mockS3Client.Setup(m =>
                m.GetAllObjectsInS3BucketUnOrdered(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<bool>()))
            .Returns(s3MockData.Select(item => new KeyValuePair<S3Object, byte[]?>(item.Key, item.Value)));

        var mockGenerator = new Mock<FromS3>();
        mockGenerator.Protected().Setup<IS3Client>("BuildS3Client")
            .Returns(mockS3Client.Object);
        mockGenerator.CallBase = true;

        var generator = mockGenerator.Object;
        generator.Configuration = config;
        generator.Context = Globals.Context;

        return generator.Generate(new List<SessionData>().ToImmutableList(),
            new List<DataSource>().ToImmutableList()).ToList();
    }
}
