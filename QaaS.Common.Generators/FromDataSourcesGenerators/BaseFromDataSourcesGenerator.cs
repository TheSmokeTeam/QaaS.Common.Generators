using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session.DataObjects;


namespace QaaS.Common.Generators.FromDataSourcesGenerators;

/// <summary>
/// Base generator for all generators using data sources, does all the generic actions that are to be done with the
/// data source items and leaves room for custom implementation of
/// the conversion of `data source` to `generated data`
/// </summary>
/// <typeparam name="TDataSourceConfig"> A configuration object used by the generator </typeparam>
public abstract class BaseFromDataSourcesGenerator<TDataSourceConfig> : BaseGenerator<TDataSourceConfig>
    where TDataSourceConfig : BaseFromDataSourcesConfiguration, new()
{
    /// <inheritdoc />
    public override IEnumerable<Data<object>> Generate(
        IImmutableList<Framework.SDK.Session.SessionDataObjects.SessionData> sessionDataList,
        IImmutableList<DataSource> dataSourceList)
    {
        var generatedItemsCounter = 0;
        foreach (var dataSource in dataSourceList)
        {
            var generatedData = dataSource.Retrieve(sessionDataList);
            var enumerator = generatedData.GetEnumerator();
            if (enumerator.MoveNext())
            {
                do
                {
                    var data = enumerator.Current;
                    // If generated as many items as user requested already stop generating
                    if (Configuration.Count != null && Configuration.Count == generatedItemsCounter) break;

                    Context.Logger.LogDebug("Generating data from Data Source {DataSourceName}", dataSource.Name);
                    yield return ConvertDataSourceDataToGenerateData(data, dataSource.Name);
                    generatedItemsCounter++;
                } while (enumerator.MoveNext());

                var expectedGeneratedItems = Configuration.Count ?? generatedItemsCounter;
                if (generatedItemsCounter < expectedGeneratedItems)
                    throw new ArgumentException($"Count given to generator {GetType()} " +
                                                " exceeds the number of items available in the data sources provided to the" +
                                                $" data source. Available items count is {generatedItemsCounter} and provided count is {expectedGeneratedItems}." +
                                                $" Provided data sources: {string.Join(", ", dataSourceList.Select(source => source.Name))}",
                        nameof(BaseFromDataSourcesConfiguration.Count));
            }
            else
                Context.Logger.LogWarning("Data Source {DataSourceName} contains no items " +
                                          "and will not be returned by generator {GeneratorName}",
                    dataSource.Name, GetType());


            enumerator.Dispose();
            // If generated as many items as user requested already stop generating
            if (Configuration.Count != null && Configuration.Count == generatedItemsCounter) break;
        }
    }

    /// <summary>
    /// Converts the given data source item to the generated `Data` object
    /// </summary>
    /// <param name="data"> The data source item to convert to generated data </param>
    /// <param name="dataSourceName"> The name of the data source the data item came from </param>
    /// <returns> A generated data fit to be used in sessions </returns>
    protected abstract Data<object> ConvertDataSourceDataToGenerateData(
        Data<object> data, string dataSourceName);
}