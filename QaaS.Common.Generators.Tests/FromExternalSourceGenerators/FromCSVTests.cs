using System.Collections.Immutable;
using System.IO.Abstractions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;
using QaaS.Common.Generators.FromExternalSourceGenerators;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.Tests.FromExternalSourceGenerators;

public class FromCSVTests
{
    private static readonly string TestsDirectoryPath = Path.Join("TestData");
    private static readonly string CsvDirectoryRelativePath = Path.Join("CsvFiles");

    [Test]
    public void TestGenerate_WithCsvFiles_ShouldReturnRowsInFileOrderWithStorageKeys()
    {
        var generator = BuildGenerator(new FromCSVConfig
        {
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            StorageMetaData = StorageMetaData.ItemName,
            FileSystem = new FileSystemConfig
            {
                Path = CsvDirectoryRelativePath,
                SearchPattern = "orders-*.csv"
            }
        });

        var output = generator.Generate(ImmutableArray<SessionData>.Empty, ImmutableArray<DataSource>.Empty).ToList();

        Assert.That(output.Count, Is.EqualTo(3));
        AssertRow(output[0], "orders-a.csv#1", ("Id", "1"), ("Product", "Keyboard"), ("Quantity", "2"));
        AssertRow(output[1], "orders-a.csv#2", ("Id", "2"), ("Product", "Mouse, Wireless"), ("Quantity", "1"));
        AssertRow(output[2], "orders-b.csv#1", ("Id", "3"), ("Product", "Monitor"), ("Quantity", "4"));
    }

    [Test]
    public void TestGenerate_WithHeaderlessCsv_ShouldUseConfiguredColumnNames()
    {
        var generator = BuildGenerator(new FromCSVConfig
        {
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            StorageMetaData = StorageMetaData.ItemName,
            HasHeaderRecord = false,
            ColumnNames = ["Id", "Name"],
            FileSystem = new FileSystemConfig
            {
                Path = CsvDirectoryRelativePath,
                SearchPattern = "customers-no-header.csv"
            }
        });

        var output = generator.Generate(ImmutableArray<SessionData>.Empty, ImmutableArray<DataSource>.Empty).ToList();

        Assert.That(output.Count, Is.EqualTo(2));
        AssertRow(output[0], "customers-no-header.csv#1", ("Id", "100"), ("Name", "Alice"));
        AssertRow(output[1], "customers-no-header.csv#2", ("Id", "101"), ("Name", "Bob"));
    }

    [Test]
    public void TestGenerate_WithCount_ShouldStopAfterRequestedNumberOfRows()
    {
        var generator = BuildGenerator(new FromCSVConfig
        {
            Count = 2,
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            StorageMetaData = StorageMetaData.ItemName,
            FileSystem = new FileSystemConfig
            {
                Path = CsvDirectoryRelativePath,
                SearchPattern = "orders-*.csv"
            }
        });

        var output = generator.Generate(ImmutableArray<SessionData>.Empty, ImmutableArray<DataSource>.Empty).ToList();

        Assert.That(output.Count, Is.EqualTo(2));
        AssertRow(output[0], "orders-a.csv#1", ("Id", "1"), ("Product", "Keyboard"), ("Quantity", "2"));
        AssertRow(output[1], "orders-a.csv#2", ("Id", "2"), ("Product", "Mouse, Wireless"), ("Quantity", "1"));
    }

    [Test]
    public void TestRetrieve_WithJsonSerializerAndDeserializer_ShouldRoundTripCsvRows()
    {
        var generator = BuildGenerator(new FromCSVConfig
        {
            DataArrangeOrder = DataArrangeOrder.AsciiAsc,
            StorageMetaData = StorageMetaData.ItemName,
            FileSystem = new FileSystemConfig
            {
                Path = CsvDirectoryRelativePath,
                SearchPattern = "orders-a.csv"
            }
        });

        var serializingDataSource = new DataSource
        {
            Name = "OrdersCsvSerialized",
            Generator = generator,
            DataSourceList = ImmutableArray<DataSource>.Empty,
            Serializer = SerializerFactory.BuildSerializer(SerializationType.Json)
        };

        var serializedRows = serializingDataSource.Retrieve(ImmutableArray<SessionData>.Empty).ToList();
        Assert.That(serializedRows.Count, Is.EqualTo(2));
        Assert.That(serializedRows[0].Body, Is.TypeOf<byte[]>());

        var deserializingGenerator = new Mock<IGenerator>();
        deserializingGenerator.Setup(generatorMock =>
                generatorMock.Generate(It.IsAny<IImmutableList<SessionData>>(), It.IsAny<IImmutableList<DataSource>>()))
            .Returns(serializedRows);

        var deserializingDataSource = new DataSource
        {
            Name = "OrdersCsvDeserialized",
            Generator = deserializingGenerator.Object,
            DataSourceList = ImmutableArray<DataSource>.Empty,
            Deserializer = DeserializerFactory.BuildDeserializer(SerializationType.Json),
            DeserializerSpecificType = typeof(Dictionary<string, string>)
        };

        var output = deserializingDataSource.Retrieve(ImmutableArray<SessionData>.Empty).ToList();

        Assert.That(output.Count, Is.EqualTo(2));
        Assert.That(output[0].Body, Is.TypeOf<Dictionary<string, string>>());
        var firstRow = (Dictionary<string, string>)output[0].Body!;
        Assert.That(firstRow["Id"], Is.EqualTo("1"));
        Assert.That(firstRow["Product"], Is.EqualTo("Keyboard"));
        Assert.That(output[0].MetaData?.Storage?.Key, Is.EqualTo("orders-a.csv#1"));
    }

    private static FromCSV BuildGenerator(FromCSVConfig configuration)
    {
        var mockGenerator = new Mock<FromCSV>();
        mockGenerator.Protected().Setup<KeyValuePair<string, IFileSystem>>("BuildFileSystem")
            .Returns(new KeyValuePair<string, IFileSystem>(TestsDirectoryPath, new FileSystem()));
        mockGenerator.CallBase = true;

        var generator = mockGenerator.Object;
        generator.Configuration = configuration;
        generator.Context = Globals.Context;
        return generator;
    }

    private static void AssertRow(
        Data<object> actualRow,
        string expectedStorageKey,
        params (string Key, string? Value)[] expectedFields)
    {
        Assert.That(actualRow.MetaData?.Storage?.Key, Is.EqualTo(expectedStorageKey));
        Assert.That(actualRow.Body, Is.TypeOf<Dictionary<string, string?>>());
        var actualFields = (Dictionary<string, string?>)actualRow.Body!;

        Assert.That(actualFields.Count, Is.EqualTo(expectedFields.Length));
        foreach (var (key, value) in expectedFields)
        {
            Assert.That(actualFields.TryGetValue(key, out var actualValue), Is.True, $"Missing field `{key}`");
            Assert.That(actualValue, Is.EqualTo(value), $"Unexpected value for field `{key}`");
        }
    }
}
