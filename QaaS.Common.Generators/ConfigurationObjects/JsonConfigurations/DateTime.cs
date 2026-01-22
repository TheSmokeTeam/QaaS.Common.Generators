using System.ComponentModel;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

/// <summary>
/// Represents a configuration object for a DateTime.
/// </summary>
public record DateTime : BaseTime
{
    [Description("Timezone of the date time (default UTC)")]
    public string? TimeZone { get; set; } = null;

    [Description("Format of the date time (default ISO 8601 without Tim Zone)"), DefaultValue("o")]
    public string Format { get; set; } = "o";
    
    /// <summary>
    /// Default instance of the DateTime.
    /// </summary>
    public static readonly DateTime DefaultInstance = new ();

    /// <summary>
    /// Adjusts to String representation of DateTime with format and timezone.
    /// </summary>
    /// <param name="dateTime">Date time to adjust</param>
    /// <returns>String Date time</returns>
    protected override object GetSpecificObjectTime(System.DateTime dateTime)
    {
        if (TimeZone != null)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
            dateTime = TimeZoneInfo.ConvertTime(dateTime, timeZoneInfo);
        }
        
        return dateTime.ToString(Format);
    }
}