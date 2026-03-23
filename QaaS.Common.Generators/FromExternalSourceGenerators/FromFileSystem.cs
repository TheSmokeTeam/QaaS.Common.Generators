using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;

namespace QaaS.Common.Generators.FromExternalSourceGenerators;

/// <summary>
/// Retrieves data from files under a configured path in the local file system.
/// </summary>
public class FromFileSystem : BaseFromFileSystem<FromFileSystemConfig>;
