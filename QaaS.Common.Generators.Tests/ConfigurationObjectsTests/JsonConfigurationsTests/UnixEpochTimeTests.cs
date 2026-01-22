using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

namespace QaaS.Common.Generators.Tests.ConfigurationObjectsTests.JsonConfigurationsTests;

public class UnixEpochTimeTests
{
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithPlainConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime();
        
        // Act + Assert
        Assert.That(dateTimeConfig.GetTime() is long);
    }
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithAsStringConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime { UnixEpochObjectType = UnixEpochObjectType.String };
        
        // Act + Assert
        Assert.That(dateTimeConfig.GetTime() is string);
    }
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithAsMillisecondsConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime
        {
            UnixEpochScaleType = UnixEpochScaleType.Milliseconds
        };
        
        // Act + Assert
        Assert.That(dateTimeConfig.GetTime() is long);
    }
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithAsStringMillisecondsConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime
        {
            UnixEpochObjectType = UnixEpochObjectType.String,
            UnixEpochScaleType = UnixEpochScaleType.Milliseconds
        };
        
        // Act + Assert
        Assert.That(dateTimeConfig.GetTime() is string);
    }
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithAsStringMillisecondsWithOffsetConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime
        {
            UnixEpochObjectType = UnixEpochObjectType.String,
            UnixEpochScaleType = UnixEpochScaleType.Milliseconds,
            DayOffset = -30,
            HourOffset = 40,
            MinuteOffset = -90,
            SecondOffset = 120,
            MillisecondOffset = -2540
        };
        
        // Act + Assert
        Assert.That(dateTimeConfig.GetTime() is string);
    }
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithConstantsConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime
        {
            Year = 1969,
            Month = 11,
            Day = 13,
            Hour = 4,
            Minute = 20,
            Second = 30,
            Millisecond = 420
        };
        const long expectedResult = -4217970;
        
        // Act
        var resultDateTime = (long) dateTimeConfig.GetTime();
        
        // Assert
        Assert.That(resultDateTime, Is.EqualTo(expectedResult));
    }
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithConstantsWithMillisecondsConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime
        {
            UnixEpochScaleType = UnixEpochScaleType.Milliseconds,
            Year = 1969,
            Month = 11,
            Day = 13,
            Hour = 4,
            Minute = 20,
            Second = 30,
            Millisecond = 420
        };
        const long expectedResult = -4217969580;
        
        // Act
        var resultDateTime = (long) dateTimeConfig.GetTime();
        
        // Assert
        Assert.That(resultDateTime, Is.EqualTo(expectedResult));
    }
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithConstantsAsStringConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime
        {
            Year = 1969,
            Month = 11,
            Day = 13,
            Hour = 4,
            Minute = 20,
            Second = 30,
            Millisecond = 420,
            UnixEpochObjectType = UnixEpochObjectType.String
        };
        const string expectedResult = "-4217970";
        
        // Act
        var resultDateTime = (string) dateTimeConfig.GetTime();
        
        // Assert
        Assert.That(resultDateTime, Is.EqualTo(expectedResult));
    }
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithConstantsWithMillisecondsAsStringConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime
        {
            UnixEpochObjectType = UnixEpochObjectType.String,
            UnixEpochScaleType = UnixEpochScaleType.Milliseconds,
            Year = 1969,
            Month = 11,
            Day = 13,
            Hour = 4,
            Minute = 20,
            Second = 30,
            Millisecond = 420
        };
        const string expectedResult = "-4217969580";
        
        // Act
        var resultDateTime = (string) dateTimeConfig.GetTime();
        
        // Assert
        Assert.That(resultDateTime, Is.EqualTo(expectedResult));
    }
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithConstantsAndOffsetConfiguration_ShouldReturnDateTimeCorrectly()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime
        {
            UnixEpochObjectType = UnixEpochObjectType.String,
            UnixEpochScaleType = UnixEpochScaleType.Milliseconds,
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
        const string expectedResult = "-6671252120";
        
        // Act
        var resultDateTime = dateTimeConfig.GetTime() as string;
        
        // Assert
        Assert.That(resultDateTime, Is.EqualTo(expectedResult));
    }
    
    
    [Test]
    public void TestUnixEpochTimeConfig_CallGetMethodWithExtremeOffsetConfiguration_ShouldThrowAnException()
    {
        // Arrange
        var dateTimeConfig = new UnixEpochTime
        {
            DayOffset = 999999999
        };
        
        // Act + Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => dateTimeConfig.GetTime());
    }
}