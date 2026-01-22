using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Generators.FromExternalSourceGenerators;

public abstract class
    BaseExternalSourceBasedGenerator<TExternalSourceConfiguration, TFilesProperties> : BaseGenerator<
    TExternalSourceConfiguration>
    where TExternalSourceConfiguration : BaseExternalSourceBasedGeneratorConfig, new()
{
    /// <summary>
    /// Gets the storage key to send inside Data object
    /// </summary>
    /// <returns>A string representing the key of to store in metadata</returns>
    protected abstract string? GetStorageKeyFromData(string key);

    /// <summary>
    /// Create the desired resources needed to retrieve the objects
    /// </summary>
    protected virtual void CreateDesiredResources() {}

    /// <summary>
    /// Load an enumerable that contains for each object the required metadata for it to be loaded
    /// </summary>
    /// <returns>Enumerable that contains each object name and it's metadata</returns>
    protected abstract IEnumerable<KeyValuePair<string, TFilesProperties>> LoadObjectsMetadata();

    /// <summary>
    /// Load data from object metadata
    /// </summary>
    /// <param name="objectProperties"></param>
    /// <returns>The loaded object's body from the external data source</returns>
    protected abstract SerializedLoadedData LoadData(KeyValuePair<string, TFilesProperties> objectProperties);

    /// <summary>
    /// Dispose of the created resources that were used to retrieve from external data source
    /// </summary>
    protected virtual void DisposeResources() {}

    /// <summary>
    /// Arranges the given data given by the keys according to the configured order in DataArrangeOrder,
    /// the keys must be the full name of their objects in the external loaded data source.
    /// </summary>
    /// <returns> The given list arranged correctly </returns>
    /// <exception cref="NotSupportedException"> If DataArrangeOrder type is not supported exception is thrown
    /// </exception>
    private IEnumerable<KeyValuePair<string, TFilesProperties>> ArrangeDataInOrder(
        IEnumerable<KeyValuePair<string, TFilesProperties>> objectProperties)
    {
        Context.Logger.LogInformation("Arranging metadata source " +
                                      "by the given DataArrangeOrder {DataArrangeOrder}",
            Configuration.DataArrangeOrder.ToString());
        return Configuration.DataArrangeOrder switch
        {
            DataArrangeOrder.AsciiAsc => objectProperties.OrderBy(item
                => item.Key),
            DataArrangeOrder.AsciiDesc => objectProperties.OrderByDescending(item
                => item.Key),
            DataArrangeOrder.FirstNumericalAsc => objectProperties.OrderBy(item
                => ExtractNumericValue(item.Key)),
            DataArrangeOrder.FirstNumericalDesc => objectProperties.OrderByDescending(item
                => ExtractNumericValue(item.Key)),
            DataArrangeOrder.Unordered => objectProperties,
            _ => throw new NotSupportedException("DataArrangeOrder" +
                                                 $" {Configuration.DataArrangeOrder} not supported")
        };
    }

    /// <summary>
    /// Removes all the values that don't match the given regex pattern
    /// </summary>
    /// <param name="elementsToFilter">Elements to iterate over</param>
    /// <returns>The values that match the regex</returns>
    private IEnumerable<KeyValuePair<string, TFilesProperties>> RemoveElementsThatDontMatchRegex(
        IEnumerable<KeyValuePair<string, TFilesProperties>> elementsToFilter)
        => elementsToFilter.Where(obj =>
            Regex.IsMatch(obj.Key.ToString(), Configuration.DataUuidRegexExpression));


    /// <summary>
    /// Extracts the numeric value from the beginning of a string
    /// </summary>
    /// <param name="str"> The string to extract the numeric value from </param>
    /// <returns> The numeric value found in the string's beginning </returns>
    /// <exception cref="Exception"> If the numeric value found could not be parsed throws an exception </exception>
    private static long ExtractNumericValue(string str)
    {
        var numericPart = Regex.Match(str, @"-?\d+").Value;
        if (!long.TryParse(numericPart, out var numericValue))
            throw new ArgumentException(
                $"Could not extract numeric value from string {str}," +
                $" numeric part to be extracted was {numericPart}");
        return numericValue;
    }

    public override IEnumerable<Data<object>> Generate(IImmutableList<SessionData> sessionDataList,
        IImmutableList<DataSource> dataSourceList)
    {
        CreateDesiredResources();
        var enumerator = ArrangeDataInOrder(RemoveElementsThatDontMatchRegex(LoadObjectsMetadata())).GetEnumerator();
        if (!enumerator.MoveNext())
        {
            Context.Logger.LogError("No data found in external source for generator {GeneratorType}", GetType());
            enumerator.Dispose();
            yield break;
        }

        var generatedItemsCounter = 0;
        do
        {
            var serializedLoadedData = LoadData(enumerator.Current);
            var loadedDataStorageKey = GetStorageKeyFromData(serializedLoadedData.FullKey);
            yield return  loadedDataStorageKey != null
                ? new Data<object>
                {
                    Body = serializedLoadedData.Content,
                    MetaData = (serializedLoadedData.MetaData ?? new MetaData()) with
                    {
                        Storage = new Storage
                        {
                            Key = loadedDataStorageKey
                        }
                    }
                }
                : new Data<object>
                {
                    Body = serializedLoadedData.Content,
                    MetaData = serializedLoadedData.MetaData
                };

            generatedItemsCounter++;
        } while (enumerator.MoveNext() && (Configuration.Count == null || generatedItemsCounter < Configuration.Count));

        DisposeResources();
        enumerator.Dispose();
        if (generatedItemsCounter < Configuration.Count)
            throw new ArgumentException($"Count given to generator {GetType()} " +
                                        " exceeds the number of items available in the external source." +
                                        $"Available items count is {generatedItemsCounter} and provided count is {Configuration.Count}.");
    }
}
