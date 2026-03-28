using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations.SessionDataConfigurations;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Extensions;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session;
using QaaS.Framework.SDK.Session.DataObjects;


namespace QaaS.Common.Generators.FromDataSourcesGenerators;

/// <summary>
/// Generates data from the enumerable of data sources it receives, presumes all items in the enumerable are serialized
/// and can be treated as a byte array
/// </summary>
/// <qaas-docs group="Existing data sources" subgroup="Session data reuse" />
public class FromSessionDataDataSources : BaseGenerator<List<FromSessionDataDataSourcesConfiguration>>
{
    /// <inheritdoc />
    public override IEnumerable<Data<object>> Generate(
        IImmutableList<Framework.SDK.Session.SessionDataObjects.SessionData> sessionDataList,
        IImmutableList<DataSource> dataSourceList)
    {
        var loadedSessionDataList = dataSourceList
            .SelectMany(dataSource => DeserializeAndValidateDataSource(sessionDataList, dataSource)).ToList();
        Context.Logger.LogInformation("Loaded {SessionDataListCount} session data items for generation with " +
                                      "{GeneratorType} generator", loadedSessionDataList.Count, GetType());
        foreach (var sessionConfig in Configuration)
        {
            // Find relevant session data
            var relevantSessionData =
                loadedSessionDataList.GetSessionDataByName(sessionConfig.SessionName!);

            // Return relevant communication data items
            foreach (var communicationData in sessionConfig.CommunicationDataList!)
            {
                switch (communicationData.Type!)
                {
                    case CommunicationDataType.Input:
                        foreach (var input in relevantSessionData
                                     .GetInputByName(communicationData.Name!).Data)
                            yield return input;
                        break;
                    case CommunicationDataType.Output:
                        foreach (var output in relevantSessionData
                                     .GetOutputByName(communicationData.Name!).Data)
                            yield return output;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(communicationData.Type), communicationData.Type,
                            "Communication data type not supported");
                }
            }
        }
    }

    /// <summary>
    /// Validates given data source can be converted to a `SessionData` object and returns it as one
    /// </summary>
    private IEnumerable<Framework.SDK.Session.SessionDataObjects.SessionData> DeserializeAndValidateDataSource(
        IImmutableList<Framework.SDK.Session.SessionDataObjects.SessionData> sessionDataList, DataSource dataSource)
    {
        var dataIndex = 0;
        foreach (var data in dataSource.Retrieve(sessionDataList))
        {
            if (data.Body is not byte[] serializedSessionData)
                throw new ArgumentException(
                    $"Data Source {dataSource.Name} contains an item not serialized as byte[] at index {dataIndex}" +
                    $", so it cannot be used in generator {GetType()}.");

            yield return SessionDataSerialization
                .DeserializeSessionData(serializedSessionData) ?? throw new ArgumentException(
                $"Data source {dataSource.Name} contains a NULL session data item at index {dataIndex} ");

            dataIndex++;
        }
    }
    
}
