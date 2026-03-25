using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using QaaS.Framework.Configurations.CustomValidationAttributes;

namespace QaaS.Common.Generators.ConfigurationObjects.JsonConfigurations;

public record JsonFieldReplacement
{
    [Required, Description("The path (JSONPath query language Feb 2024, by Stefan Gossner) of the field to inject to")]
    public string Path { get; set; } = string.Empty;
    
    [Description("The type of static field injection to use")]
    public InjectionValueType ValueType { get; set; } = InjectionValueType.Null;

    [RequiredIfAny(nameof(ValueType), InjectionValueType.FromDataSource),
     Description("The generation field configuration")]
    public FromDataSource? FromDataSource { get; set; }

    [Description("The date-time field configuration")]
    public DateTime DateTime { get; set; } = DateTime.DefaultInstance;
    
    [Description("The unix epoch time field configuration")]
    public UnixEpochTime UnixEpochTime { get; set; } = UnixEpochTime.DefaultInstance;
    
    [RequiredIfAny(nameof(ValueType), InjectionValueType.String),
     Description("The value of the field if the type is 'String'")]
    public ManualValue<string>? String { get; set; }
    
    [RequiredIfAny(nameof(ValueType), InjectionValueType.Integer),
     Description("The value of the field if the type is 'Integer'")]
    public ManualValue<int>? Integer { get; set; }
    
    [RequiredIfAny(nameof(ValueType), InjectionValueType.Double),
      Description("The value of the field if the type is 'Double'")]
    public ManualValue<double>? Double { get; set; }
    
    [RequiredIfAny(nameof(ValueType), InjectionValueType.Boolean),
     Description("The value of the field if the type is 'Boolean'")]
    public ManualValue<bool>? Boolean { get; set; }
    
    [RequiredIfAny(nameof(ValueType), InjectionValueType.ByteArray),
     Description("The value of the field if the type is 'ByteArray'")]
    public ManualValue<byte[]>? ByteArray { get; set; }
    
    public object? GetManualValue() => ValueType switch
    { 
         InjectionValueType.String => String?.Value ?? throw new ArgumentNullException(nameof(String), "Value of String is required"),
         InjectionValueType.Integer => Integer?.Value ?? throw new ArgumentNullException(nameof(Integer), "Value of Integer is required"),
         InjectionValueType.Double => Double?.Value ?? throw new ArgumentNullException(nameof(Double), "Value of Double is required"),
         InjectionValueType.Boolean => Boolean?.Value ?? throw new ArgumentNullException(nameof(Boolean), "Value of Boolean is required"),
         InjectionValueType.ByteArray => ByteArray?.Value ?? throw new ArgumentNullException(nameof(ByteArray), "Value of ByteStream is required"),
         InjectionValueType.DateTime => DateTime.GetTime(),
         InjectionValueType.UnixEpochTime => UnixEpochTime.GetTime(),
         InjectionValueType.Null => null,
         _ => throw new ArgumentOutOfRangeException(nameof(ValueType), 
          ValueType, "Static Field Injection Type to Value not supported")
    };
}
