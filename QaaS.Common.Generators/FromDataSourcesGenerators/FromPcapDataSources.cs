using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Common.Generators.FromDataSourcesGenerators.Utils;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Generators.FromDataSourcesGenerators;

/// <summary>
/// Generator that parses pcap files received from upstream data sources into individual packets.
/// </summary>
/// <qaas-docs group="Existing data sources" subgroup="Pcap files" />
public class FromPcapDataSources : BaseFromDataSourcesGenerator<FromPcapDataSourcesConfiguration>
{
    private int? _remainingSkip;

    /// <inheritdoc />
    public override IEnumerable<Data<object>> Generate(
        IImmutableList<SessionData> sessionDataList,
        IImmutableList<DataSource> dataSourceList)
    {
        ValidateConfiguration();
        _remainingSkip = Configuration.Skip ?? 0;
        return base.Generate(sessionDataList, dataSourceList);
    }

    /// <inheritdoc />
    protected override IEnumerable<Data<object>> ConvertDataSourceDataToGeneratedItems(
        Data<object> data, string dataSourceName)
    {
        _remainingSkip ??= Configuration.Skip ?? 0;

        using var enumerator = TryOpenEnumerator(data, dataSourceName);
        if (enumerator is null)
            yield break;

        var emittedPackets = 0;
        var totalPacketsInSource = 0;

        while (enumerator.MoveNext())
        {
            totalPacketsInSource++;

            if (_remainingSkip > 0)
            {
                _remainingSkip--;
                continue;
            }

            var packet = enumerator.Current;
            emittedPackets++;

            yield return new Data<object>
            {
                Body = packet,
                MetaData = CreateMetaData(data.MetaData)
            };
        }

        if (enumerator.SkippedPackets > 0)
        {
            Context.Logger.LogInformation(
                "Data source {DataSourceName}: skipped {SkippedPackets} packets ({SkippedBytes} bytes) due to packet size filters.",
                dataSourceName,
                enumerator.SkippedPackets,
                enumerator.SkippedBytes);
        }

        if (Configuration.Skip is > 0 && emittedPackets == 0 && totalPacketsInSource > 0)
        {
            Context.Logger.LogWarning(
                "Data source {DataSourceName}: configured Skip ({ConfiguredSkip}) exceeds or equals the number of matching packets ({TotalPackets}). 0 packets were emitted.",
                dataSourceName,
                Configuration.Skip,
                totalPacketsInSource);
        }
    }

    private void ValidateConfiguration()
    {
        if (Configuration.MinPacketSize > Configuration.MaxPacketSize)
        {
            throw new ArgumentException(
                $"MinPacketSize ({Configuration.MinPacketSize}) cannot be greater than MaxPacketSize ({Configuration.MaxPacketSize}).",
                nameof(Configuration.MinPacketSize));
        }
    }

    private PcapPacketParser.Enumerator? TryOpenEnumerator(Data<object> data, string dataSourceName)
    {
        try
        {
            var pcapBytes = GetPcapBytes(data, dataSourceName);
            return PcapPacketParser.GetEnumerator(
                pcapBytes,
                Configuration.MinPacketSize,
                Configuration.MaxPacketSize,
                Context.Logger);
        }
        catch (Exception ex)
        {
            Context.Logger.LogError(
                ex,
                "Failed to process pcap from data source {DataSourceName}.",
                dataSourceName);
            return null;
        }
    }

    private static byte[] GetPcapBytes(Data<object> data, string dataSourceName)
    {
        if (data.Body is not byte[] pcapBytes)
        {
            throw new ArgumentException(
                $"Data source {dataSourceName} contains an item not deserialized into binary bytes, " +
                $"so it cannot be used in generator {nameof(FromPcapDataSources)}.");
        }

        return pcapBytes;
    }

    private static MetaData CreateMetaData(MetaData? sourceMetaData)
    {
        var storage = new Storage { Key = Guid.NewGuid().ToString() };

        return sourceMetaData != null
            ? sourceMetaData with { Storage = storage }
            : new MetaData { Storage = storage };
    }
}
