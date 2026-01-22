namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

/// <summary>
/// Class for manual value configuration.
/// </summary>
/// <typeparam name="T"> Type of value. </typeparam>
public record ManualValue<T>
{
    public T Value { get; set; }
}