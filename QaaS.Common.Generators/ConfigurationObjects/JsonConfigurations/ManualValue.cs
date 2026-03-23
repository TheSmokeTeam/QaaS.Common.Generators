using System.ComponentModel;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

/// <summary>
/// Class for manual value configuration.
/// </summary>
/// <typeparam name="T"> Type of value. </typeparam>
public record ManualValue<T>
{
    [Description("The literal value to inject when this manual value configuration is selected.")]
    public T Value { get; set; }
}
