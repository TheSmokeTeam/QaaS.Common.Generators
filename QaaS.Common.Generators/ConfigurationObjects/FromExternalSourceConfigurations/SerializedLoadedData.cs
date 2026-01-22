
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;

/// <summary>
/// A structure representing a single loaded serialized data item from an external source
/// </summary>
public class SerializedLoadedData
{
    public byte[] Content { get; init; }
    public MetaData? MetaData { get; init; }
    public string FullKey { get; init; }
}