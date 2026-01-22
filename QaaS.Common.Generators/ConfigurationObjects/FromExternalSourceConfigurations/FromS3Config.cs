using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Common.Generators.FromExternalSourceGenerators;

namespace QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;

[Description("Retrieves data from a configured S3 bucket. " +
             "`DataSources`: Not used. `SessionData`: Not used."),
 Display(Name = nameof(FromS3))]
public record FromS3Config : BaseExternalSourceBasedGeneratorConfig
{
    [Description(
         "Wheather to load lightweight S3 metadata first and then load objects sequentially (true) or load all objects parallelly (false)"),
     DefaultValue(true)]
    public bool LoadMetadataFirst { get; set; } = true;

    [Required, Description("S3 configurations")]
    public S3Config? S3 { get; set; }
}