using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using QaaS.Common.Generators.FromDataSourcesGenerators;

namespace QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;

/// <summary>
/// Configuration for <see cref="FromPcapDataSources"/>.
/// </summary>
[ExcludeFromCodeCoverage]
[Description("Generates packet data from pcap files provided by other data sources")]
public record FromPcapDataSourcesConfiguration : BaseFromDataSourcesConfiguration
{
    /// <summary>
    /// Maximum packet size in bytes that will be emitted.
    /// Packets larger than this value are skipped.
    /// A null or zero value disables the limit.
    /// </summary>
    [Description("Maximum packet size in bytes. Larger packets are skipped. 0 or null means no limit.")]
    [Range(0, int.MaxValue)]
    public int? MaxPacketSize { get; init; }

    /// <summary>
    /// Minimum packet size in bytes that will be emitted.
    /// Packets smaller than this value are skipped.
    /// A null or zero value disables the limit.
    /// </summary>
    [Description("Minimum packet size in bytes. Smaller packets are skipped. 0 or null means no limit.")]
    [Range(0, int.MaxValue)]
    public int? MinPacketSize { get; init; }

    /// <summary>
    /// Number of initial packets to skip before starting to emit.
    /// A null or zero value disables skipping.
    /// </summary>
    [Description("Number of initial packets to skip before starting to emit. 0 or null means do not skip.")]
    [Range(0, int.MaxValue)]
    public int? Skip { get; init; }
}
