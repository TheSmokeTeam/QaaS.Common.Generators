using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using QaaS.Common.Generators.Tests.ConfigurationObjects.Laziness;
using QaaS.Framework.SDK.ContextObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;
using Serilog;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace QaaS.Common.Generators.Tests;

public static class Globals
{
    public static readonly ILogger Logger = new SerilogLoggerFactory(
        new LoggerConfiguration().MinimumLevel.Debug()
            .WriteTo.NUnitOutput()
            .CreateLogger()).CreateLogger("TestsLogger");

    public static readonly Context Context = new()
    {
        Logger = Logger, RootConfiguration = new ConfigurationBuilder().Build()
    };

    public static string rootPath = "$";

    public static (Dictionary<string, bool> testResults, bool allPassed, int itemCount, int sideEffectCounter)
        RunLazinessTest<TConfig, TGenerator>(
            TGenerator generator,
            Func<IImmutableList<SessionData>, IImmutableList<DataSource>, IEnumerable<Data<object>>> generateFunc,
            int numberOfItemsToGenerate,
            Action<Mock<TGenerator>> configureMock = null)
        where TGenerator : class, IGenerator
    {
        // Configure mock if needed
        if (configureMock != null)
        {
            var mock = Mock.Get(generator);
            configureMock(mock);
        }

        // Run comprehensive laziness test
        var (testResults, allPassed) = GenerateFunctionLazinessTester.TestAllLazinessProperties(
            generatorFunc: () => generateFunc([new SessionData()], [new DataSource()]),
            out var sideEffectCounter);

        // Verify results
        var itemCount = generateFunc([new SessionData()], [new DataSource()]).Count();

        return (testResults, allPassed, itemCount, sideEffectCounter);
    }

    public static void AssertLazinessProperties(Dictionary<string, bool> testResults, int itemCount,
        int sideEffectCounter,
        int numberOfItemsToGenerate, bool allPassed)
    {
        Assert.Multiple(() =>
        {
            // Core lazy evaluation properties
            Assert.That(testResults[LazinessTestResults.LazyStart.ToString()], Is.True,
                "1. Lazy Start: Generator should not enumerate inputs at call time");
            Assert.That(testResults[LazinessTestResults.StreamingLazy.ToString()], Is.True,
                "2. Streaming Lazy: Generator should stream elements one at a time");
            Assert.That(testResults[LazinessTestResults.BufferingLazy.ToString()], Is.True,
                "3. Buffering Lazy: Generator should buffer elements when appropriate");
            Assert.That(itemCount, Is.EqualTo(numberOfItemsToGenerate), "Should produce correct number of items");

            // Additional behaviors
            Assert.That(testResults[LazinessTestResults.MultiPass.ToString()], Is.True,
                "4. Multi-Pass: Generator should be able to enumerate multiple times");
            Assert.That(testResults[LazinessTestResults.PartialLaziness.ToString()], Is.True,
                "5. Partial Laziness: Generator should handle partial lazy evaluation");
            Assert.That(testResults[LazinessTestResults.Deterministic.ToString()], Is.True,
                "6. Deterministic: Generator should have deterministic behavior");

            // Side effect verification
            Assert.That(sideEffectCounter, Is.EqualTo(numberOfItemsToGenerate),
                "Should have correct side effect count");
            // Final verification: all tests passed
            Assert.That(allPassed, Is.True, "All laziness properties must pass for a fully lazy generator");
        });
    }
}