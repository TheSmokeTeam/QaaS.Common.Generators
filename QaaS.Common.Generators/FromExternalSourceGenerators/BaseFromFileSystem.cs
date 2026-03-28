using System.IO.Abstractions;
using QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Generators.FromExternalSourceGenerators;

public abstract class BaseFromFileSystem<TFromFileSystemConfig>: BaseExternalSourceBasedGenerator<TFromFileSystemConfig, string>
    where TFromFileSystemConfig: FromFileSystemConfig, new()
{
    private IFileSystem? _fileSystem;
    private string? _currentDirectory;

     protected override void CreateDesiredResources()
    {
        (_currentDirectory, _fileSystem) = BuildFileSystem();
    }

    protected override IEnumerable<KeyValuePair<string, string>> LoadObjectsMetadata()
    {
        var fileSystemConfiguration = RequireFileSystemConfiguration();
        var fullPath = Path.Combine(_currentDirectory!, fileSystemConfiguration.Path!);
        return _fileSystem!.Directory.GetFiles(fullPath, fileSystemConfiguration.SearchPattern,
                SearchOption.AllDirectories)
            .Select(fileName => new KeyValuePair<string, string>(fileName, fileName));
    }

    protected override SerializedLoadedData LoadData(KeyValuePair<string, string> objectProperties)
    {
        var fileMetaData = new MetaData();
        return new SerializedLoadedData
        {
            FullKey = objectProperties.Key,
            Content = ProcessFileContents(_fileSystem!.File.ReadAllBytes(objectProperties.Key), objectProperties.Key, ref fileMetaData),
            MetaData = fileMetaData
        };
    }

    protected override string? GetStorageKeyFromData(string key) =>
        Configuration.StorageMetaData switch
        {
            StorageMetaData.RelativePath => Path.GetRelativePath(RequireFileSystemConfiguration().Path!, key),
            StorageMetaData.ItemName => Path.GetFileName(key),
            StorageMetaData.FullPath => key,
            StorageMetaData.None => null,
            _ => throw new NotSupportedException($"{nameof(StorageMetaData)}" +
                                                 $" {Configuration.StorageMetaData} not supported")
        };

    private FileSystemConfig RequireFileSystemConfiguration()
    {
        if (Configuration.FileSystem == null)
        {
            throw new ArgumentException(
                $"{GetType().Name} requires GeneratorConfiguration.FileSystem to be configured.");
        }

        if (string.IsNullOrWhiteSpace(Configuration.FileSystem.Path))
        {
            throw new ArgumentException(
                $"{GetType().Name} requires GeneratorConfiguration.FileSystem.Path to be configured.");
        }

        return Configuration.FileSystem;
    }

    /// <summary>
    /// Process the file contents
    /// </summary>
    /// <param name="fileContents">the contents of the file</param>
    /// <param name="fileName">the name of the file</param>
    /// <param name="metaData">the metadata object that will be attached with the file contents</param>
    /// <returns>the contents of the file after they have been processed</returns>
    protected virtual byte[] ProcessFileContents(byte[] fileContents, string fileName, ref MetaData metaData) =>
        fileContents;


    /// <summary>
    /// Builds a file system dictionary containing the current directory string as key and a file system object as value
    /// </summary>
    /// <returns>Dictionary describing the filesystem</returns>
    protected virtual KeyValuePair<string, IFileSystem> BuildFileSystem() =>
        new(Environment.CurrentDirectory, new FileSystem());

}
