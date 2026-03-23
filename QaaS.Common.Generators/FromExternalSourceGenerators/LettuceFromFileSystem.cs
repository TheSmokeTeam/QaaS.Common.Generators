using System.Text;
using System.Text.Json.Nodes;
using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;
using QaaS.Framework.SDK.Session.MetaDataObjects;
using ArgumentException = System.ArgumentException;

namespace QaaS.Common.Generators.FromExternalSourceGenerators;

/// <summary>
/// Retrieves lettuce-formatted files from the configured file-system path and exposes them as generated messages with their routing key metadata.
/// </summary>
public class LettuceFromFileSystem : BaseFromFileSystem<LettuceFromFileSystemConfig>
{
    /// <summary>
    /// Gets Lettuce body content from  file content. Throws an ArgumentException if the file content cannot be parsed to a Json.
    /// </summary>
    /// <param name="fileContents">The file contents</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="metaData"></param>
    /// <returns>The lettuce body contents.</returns>
    protected override byte[] ProcessFileContents(byte[] fileContents, string fileName, ref MetaData metaData)
    {
        try
        {
            var jsonContent = JsonNode.Parse(Encoding.UTF8.GetString(fileContents));
            var routingKey  = jsonContent?[Constants.Lettuce.RoutingKeyFieldName]?.ToString();
            metaData = metaData with
            {
                RabbitMq = metaData.RabbitMq != null
                    ? metaData.RabbitMq with
                    {
                        RoutingKey = routingKey
                    }
                    : new RabbitMq
                    {
                        RoutingKey = routingKey
                    }
            };
            return Convert.FromBase64String(jsonContent![Constants.Lettuce.BodyFieldName]?.ToString() ?? "");
        }
        catch (Exception exception)
        {
            throw new ArgumentException($"The file {fileName} is not a valid lettuce file, encountered following exception: \n {exception}");
        }
    }
}
