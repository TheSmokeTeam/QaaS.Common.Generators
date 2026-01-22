using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;

public abstract record BaseFromDataSourcesConfiguration
{
    [Range(0 , int.MaxValue),
     Description("The number of items to generate out of the given data sources (If count is bigger than" +
                 " available number of items an exception will be thrown), " +
                 "if no value is given generates the number of items in the given data sources")]
    public int? Count { get; set; }
    
}


