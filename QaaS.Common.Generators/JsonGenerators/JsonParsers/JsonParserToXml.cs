using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace QaaS.Common.Generators.JsonGenerators.JsonParsers;

/// <inheritdoc />
public class JsonParserToXml : IJsonParser
{
    /// <inheritdoc />
    public object Parse(JsonNode jsonNode)
    {
        /*
         * DOESN'T INCLUDE ANY ADDED XDeclaration OR ANY ADDED XML FEATURES
         */
        return JsonConvert.DeserializeXNode(jsonNode.ToJsonString())!;
    }
}