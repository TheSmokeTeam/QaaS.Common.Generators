using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Common.Generators.FromDataLakeGenerator;

namespace QaaS.Common.Generators.ConfigurationObjects.FromDataLakeConfigurations;

[Description("Supports generating data from data lake. " +
             "`DataSources`: Not used. `SessionData`: Not used."),
 Display(Name = nameof(FromDataLake))]
public record FromDataLakeConfiguration
{
    [DefaultValue("http://localhost:8080"), Description("The trino server to connnect to")]
    public string TrinoServerUri { get; set; } = "http://localhost:8080";
    
    [Required, Description("The username for connecting to the trino server")]
    public string? Username { get; set; }
    
    [Required, Description("The password for connecting to the trino server")]
    public string? Password { get; set; }
    
    [DefaultValue("qaas"), Description("The client tag to use for connection")]
    public string ClientTag { get; set; } = "qaas";
    
    [DefaultValue("hive"), Description("The datalake catalog to query")]
    public string Catalog { get; set; } = "hive";
    
    [Required, Description("The query to execute against the datalake")]
    public string? Query { get; set; }
    
    [Description("The columns to ignore in the query results, if no columns are given doesn't ignore any columns"),
     DefaultValue(new string[]{})]
    public string[] ColumnsToIgnore { get; set; } = Array.Empty<string>();
}