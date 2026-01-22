using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations.SessionDataConfigurations;

public record CommunicationDataName
{
    [Required, Description("The type of the communication data to generate. " +
                           "Options: " +
                           "[ `Input` -  an input communication data from the Inputs list in the SessionData structure" +
                           " / `Output` - an output communication data from the Outputs list in the SessionData structure ]")]
    public CommunicationDataType? Type { get; set; }
    
    [Required, Description("The name of the communication data in the session data to generate")]
    public string? Name { get; set; }
}