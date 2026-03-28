using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Common.Generators.FromExternalSourceGenerators;

namespace QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;

[Description("Retrieves data from a configured path to a directory in the file system. " +
             "`DataSources`: Not used. `SessionData`: Not used."),
 Display(Name = nameof(FromFileSystem))]
public record FromFileSystemConfig : BaseExternalSourceBasedGeneratorConfig
{
    [Required(ErrorMessage = "GeneratorConfiguration:FileSystem is required for FromFileSystem generators."),
     Description("File system directory configuration")]
    public FileSystemConfig? FileSystem { get; set; }
}
