using Amazon.S3;
using Amazon.S3.Model;
using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;
using QaaS.Framework.Protocols.Utils.S3Utils;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Generators.FromExternalSourceGenerators;

/// <summary>
/// Retrieves data from objects in a configured S3 bucket and prefix.
/// </summary>
public class FromS3 : BaseExternalSourceBasedGenerator<FromS3Config, KeyValuePair<S3Object, byte[]>>
{
    private const char S3SeparatorChar = '/';
    private IS3Client? _s3Client;

    protected override void CreateDesiredResources()
    {
        _s3Client = BuildS3Client();
    }

    protected override IEnumerable<KeyValuePair<string, KeyValuePair<S3Object, byte[]>>> LoadObjectsMetadata() =>
        Configuration.LoadMetadataFirst
            ? _s3Client!.ListAllObjectsInS3Bucket(Configuration.S3!.StorageBucket!,
                    Configuration.S3.Prefix,
                    Configuration.S3.Delimiter, Configuration.S3.SkipEmptyObjects).GetAwaiter().GetResult()
                .Select(s3Object =>
                    new KeyValuePair<string, KeyValuePair<S3Object, byte[]>>(s3Object.Key,
                        new KeyValuePair<S3Object, byte[]>(s3Object, [])))
            : _s3Client!.GetAllObjectsInS3BucketUnOrdered(Configuration.S3!.StorageBucket!,
                    Configuration.S3.Prefix,
                    Configuration.S3.Delimiter, Configuration.S3.SkipEmptyObjects)
                .Select(s3Object => new KeyValuePair<string, KeyValuePair<S3Object, byte[]>>(s3Object.Key.Key,
                    new KeyValuePair<S3Object, byte[]>(s3Object.Key, s3Object.Value!)));


    protected override SerializedLoadedData LoadData(
        KeyValuePair<string, KeyValuePair<S3Object, byte[]>> objectProperties) =>
        new()
        {
            FullKey = objectProperties.Key,
            Content = Configuration.LoadMetadataFirst
                ? _s3Client!.GetObjectFromObjectMetadata(objectProperties.Value.Key,
                    Configuration.S3!.StorageBucket!).Value ?? []
                : objectProperties.Value.Value,
            MetaData = new MetaData()
        };

    protected override void DisposeResources()
    {
        _s3Client?.Dispose();
    }

    protected override string? GetStorageKeyFromData(string key) =>
        Configuration.StorageMetaData switch
        {
            StorageMetaData.RelativePath => key[Configuration.S3!.Prefix.Length..],
            StorageMetaData.ItemName => key.Split(S3SeparatorChar).Last(),
            StorageMetaData.FullPath => key,
            StorageMetaData.None => null,
            _ => throw new NotSupportedException($"{nameof(StorageMetaData)}" +
                                                 $" {Configuration.StorageMetaData} not supported")
        };

    protected virtual IS3Client BuildS3Client() =>
        new S3Client(new AmazonS3Client(Configuration.S3!.AccessKey, Configuration.S3.SecretKey, new AmazonS3Config
        {
            ServiceURL = Configuration.S3.ServiceURL,
            ForcePathStyle = Configuration.S3.ForcePathStyle
        }), logger: Context.Logger);
}
