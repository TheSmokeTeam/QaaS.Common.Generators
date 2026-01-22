using System.Collections.Immutable;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Generators.FromDataSourcesGenerators;

public class Stacking: BaseGenerator<StackingConfiguration>
{
    public override IEnumerable<Data<object>> Generate(IImmutableList<SessionData> sessionDataList, 
        IImmutableList<DataSource> dataSourceList)
    {
        var generatorEnumerators = dataSourceList.Select(source => source.Retrieve(sessionDataList).
            GetEnumerator()).ToArray();
        var finishedGenerators = new bool[generatorEnumerators.Length];
        var generatorIndex = 0;
        var itemsPerGeneratorIndex = 0;
        var totalItemsGenerated = 0;

        while (finishedGenerators.Contains(false) && (Configuration.Count is null || totalItemsGenerated < Configuration.Count))
        {
            if (finishedGenerators[generatorIndex])
            {
                // Move to the next generator (in round-robin)
                generatorIndex = (generatorIndex + 1) % generatorEnumerators.Length;
                itemsPerGeneratorIndex = (itemsPerGeneratorIndex + 1) % Configuration.ItemsPerGenerator!.Length;
                continue;
            }
            var enumerator = generatorEnumerators[generatorIndex];
            var itemsToGenerate = Configuration.ItemsPerGenerator![itemsPerGeneratorIndex];

            for (var itemNumber = 0; itemNumber < itemsToGenerate && 
                                     (Configuration.Count is null || totalItemsGenerated < Configuration.Count); itemNumber++)
            {
                var item = GenerateItem(enumerator, dataSourceList[generatorIndex], sessionDataList);
                if (item is null)
                {
                    finishedGenerators[generatorIndex] = true;
                    break;
                }
                totalItemsGenerated++;
                yield return item;
            }
            
            generatorIndex = (generatorIndex + 1) % generatorEnumerators.Length;
            itemsPerGeneratorIndex = (itemsPerGeneratorIndex + 1) % Configuration.ItemsPerGenerator!.Length;
        }

        foreach (var enumerator in generatorEnumerators)
            enumerator.Dispose();

        if (Configuration.Count is not null && totalItemsGenerated < Configuration.Count)
            throw new ArgumentException($"Count given to generator {GetType()} " +
                                        " exceeds the number of items available in the data sources provided to the" +
                                        $" data source. Available items count is {totalItemsGenerated} and provided " +
                                        $"count is {Configuration.Count}." +
                                        $" Provided data sources: {string.Join(", ", dataSourceList.Select(source => source.Name))}",
                nameof(BaseFromDataSourcesConfiguration.Count));
    }

    private Data<object>? GenerateItem(IEnumerator<Data<object>> enumerator, DataSource dataSource, 
        IImmutableList<SessionData> sessionDataList)
    {
        if (enumerator.MoveNext()) return enumerator.Current;
        if (!Configuration.LoopFinishedGenerators) return null;
        enumerator.Dispose();
        enumerator = dataSource.Retrieve(sessionDataList).GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current :  null;
    }
}