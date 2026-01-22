using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Common.Generators.FromDataSourcesGenerators;
using QaaS.Common.Generators.Tests.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Generators.Tests.FromDataSourcesGeneratorsTests;

public class StackingTests
{
    private static IEnumerable<TestCaseData> TestGenerateCaseSource()
    {
        var dataSourceA = new DataSource { Name = "dataSourceA" };
        var dataSourceB = new DataSource { Name = "dataSourceB" };
        var dataSourceC = new DataSource { Name = "dataSourceC" };
        
        dataSourceA.SetGeneratedData(Enumerable.Repeat(new Data<object> {Body = "A"}, 2).ToList());
        dataSourceB.SetGeneratedData(Enumerable.Repeat(new Data<object> {Body = "B"}, 2).ToList());
        dataSourceC.SetGeneratedData(Enumerable.Repeat(new Data<object> {Body = "C"}, 3).ToList());

        yield return new TestCaseData(new StackingConfiguration(),
            new List<DataSource>(), new List<Data<object>>()).SetName("EmptyDataSourceEmptyConfiguration");

        yield return new TestCaseData(new StackingConfiguration {ItemsPerGenerator = [1] },
            new List<DataSource>
            {
                dataSourceA, dataSourceB
            }, new List<Data<object>>
            {
                new() { Body = "A" },
                new() { Body = "B" },
                new() { Body = "A" },
                new() { Body = "B" },
            }).SetName("SimpleCase");
        
        yield return new TestCaseData(new StackingConfiguration {ItemsPerGenerator = [1], Count = 4},
            new List<DataSource>
            {
                dataSourceA, dataSourceB, dataSourceC
            }, new List<Data<object>>
            {
                new() { Body = "A" },
                new() { Body = "B" },
                new() { Body = "C" },
                new() { Body = "A" },
            }).SetName("SimpleCaseWithCount");
        
        yield return new TestCaseData(new StackingConfiguration {ItemsPerGenerator = [2]},
            new List<DataSource>
            {
                dataSourceA, dataSourceB, dataSourceC
            }, new List<Data<object>>
            {
                new() { Body = "A" },
                new() { Body = "A" },
                new() { Body = "B" },
                new() { Body = "B" },
                new() { Body = "C" },
                new() { Body = "C" },
                new() { Body = "C" },
            }).SetName("CaseWithFinishingGenerators");
        
        yield return new TestCaseData(new StackingConfiguration {ItemsPerGenerator = [3, 2], Count = 10, 
                LoopFinishedGenerators = true},
            new List<DataSource>
            {
                dataSourceA, dataSourceB, dataSourceC
            }, new List<Data<object>>
            {
                new() { Body = "A" },
                new() { Body = "A" },
                new() { Body = "A" },
                new() { Body = "B" },
                new() { Body = "B" },
                new() { Body = "C" },
                new() { Body = "C" },
                new() { Body = "C" },
                new() { Body = "A" },
                new() { Body = "A" },
            }).SetName("UsingLoopFinishedGenerators");
        
        
    }

    [Test, TestCaseSource(nameof(TestGenerateCaseSource))]
    public void TestGenerate_CallGenerateFunctionWithFakeDataSources_ShouldReturnExpectedOutput
    (StackingConfiguration config, List<DataSource> dataSourceList,
        List<Data<object>> expectedOutput)
    {
        // Arrange
        var generator = new Stacking
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