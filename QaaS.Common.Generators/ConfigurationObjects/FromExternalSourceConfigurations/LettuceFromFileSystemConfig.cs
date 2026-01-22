using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Common.Generators.FromExternalSourceGenerators;

namespace QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;

[Description("Retrieves lettuce data from a configured path to a directory in the file system. " +
             "`DataSources`: Not used. `SessionData`: Not used."),
 Display(Name = nameof(LettuceFromFileSystem))]
public record LettuceFromFileSystemConfig : FromFileSystemConfig;