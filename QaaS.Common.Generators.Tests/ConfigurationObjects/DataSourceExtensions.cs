using System.Reflection;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;


namespace QaaS.Common.Generators.Tests.ConfigurationObjects;

public static class DataSourceExtensions
{
        
    private static readonly FieldInfo GeneratedDataFieldInfo =
        typeof(DataSource).GetField("_generatedData", 
            BindingFlags.Instance | BindingFlags.NonPublic)!;
    
    public static DataSource SetGeneratedData(this DataSource dataSource, IList<Data<object>>? generatedData)
    {
        GeneratedDataFieldInfo.SetValue(dataSource,generatedData);
        return dataSource;
    }
}