using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.FromDataSourcesGenerators;

/// <summary>
/// Generates data from the enumerable of data sources it receives that is in `Lettuce` file format,
/// presumes all items in the enumerable are deserialized into <see cref="SerializationType.Json"/>
/// </summary>
public class FromLettuceDataSources : BaseFromDataSourcesGenerator<FromLettuceDataSourcesConfiguration>
{
    /// <inheritdoc />
    protected override Data<object> ConvertDataSourceDataToGenerateData(
        Data<object> data, string dataSourceName)
    {
        if (data.Body is not JsonNode lettuce)
            throw new ArgumentException($"Data source {dataSourceName} contains an item not deserialized" +
                                        $" into a {nameof(SerializationType.Json)} ({nameof(JsonNode)})," +
                                        $" so it cannot be used in generator {GetType()}.");

        var lettuceBody = Convert.FromBase64String(lettuce[Constants.Lettuce.BodyFieldName]?.ToString() ?? "");
        var lettuceRoutingKey = lettuce[Constants.Lettuce.RoutingKeyFieldName]?.ToString();
        var metaData = data.MetaData != null
            ? data.MetaData with
            {
                RabbitMq = new RabbitMq
                {
                    RoutingKey = lettuceRoutingKey
                }
            }
            : new MetaData
            {
                RabbitMq = new RabbitMq
                {
                    RoutingKey = lettuceRoutingKey
                }
            };
        return new Data<object>
        {
            Body = lettuceBody,
            MetaData = metaData
        };
    }
}