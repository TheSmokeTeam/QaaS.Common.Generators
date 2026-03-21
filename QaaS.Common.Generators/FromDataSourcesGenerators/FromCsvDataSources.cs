using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Common.Generators.CsvGenerators;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Generators.FromDataSourcesGenerators;

/// <summary>
/// Generates structured row objects from the enumerable of data sources it receives that contain CSV data.
/// </summary>
public class FromCsvDataSources : BaseGenerator<FromCsvDataSourcesConfiguration>
{
    public override IEnumerable<Data<object>> Generate(
        IImmutableList<SessionData> sessionDataList,
        IImmutableList<DataSource> dataSourceList)
    {
        if (Configuration.Count == 0)
        {
            yield break;
        }

        var generatedItemsCounter = 0;
        foreach (var dataSource in dataSourceList)
        {
            var sourceItemIndex = 0;
            foreach (var sourceData in dataSource.Retrieve(sessionDataList))
            {
                var generatedRowsForCurrentSourceItem = 0;
                foreach (var generatedRow in ConvertCsvDataToGenerateData(sourceData, dataSource.Name, sourceItemIndex))
                {
                    if (Configuration.Count != null && generatedItemsCounter == Configuration.Count)
                    {
                        yield break;
                    }

                    Context.Logger.LogDebug("Generating CSV row from Data Source {DataSourceName}", dataSource.Name);
                    yield return generatedRow;
                    generatedItemsCounter++;
                    generatedRowsForCurrentSourceItem++;
                }

                if (generatedRowsForCurrentSourceItem == 0)
                {
                    Context.Logger.LogWarning("Data Source {DataSourceName} CSV item at index {ItemIndex} contains no rows " +
                                              "and will not be returned by generator {GeneratorName}",
                        dataSource.Name, sourceItemIndex, GetType());
                }

                sourceItemIndex++;
                if (Configuration.Count != null && generatedItemsCounter == Configuration.Count)
                {
                    yield break;
                }
            }
        }

        var expectedGeneratedItems = Configuration.Count ?? generatedItemsCounter;
        if (generatedItemsCounter < expectedGeneratedItems)
        {
            throw new ArgumentException($"Count given to generator {GetType()} exceeds the number of rows available " +
                                        "in the CSV data sources provided. " +
                                        $"Available rows count is {generatedItemsCounter} and provided count is {expectedGeneratedItems}. " +
                                        $"Provided data sources: {string.Join(", ", dataSourceList.Select(source => source.Name))}",
                nameof(BaseFromDataSourcesConfiguration.Count));
        }
    }

    private IEnumerable<Data<object>> ConvertCsvDataToGenerateData(Data<object> data, string dataSourceName, int dataIndex)
    {
        var sourceName = $"{dataSourceName} item {dataIndex}";
        var parsedRows = data.Body switch
        {
            byte[] csvBytes => CsvRowParser.Parse(csvBytes, Configuration, sourceName),
            string csvText => CsvRowParser.Parse(csvText, Configuration, sourceName),
            _ => throw new ArgumentException(
                $"Data source {dataSourceName} contains an item at index {dataIndex} not represented as UTF-8 byte[] or string, " +
                $"so it cannot be used in generator {GetType()}.")
        };

        var rowIndex = 0;
        foreach (var row in parsedRows)
        {
            rowIndex++;
            yield return new Data<object>
            {
                Body = row,
                MetaData = AttachRowStorageKey(data.MetaData, rowIndex)
            };
        }
    }

    private static MetaData? AttachRowStorageKey(MetaData? sourceMetaData, int rowIndex)
    {
        var storageKey = sourceMetaData?.Storage?.Key;
        if (storageKey is null)
        {
            return sourceMetaData;
        }

        return (sourceMetaData ?? new MetaData()) with
        {
            Storage = new Storage
            {
                Key = CsvRowParser.AppendRowIndexToStorageKey(storageKey, rowIndex)
            }
        };
    }
}
