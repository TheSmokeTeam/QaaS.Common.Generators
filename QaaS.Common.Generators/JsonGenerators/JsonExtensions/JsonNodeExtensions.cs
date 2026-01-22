using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Json.Path;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Generators.JsonGenerators.JsonExtensions;

/// <summary>
/// Extensions for <see cref="JsonNode"/>
/// </summary>
public static class JsonNodeExtensions
{

    /// <summary>
    /// Extracts a JsonNode from the first item in the data source list.
    /// </summary>
    /// <param name="sessionDataList">The list of session datas</param>
    /// <param name="dataSourceList">The list of data sources.</param>
    /// <param name="jsonDataSourceName">The name of the data source containing json</param>
    /// <returns>The JsonNode extracted from the first item in the list.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the list is null or empty, or when the first item in the list is not a JsonNode.
    /// </exception>
    public static JsonNode GetJsonNodeFromDataSource(IImmutableList<SessionData> sessionDataList,
        IImmutableList<DataSource> dataSourceList, string jsonDataSourceName)
    {
        var jsonDataSource = dataSourceList.FirstOrDefault(dataSource => dataSource.Name == jsonDataSourceName);
        if ( jsonDataSource == null)
            throw new ArgumentException($"Json Data Source name {jsonDataSourceName} was not found in dataSources list");
        

        var dataSourceOfJson = jsonDataSource.Retrieve(sessionDataList).ToList();
        if (dataSourceOfJson == null || !dataSourceOfJson.Any() || dataSourceOfJson.Count() > 1)
            throw new ArgumentException($"Json Data Source doesn't contain the expected single object of type Json");


        if (dataSourceOfJson.First().Body is not JsonNode json)
            throw new ArgumentException($"Data Source {jsonDataSource.Name} " +
                                        $"contains an item not deserialized into a Json.");
        return json;
    }

    /// <summary>
    /// Replace a value in a JsonNode using a json path.
    /// </summary>
    /// <param name="jsonNode">The json node to replace the value in.</param>
    /// <param name="jsonFieldPath">The json path to the value to replace.</param>
    /// <param name="value">The value to replace the value in the json node with.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if no matches are found for the json path.</exception>
    public static void Replace(this JsonNode jsonNode, string jsonFieldPath, object? value)
    {
        var jsonPathObject = JsonPath.Parse(jsonFieldPath);
        var jsonPathMatch = jsonPathObject.Evaluate(jsonNode).Matches!.FirstOrDefault() ??
                            throw new ArgumentException($"Field not found for JSONPath given: {jsonFieldPath}");
        jsonPathMatch.Value!.ReplaceWith(value);
    }
}