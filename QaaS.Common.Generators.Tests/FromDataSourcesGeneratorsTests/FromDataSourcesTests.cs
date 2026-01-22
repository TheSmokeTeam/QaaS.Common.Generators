using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Common.Generators.FromDataSourcesGenerators;
using QaaS.Common.Generators.Tests.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;

using SessionData = QaaS.Framework.SDK.Session.SessionDataObjects.SessionData;

namespace QaaS.Common.Generators.Tests.FromDataSourcesGeneratorsTests;

public class FromDataSourcesTests
{

    private static IEnumerable<TestCaseData> TestGenerateCaseSource()
    {
        var byteArrayItem = new byte[] { 1, 2, 3 };
        const string dataSourceName = "Test";
        var data = new Data<object>() { Body = byteArrayItem };
        var dataSourceWithOneData = new DataSource() { Name = dataSourceName };
        var dataSourceWithTwoData = new DataSource() { Name = dataSourceName };
        var dataSourceWithThreeData = new DataSource() { Name = dataSourceName };
        var dataSourceWithFourData = new DataSource() { Name = dataSourceName };


        dataSourceWithOneData.SetGeneratedData(new List<Data<object>>() { data });
        dataSourceWithTwoData.SetGeneratedData(new List<Data<object>>() { data, data });
        dataSourceWithThreeData.SetGeneratedData(new List<Data<object>>() { data, data, data });
        dataSourceWithFourData.SetGeneratedData(new List<Data<object>>() { data, data, data, data });

        yield return new TestCaseData(new FromDataSourceBasedConfiguration(),
            new List<DataSource>(), new List<Data<object>>()).SetName("EmptyDataSourceEmptyConfiguration");

        yield return new TestCaseData(new FromDataSourceBasedConfiguration(),
            new List<DataSource>
            {
                dataSourceWithOneData
            }, new List<Data<object>>
            {
                new() { Body = byteArrayItem }
            }).SetName("OneItemOneDataSourceEmptyConfiguration");

        yield return new TestCaseData(new FromDataSourceBasedConfiguration(),
            new List<DataSource>
            {
                dataSourceWithOneData,
                dataSourceWithOneData
            }, new List<Data<object>>
            {
                data,
                data
            }).SetName("OneItemMultipleDataSourceEmptyConfiguration");

        yield return new TestCaseData(new FromDataSourceBasedConfiguration(),
            new List<DataSource>
            {
                dataSourceWithFourData
            }, new List<Data<object>>
            {
                data,
                data,
                data,
                data
            }).SetName("MultipleItemsOneDataSourceEmptyConfiguration");

        yield return new TestCaseData(new FromDataSourceBasedConfiguration(),
            new List<DataSource>
            {
                dataSourceWithThreeData,
                dataSourceWithTwoData
            }, new List<Data<object>>
            {
                data,
                data,
                data,
                data,
                data,
            }).SetName("MultipleItemsMultipleDataSourceEmptyConfiguration");


        yield return new TestCaseData(new FromDataSourceBasedConfiguration { Count = 0 },
            new List<DataSource>
            {
                dataSourceWithThreeData,
                dataSourceWithTwoData
            }, new List<Data<object>>()).SetName("MultipleItemsMultipleDataSourceConfigurationWithCountBeing0");
    }

    [Test, TestCaseSource(nameof(TestGenerateCaseSource))]
    public void TestGenerate_CallGenerateFunctionWithFakeDataSources_ShouldReturnExpectedOutput
    (FromDataSourceBasedConfiguration config, List<DataSource> dataSourceList,
        List<Data<object>> expectedOutput)
    {
        // Arrange
        var generator = new FromDataSources
        {
            Context = Globals.Context,
            Configuration = config
        };

        // Act
        var output = generator.Generate(new ImmutableArray<SessionData>(),
            dataSourceList.ToImmutableList()).ToList();

        // Assert
        if (expectedOutput.Count != output.Count)
        {
            Assert.Fail($"Output count ({output.Count}) not equal to expected count ({expectedOutput.Count})");
            return;
        }

        var areEqual = true;
        for (var dataIndex = 0; dataIndex < output.Count; dataIndex++)
        {
            var expectedData = expectedOutput[dataIndex];
            var data = output[dataIndex];
            if (expectedData.Body != data.Body)
            {
                Globals.Logger.LogWarning(
                    "data at index {DataIndex} not equal between output and expected output", dataIndex);
                areEqual = false;
            }
        }

        Assert.That(areEqual);
    }
}