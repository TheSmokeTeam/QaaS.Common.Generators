using System.Collections.Immutable;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Common.Generators.FromDataSourcesGenerators;
using QaaS.Common.Generators.Tests.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Generators.Tests.FromDataSourcesGeneratorsTests;

public class FromCsvDataSourcesTests
{
    [Test]
    public void TestGenerate_WithUtf8CsvBytesAndStorageKey_ShouldReturnExpectedRows()
    {
        var csv = "Id,Name,City\r\n1,Alice,London\r\n2,\"Bob, Jr.\",Paris";
        var dataSource = new DataSource { Name = "TestCsv" };
        dataSource.SetGeneratedData([
            new Data<object>
            {
                Body = System.Text.Encoding.UTF8.GetBytes(csv),
                MetaData = new MetaData
                {
                    Storage = new Storage
                    {
                        Key = "results.csv"
                    }
                }
            }
        ]);

        var generator = new FromCsvDataSources
        {
            Context = Globals.Context,
            Configuration = new FromCsvDataSourcesConfiguration()
        };

        var output = generator.Generate(ImmutableArray<SessionData>.Empty, [dataSource]).ToList();

        Assert.That(output.Count, Is.EqualTo(2));
        AssertRow(output[0], ("Id", "1"), ("Name", "Alice"), ("City", "London"));
        Assert.That(output[0].MetaData?.Storage?.Key, Is.EqualTo("results.csv#1"));
        AssertRow(output[1], ("Id", "2"), ("Name", "Bob, Jr."), ("City", "Paris"));
        Assert.That(output[1].MetaData?.Storage?.Key, Is.EqualTo("results.csv#2"));
    }

    [Test]
    public void TestGenerate_WithHeaderlessCsvAndCount_ShouldReturnConfiguredRowsOnly()
    {
        var firstDataSource = new DataSource { Name = "CustomersA" };
        firstDataSource.SetGeneratedData([
            new Data<object> { Body = "1,Alice\r\n2,Bob" }
        ]);

        var secondDataSource = new DataSource { Name = "CustomersB" };
        secondDataSource.SetGeneratedData([
            new Data<object> { Body = "3,Charlie\r\n4,Dana" }
        ]);

        var generator = new FromCsvDataSources
        {
            Context = Globals.Context,
            Configuration = new FromCsvDataSourcesConfiguration
            {
                HasHeaderRecord = false,
                ColumnNames = ["Id", "Name"],
                Count = 3
            }
        };

        var output = generator.Generate(ImmutableArray<SessionData>.Empty, [firstDataSource, secondDataSource]).ToList();

        Assert.That(output.Count, Is.EqualTo(3));
        AssertRow(output[0], ("Id", "1"), ("Name", "Alice"));
        AssertRow(output[1], ("Id", "2"), ("Name", "Bob"));
        AssertRow(output[2], ("Id", "3"), ("Name", "Charlie"));
    }

    [Test]
    public void TestGenerate_WithUnsupportedDataType_ShouldThrowArgumentException()
    {
        var dataSource = new DataSource { Name = "InvalidCsv" };
        dataSource.SetGeneratedData([
            new Data<object> { Body = 42 }
        ]);

        var generator = new FromCsvDataSources
        {
            Context = Globals.Context,
            Configuration = new FromCsvDataSourcesConfiguration()
        };

        var exception = Assert.Throws<ArgumentException>(() =>
            generator.Generate(ImmutableArray<SessionData>.Empty, [dataSource]).ToList());

        Assert.That(exception!.Message, Does.Contain("UTF-8 byte[] or string"));
    }

    [Test]
    public void TestGenerate_WithMismatchedColumnCount_ShouldThrowArgumentException()
    {
        var dataSource = new DataSource { Name = "BrokenCsv" };
        dataSource.SetGeneratedData([
            new Data<object> { Body = "Id,Name\r\n1,Alice\r\n2,Bob,Extra" }
        ]);

        var generator = new FromCsvDataSources
        {
            Context = Globals.Context,
            Configuration = new FromCsvDataSourcesConfiguration()
        };

        var exception = Assert.Throws<ArgumentException>(() =>
            generator.Generate(ImmutableArray<SessionData>.Empty, [dataSource]).ToList());

        Assert.That(exception!.Message, Does.Contain("contains 3 columns but expected 2"));
    }

    private static void AssertRow(Data<object> actualRow, params (string Key, string? Value)[] expectedFields)
    {
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
