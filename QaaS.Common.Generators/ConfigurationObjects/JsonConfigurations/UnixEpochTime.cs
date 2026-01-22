using System.ComponentModel;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

/// <summary>
/// Represents Unix Epoch Time configuration.
/// </summary>
public record UnixEpochTime : BaseTime
{
    [Description("Epoch Unix Time Scale Representation"), DefaultValue(UnixEpochScaleType.Seconds)]
    public UnixEpochScaleType UnixEpochScaleType { get; set; } = UnixEpochScaleType.Seconds;
    
    [Description("Epoch Unix Time Object Representation"), DefaultValue(UnixEpochObjectType.Long)]
    public UnixEpochObjectType UnixEpochObjectType { get; set; } = UnixEpochObjectType.Long;
    
    /// <summary>
    /// Default instance of the DateTime.
    /// </summary>
    public static readonly UnixEpochTime DefaultInstance = new ();
    
    /// <summary>
    /// Adjusts to Unix Epoch representation of DateTime.
    /// </summary>
    /// <param name="dateTime">Date time to adjust</param>
    /// <returns>Unix Epoch Date time (long or string)</returns>
    protected override object GetSpecificObjectTime(System.DateTime dateTime)
    {
        DateTimeOffset dateTimeOffsetEpoch;
        
        try
        {
            dateTimeOffsetEpoch = new DateTimeOffset(dateTime);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentOutOfRangeException($"DateTime '{dateTime:o}' is out of Unix Epoch range");
        }

        var returnedEpochTime = GetUnixEpochFromScale(dateTimeOffsetEpoch, UnixEpochScaleType);
        return GetUnixEpochObject(returnedEpochTime, UnixEpochObjectType);
    }

    /// <summary>
    /// Gets Unix Epoch from scale type.
    /// </summary>
    private static long GetUnixEpochFromScale(DateTimeOffset dateTimeOffsetEpoch, UnixEpochScaleType scaleType)
    {
        return scaleType switch
        {
            UnixEpochScaleType.Seconds => dateTimeOffsetEpoch.ToUnixTimeSeconds(),
            UnixEpochScaleType.Milliseconds => dateTimeOffsetEpoch.ToUnixTimeMilliseconds(),
            _ => throw new ArgumentOutOfRangeException(nameof(scaleType), scaleType, "Unsupported Unix Epoch Scale Type")
        };
    }
    
    /// <summary>
    /// Gets Unix Epoch object.
    /// </summary>
    private static object GetUnixEpochObject(long epochTime, UnixEpochObjectType objectType)
    {
        return objectType switch
        {
            UnixEpochObjectType.Long => epochTime,
            UnixEpochObjectType.String => epochTime.ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(objectType), objectType, "Unsupported Unix Epoch Object Type")
        };
    }
}