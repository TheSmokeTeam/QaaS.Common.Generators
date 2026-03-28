using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations;

public record BaseExternalSourceBasedGeneratorConfig
{
    [Description(
         "In which order to arrange the loaded data. Arranges data based of the key identifier of the item.  Options: " +
         "[ `AsciiAsc` - orders by the ascii value ascending / " +
         "`AsciiDesc` - orders by the ascii value descending / " +
         "`FirstNumericalAsc` - orders by the first found numerical value in the string ascending /" +
         "`FirstNumericalDesc` - orders by the first found numerical value in the string descending /" +
         "'Unordered' - does not order the data ]"),
     DefaultValue(typeof(DataArrangeOrder), nameof(global::QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations.DataArrangeOrder.Unordered))]
    public DataArrangeOrder? DataArrangeOrder { get; set; } =
        global::QaaS.Common.Generators.ConfigurationObjects.FromExternalSourceConfigurations.DataArrangeOrder.Unordered;

    [Range(0, int.MaxValue),
     Description("The number of items to generate out of the given data sources (If count is bigger than" +
                 " available number of items an exception will be thrown), " +
                 "if no value is given generates the number of items in the given data sources")]
    public int? Count { get; set; } = null;

    [Description(
         "A regex expression to use for filtering data from the external source by their UUID, only items that match the regex will be taken"),
     DefaultValue(".*")]
    public string DataUuidRegexExpression { get; set; } = ".*";

    [Description($"The way to store the name of the generated data as key in {nameof(FromExternalSourceConfigurations.StorageMetaData)}. Options:" +
                 "[ `FullPath` - stores the full path of the item as the key /" + 
                 " `RelativePath` - stores the relative path related to the prefix /" +
                 " `ItemName` - stores only the name of the item as the key /" +
                 " `None` - does not store the name of the item"), DefaultValue(StorageMetaData.RelativePath)]
    public StorageMetaData StorageMetaData { get; set; } = StorageMetaData.RelativePath;
    
}
