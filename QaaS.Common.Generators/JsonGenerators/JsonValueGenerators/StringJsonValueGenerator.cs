using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using QaaS.Common.Generators.JsonGenerators.JsonExtensions;
using DateTime = System.DateTime;

namespace QaaS.Common.Generators.JsonGenerators.JsonValueGenerators;

/// <inheritdoc />
public class StringJsonValueGenerator : BaseJsonValueGenerator
{
    public const uint DefaultStringMinimumLength = 0;
    public const uint DefaultStringMaximumLength = 69;
    private const string AsciiChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const string AsciiDigits = "0123456789";
    private const int BytesInIPv4 = 4;
    private const int BytesInIPv6 = 16;
    private const int SecondsInADay = 86400;
    private static readonly DateTime StartDate = new (1970, 1, 1);
    
    private Random Random { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringJsonValueGenerator"/> class.
    /// </summary>
    /// <param name="seed">The seed for the random number generator.</param>
    public StringJsonValueGenerator(int seed)
    {
        Random = new Random(seed);
    }
    
    /// <inheritdoc /> 
    protected override object GenerateRawValue(JsonObject jsonSchemaObject)
    {
        var format = jsonSchemaObject.GetJsonSchemaStringFormat();
        return format != null ? 
            ResolveFormatGeneration(format) : 
            GenerateValueRandomUtf8StringExpression(jsonSchemaObject);
    }

    /// <summary>
    /// Generates a random JSON String value of utf-8 string expression based on the given JSON schema object.
    /// </summary>
    /// <param name="jsonSchemaObject">The JSON schema object to generate a value for.</param>
    /// <returns>A random JSON value of utf-8 string expression.</returns>
    private string GenerateValueRandomUtf8StringExpression(JsonObject jsonSchemaObject)
    {
        var minLength = (int) (jsonSchemaObject.GetJsonSchemaStringMinLength() ?? DefaultStringMinimumLength);
        var maxLength = (int) (jsonSchemaObject.GetJsonSchemaStringMaxLength() ?? DefaultStringMaximumLength);
        
        var length = Random.Next(minLength, maxLength + 1);
        var stringBuilder = new StringBuilder(length);

        for (var characterIndex = 0; characterIndex < length; characterIndex++)
        {
            var character = (char) Random.Next(32, 127);
            stringBuilder.Append(character);
        }

        return stringBuilder.ToString();
    }
    
    /// <summary>
    /// Generates a random JSON String value based on the given format.
    /// </summary>
    /// <param name="format">The format to generate a value for.</param>
    /// <returns>A random JSON String value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported format is provided.</exception>
    private string ResolveFormatGeneration(string? format)
    {
        return format switch 
        {
            "uuid" => GenerateUuid(),
            "date-time" => GenerateDateTime(),
            "date" => GenerateDate(),
            "time" => GenerateTime(),
            "email" => GenerateEmail(),
            "hostname" => GenerateHostname(),
            "ipv4" => GenerateIpv4(),
            "ipv6" => GenerateIpv6(),
            _ => throw new ArgumentException("Json Schema String 'Format' type generation not supported", nameof(format))
        };
    }

    /// <summary>
    /// Generates a random UUID.
    /// </summary>
    /// <returns>A random UUID.</returns>
    private string GenerateUuid()
    {
        return Guid.NewGuid().ToString();
    }
    
    /// <summary>
    /// Generates a random date.
    /// </summary>
    /// <returns>A random date.</returns>
    private string GenerateDate()
    {
        var rangeOfDays = (DateTime.Today - StartDate).Days;
        var generatedDate = StartDate.AddDays(Random.Next(rangeOfDays));
        return $"{generatedDate:yyyy-MM-dd}";
    }

    /// <summary>
    /// Generates a random time.
    /// </summary>
    /// <returns>A random time.</returns>
    private string GenerateTime()
    {
        var generatedTimeInSeconds = Random.Next(SecondsInADay);
        var generatedTime = TimeSpan.FromSeconds(generatedTimeInSeconds).ToString("c");
        return $"{generatedTime}+00:00";
}
    
    /// <summary>
    /// Generates a random date and time.
    /// </summary>
    /// <returns>A random date and time.</returns>
    private string GenerateDateTime()
    {
        var generatedDate = GenerateDate();
        var generatedTime = GenerateTime();
        return $"{generatedDate}" + "T" + $"{generatedTime}";
    }
    
    /// <summary>
    /// Generates a random email address.
    /// </summary>
    /// <returns>A random email address.</returns>
    private string GenerateEmail()
    {
        var nameEmailSection = RandomAsciiString(Random.Next(1, 65), true);
        var domainEmailSection = RandomAsciiString(Random.Next(1, 10)) + "." + RandomAsciiString(3);
        var generatedEmail = $"{nameEmailSection}@{domainEmailSection}";
        return generatedEmail;
    }
    
    /// <summary>
    /// Generates a random hostname.
    /// </summary>
    /// <returns>A random hostname.</returns>
    private string GenerateHostname()
    {
        return $"{RandomAsciiString(Random.Next(1, 9))}-{Random.Next(1, 9)}-{Random.Next(1, 9)}";
    }

    /// <summary>
    /// Generates a random IPv4 address.
    /// </summary>
    /// <returns>A random IPv4 address.</returns>
    private string GenerateIpv4()
    {
        var bytes = new byte[BytesInIPv4];
        Random.NextBytes(bytes);
        return new IPAddress(bytes).ToString();
    }
    
    /// <summary>
    /// Generates a random IPv6 address.
    /// </summary>
    /// <returns>A random IPv6 address.</returns>
    private string GenerateIpv6()
    {
        var bytes = new byte[BytesInIPv6];
        Random.NextBytes(bytes);
        return new IPAddress(bytes).ToString();
    }
    
    /// <summary>
    /// Generates a random ASCII string of the specified length.
    /// </summary>
    /// <param name="length">The length of the string to generate.</param>
    /// <param name="includeDigits">Whether to include digits in the string.</param>
    /// <returns>A random ASCII string.</returns>
    private string RandomAsciiString(int length, bool includeDigits = false)
    {
        var charPool = includeDigits ? AsciiChars + AsciiDigits : AsciiChars;
        var randomString = new StringBuilder();
        
        for (var characterIndex = 0; characterIndex < length; characterIndex++)
        {
            var randomCharPoolIndex = Random.Next(charPool.Length);
            var randomChar = charPool[randomCharPoolIndex];
            randomString.Append(randomChar);
        }
        
        return randomString.ToString();
    }
}