using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;
using QaaS.Common.Generators.CsvGenerators;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Generators.FromExternalSourceGenerators;

public class FromCSV : BaseFromFileSystem<FromCSVConfig>
{
    public override IEnumerable<Data<object>> Generate(
        IImmutableList<SessionData> sessionDataList,
        IImmutableList<DataSource> dataSourceList)
    {
        if (Configuration.Count == 0)
        {
            yield break;
        }

        CreateDesiredResources();
        var generatedItemsCounter = 0;
        var foundAnyFiles = false;

        try
        {
            foreach (var file in ArrangeDataInOrder(RemoveElementsThatDontMatchRegex(LoadObjectsMetadata())))
            {
                foundAnyFiles = true;
                var loadedData = LoadData(file);
                var sourceStorageKey = GetStorageKeyFromData(loadedData.FullKey);
                var rowIndex = 0;
                foreach (var row in CsvRowParser.Parse(loadedData.Content, Configuration, loadedData.FullKey))
                {
                    rowIndex++;
                    yield return new Data<object>
                    {
                        Body = row,
                        MetaData = AttachRowStorageKey(loadedData.MetaData, sourceStorageKey, rowIndex)
                    };

                    generatedItemsCounter++;
                    if (Configuration.Count != null && generatedItemsCounter == Configuration.Count)
                    {
                        yield break;
                    }
                }
            }
        }
        finally
        {
            DisposeResources();
        }

        if (!foundAnyFiles)
        {
            Context.Logger.LogError("No data found in external source for generator {GeneratorType}", GetType());
        }

        if (Configuration.Count != null && generatedItemsCounter < Configuration.Count)
        {
            throw new ArgumentException($"Count given to generator {GetType()} exceeds the number of rows available " +
                                        "in the configured CSV files. " +
                                        $"Available rows count is {generatedItemsCounter} and provided count is {Configuration.Count}.");
        }
    }

    private static MetaData? AttachRowStorageKey(MetaData? fileMetaData, string? sourceStorageKey, int rowIndex)
    {
        if (sourceStorageKey is null)
        {
            return fileMetaData;
        }

        return (fileMetaData ?? new MetaData()) with
        {
            Storage = new Storage
            {
                Key = CsvRowParser.AppendRowIndexToStorageKey(sourceStorageKey, rowIndex)
            }
        };
    }
}
