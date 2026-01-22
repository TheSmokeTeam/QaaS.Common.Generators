using System.Collections.Immutable;
using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;
using QaaS.Common.Generators.JsonGenerators.JsonNodeGenerators;
using QaaS.Common.Generators.JsonGenerators.JsonParsers;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Generators.JsonGenerators;

public abstract class BaseJsonGenerator<TJsonConfiguration>: BaseGenerator<TJsonConfiguration>
    where TJsonConfiguration: JsonConfiguration, new()
{
    /// <summary>
    /// Constructs a new instance of <see cref="IJsonNodeGenerator"/> with the provided <see cref="JsonNode"/>.
    /// </summary>
    /// <param name="jsonNode">The <see cref="JsonNode"/> to use for constructing the <see cref="IJsonNodeGenerator"/>.</param>
    /// <returns>A new instance of <see cref="IJsonNodeGenerator"/>.</returns>
    protected abstract IJsonNodeGenerator ConstructJsonNodeGenerator(JsonNode jsonNode);

    /// <inheritdoc />
    public override IEnumerable<Data<object>> Generate(
        IImmutableList<SessionData> sessionDataList,
        IImmutableList<DataSource> dataSourceList)
    {
        
        var baseGenerationJson = JsonNodeExtensions.GetJsonNodeFromDataSource(sessionDataList,dataSourceList,Configuration.JsonDataSourceName!);
        
        // Removing JsonDataSource from DataSource list
        dataSourceList = dataSourceList.Where(dataSource => dataSource.Name != Configuration.JsonDataSourceName!).ToImmutableList();
        
        var generationJsonGenerator = ConstructJsonNodeGenerator(baseGenerationJson);
        
        var jsonParserToObject = Configuration.OutputObjectType != JsonParserType.Json
            ? JsonParserFactory.GetInstance().GetJsonParser(Configuration.OutputObjectType, Configuration.OutputObjectTypeConfiguration)
            : null;

        JsonNodeFieldInjector? jsonNodeFieldInjector = null;
        if (Configuration.JsonFieldReplacements.Any())
        {
            var generationsEnumerableDictionary = dataSourceList.ToDictionary(generation => generation.Name,
                generation => new GenerationEnumerable(generation.Retrieve(sessionDataList)));
            jsonNodeFieldInjector = new JsonNodeFieldInjector(generationsEnumerableDictionary, 
                Configuration.JsonFieldReplacements);
        }
        
        for (var generationIndex = 0; generationIndex < Configuration.Count; generationIndex++)
        {
            var generationJson = generationJsonGenerator.Generate();

            jsonNodeFieldInjector?.ReplaceFields(generationJson);

            var generationObject = jsonParserToObject != null ?
                jsonParserToObject.Parse(generationJson) : generationJson;
            
            yield return new Data<object>
            {
                Body = generationObject
            };
        }
    }
}