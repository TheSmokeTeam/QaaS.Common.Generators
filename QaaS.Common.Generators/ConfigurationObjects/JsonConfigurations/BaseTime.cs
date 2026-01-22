using System.ComponentModel;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

/// <summary>
/// Base class for time configuration.
/// </summary>
public abstract record BaseTime
{
    [Description("Constant Year of the date time")]
    public int? Year { get; set; } = null;

    [Description("Constant Month of the date time")]
    public int? Month { get; set; } = null;

    [Description("Constant Day of the date time")]
    public int? Day { get; set; } = null;

    [Description("Day offset from the date time"), DefaultValue(0)]
    public int DayOffset { get; set; } = 0;

    [Description("Constant Hour of the date time")]
    public int? Hour { get; set; } = null;

    [Description("Hour offset from the date time"), DefaultValue(0)]
    public int HourOffset { get; set; } = 0;

    [Description("Constant Minute of the date time")]
    public int? Minute { get; set; } = null;

    [Description("Minute offset from the date time"), DefaultValue(0)]
    public int MinuteOffset { get; set; } = 0;

    [Description("Constant Second of the date time")]
    public int? Second { get; set; } = null;

    [Description("Second offset from the date time"), DefaultValue(0)]
    public int SecondOffset { get; set; } = 0;

    [Description("Constant Millisecond of the date time")]
    public int? Millisecond { get; set; } = null;

    [Description("Millisecond offset from the date time"), DefaultValue(0)]
    public int MillisecondOffset { get; set; } = 0;

    /// <summary>
    /// Get current date time (UTC), and changes it by given configuration.
    /// </summary>
    /// <returns>Object of time implement by the child</returns>
    public object GetTime()
    {
        var dateTimeOffset = new DateTimeOffset(new System.DateTime(
            Year ?? System.DateTime.UtcNow.Year,
            Month ?? System.DateTime.UtcNow.Month,
            Day ?? System.DateTime.UtcNow.Day,
            Hour ?? System.DateTime.UtcNow.Hour,
            Minute ?? System.DateTime.UtcNow.Minute,
            Second ?? System.DateTime.UtcNow.Second,
            Millisecond ?? System.DateTime.UtcNow.Millisecond,
            kind: DateTimeKind.Utc
        ));

        var timespan = new TimeSpan(
            DayOffset,
            HourOffset,
            MinuteOffset,
            SecondOffset,
            MillisecondOffset
        );

        var dateTime = dateTimeOffset.Add(timespan).UtcDateTime;

        return GetSpecificObjectTime(dateTime);
    }

    /// <summary>
    /// Adjusts the date time to relevant to the child class.
    /// </summary>
    /// <param name="dateTime">Date time to adjust</param>
    /// <returns>Date time of specific object</returns>
    protected abstract object GetSpecificObjectTime(System.DateTime dateTime);
}