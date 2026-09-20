using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Generators.FromDataSourcesGenerators;

/// <summary>
/// Generates data from the enumerable of data sources it receives
/// </summary>
/// <qaas-docs group="Existing data sources" subgroup="Direct reuse" />
public class FromDataSources : BaseFromDataSourcesGenerator<FromDataSourceBasedConfiguration>
{
    protected override IEnumerable<Data<object>> ConvertDataSourceDataToGeneratedItems(Data<object> data, string dataSourceName)
    {
        yield return data;
    }
}
