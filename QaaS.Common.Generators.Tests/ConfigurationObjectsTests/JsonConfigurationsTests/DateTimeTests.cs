using System.Text.RegularExpressions;
using NUnit.Framework;
using DateTime = QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations.DateTime;

namespace QaaS.Common.Generators.Tests.ConfigurationObjectsTests.JsonConfigurationsTests;

public class DateTimeTests
{
    [Test]
    public void TestDateTimeConfig_CallGetMethodWithPlainConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new DateTime();
        var expectedResultRegex = @"^\b\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{7}Z$";
        
        // Act
        var resultDateTime = dateTimeConfig.GetTime() as string;
        
        // Assert
        Assert.That(Regex.IsMatch(resultDateTime!, expectedResultRegex), Is.True);
    }
    
    [Test]
    public void TestDateTimeConfig_CallGetMethodWithFormatAndTimezoneConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new DateTime
        {
            TimeZone = "Tokyo Standard Time",
            Format = "yyyy-MM-ddTHH:mm:ss.fffzzz"
        };
        var expectedResultRegex = @"^\b\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}\S*\d{2}:\d{2}\b$";
        
        // Act
        var resultDateTime = dateTimeConfig.GetTime() as string;
        
        // Assert
        Assert.That(Regex.IsMatch(resultDateTime!, expectedResultRegex), Is.True);
    }
    
    [Test]
    public void TestDateTimeConfig_CallGetMethodWithOffsetConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new DateTime
        {
            DayOffset = -30,
            HourOffset = 40,
            MinuteOffset = -90,
            SecondOffset = 120,
            MillisecondOffset = -2540
        };
        var expectedResultRegex = @"^\b\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{7}Z$";
        
        // Act
        var resultDateTime = dateTimeConfig.GetTime() as string;
        
        // Assert
        Assert.That(Regex.IsMatch(resultDateTime!, expectedResultRegex), Is.True);
    }
    
    [Test]
    public void TestDateTimeConfig_CallGetMethodWithFormatAndTimezoneAndOffsetConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new DateTime
        {
            TimeZone = "Tokyo Standard Time",
            Format = "yyyy-MM-ddTHH:mm:ss.fffzzz",
            DayOffset = -30,
            HourOffset = 40,
            MinuteOffset = -90,
            SecondOffset = 120,
            MillisecondOffset = -2540
        };
        var expectedResultRegex = @"^\b\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}\S*\d{2}:\d{2}\b$";
        
        // Act
        var resultDateTime = dateTimeConfig.GetTime() as string;
        
        // Assert
        Assert.That(Regex.IsMatch(resultDateTime!, expectedResultRegex), Is.True);
    }
    
    [Test]
    public void TestDateTimeConfig_CallGetMethodWithConstantsConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new DateTime
        {
            Year = 1969,
            Month = 11,
            Day = 13,
            Hour = 4,
            Minute = 20,
            Second = 30,
            Millisecond = 420
        };
        const string expectedResult = "1969-11-13T04:20:30.4200000Z";
        
        // Act
        var resultDateTime = dateTimeConfig.GetTime() as string;
        
        // Assert
        Assert.That(resultDateTime, Is.EqualTo(expectedResult));
    }
    
    [Test]
    public void TestDateTimeConfig_CallGetMethodWithConstantsAndOffsetConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new DateTime
        {
            Year = 1969,
            Month = 11,
            Day = 13,
            Hour = 4,
            Minute = 20,
            Second = 30,
            Millisecond = 420,
            DayOffset = -30,
            HourOffset = 40,
            MinuteOffset = -90,
            SecondOffset = 120,
            MillisecondOffset = -2540
        };
        const string expectedResult = "1969-10-15T18:52:27.8800000Z";
        
        // Act
        var resultDateTime = dateTimeConfig.GetTime() as string;
        
        // Assert
        Assert.That(resultDateTime, Is.EqualTo(expectedResult));
    }
    
    
    [Test]
    public void TestDateTimeConfig_CallGetMethodWithExtremeOffsetConfiguration_ShouldThrowAnException()
    {
        // Arrange
        var dateTimeConfig = new DateTime
        {
            DayOffset = 999999999
        };
        
        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => dateTimeConfig.GetTime());
    }
    
    [Test]
    public void TestDateTimeConfig_CallGetMethodWithUnknownTimezoneConfiguration_ShouldThrowAnException()
    {
        // Arrange
        var dateTimeConfig = new DateTime
        {
            TimeZone = "NotARealTimeZone"
        };
        
        // Act + Assert
        Assert.Throws<TimeZoneNotFoundException>(() => dateTimeConfig.GetTime());
    }
}
