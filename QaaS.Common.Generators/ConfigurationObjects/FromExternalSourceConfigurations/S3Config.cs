using System.ComponentModel;

namespace QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;

public record S3Config : Framework.Configurations.CommonConfigurationObjects.S3BucketConfig
{
    [Description("Delimiter of the objects to extract from s3 bucket, this determines what objects will be retrieved from the bucket, " +
                 $"objects that have at least one occurence of the delimiter in their relative path after the `{nameof(Prefix)}` " +
                 "will not be retrieved from the bucket."), DefaultValue("")]
    public string Delimiter { get; set; } = "";
    
    [Description("The prefix of the objects, in the s3 bucket to take"), DefaultValue("")]
    public string Prefix { get; set; } = "";

    [Description(
         "Whether to skip the loading of empty s3 objects or not, if true skips them if false doesnt skip them"),
     DefaultValue(true)]
    public bool SkipEmptyObjects { get; set; } = true;
}