using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

[Description("Supports generating data from Json data sources. `DataSources`: " +
             "Used, should only be a single one and must contain data deserialized into " +
             $"{nameof(SerializationType.Json)} ({nameof(JsonNode)})." +
             " `SessionData`: Passed to used DataSources."),
 Display(Name = nameof(JsonGenerators.Json))]
public record JsonConfiguration
{
    [Required, Range(0 , int.MaxValue), Description("The Number of generations")]
    public int? Count { get; set; }
    
    [Description("Field injections to generation's Json")]
    public IList<JsonFieldReplacement> JsonFieldReplacements { get; set; } 
        = new List<JsonFieldReplacement>();

    [Description("Output type of generation's Json, parsing the result to a specific type")]
    public JsonParserType OutputObjectType { get; set; } = JsonParserType.Json;
    
    [Description("Output type of generation's Json configuration")]
    public SpecificTypeConfig? OutputObjectTypeConfiguration { get; set; } = null;

    [Required, Description("DataSource name that contains json")]
    public string? JsonDataSourceName { get; set; }

}