using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Generators.FromDataSourcesGenerators;

/// <summary>
/// Generates data from the enumerable of data sources it receives
/// </summary>
public class FromDataSources : BaseFromDataSourcesGenerator<FromDataSourceBasedConfiguration>
{
    protected override Data<object> ConvertDataSourceDataToGenerateData(Data<object> data, string dataSourceName) => data;
}