using Haukcode.PcapngUtils;
using Microsoft.Extensions.Logging;

namespace QaaS.Common.Generators.FromDataSourcesGenerators.Utils;

/// <summary>
/// Provides an enumerator over packets stored in a pcap file byte array.
/// Supports classic pcap and pcapng without writing temporary files.
/// </summary>
internal static class PcapPacketParser
{
    /// <summary>
    /// Creates an enumerator over packets in the provided pcap bytes.
    /// </summary>
    public static Enumerator GetEnumerator(byte[] pcapBytes, int? minPacketSize, int? maxPacketSize, ILogger? logger = null)
    {
        return new Enumerator(pcapBytes, minPacketSize, maxPacketSize, logger);
    }

    /// <summary>
    /// Enumerates packets from a pcap byte array.
    /// </summary>
    internal sealed class Enumerator : IEnumerator<byte[]>
    {
        private readonly IEnumerator<byte[]> _inner;
        private readonly ILogger? _logger;
        private readonly int? _minPacketSize;
        private readonly int? _maxPacketSize;

        private byte[]? _current;

        /// <summary>
        /// Gets the number of packets skipped because their size did not meet the size constraints.
        /// </summary>
        public int SkippedPackets { get; private set; }

        /// <summary>
        /// Gets the total number of bytes skipped from packets not meeting the size constraints.
        /// </summary>
        public long SkippedBytes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> class.
        /// </summary>
        /// <param name="pcapBytes">The raw pcap binary data.</param>
        /// <param name="minPacketSize">Minimum allowed packet size in bytes, or null/0 for no limit.</param>
        /// <param name="maxPacketSize">Maximum allowed packet size in bytes, or null/0 for no limit.</param>
        /// <param name="logger">Optional logger for diagnostic messages.</param>
        public Enumerator(byte[] pcapBytes, int? minPacketSize, int? maxPacketSize, ILogger? logger)
        {
            _logger = logger;
            _minPacketSize = minPacketSize;
            _maxPacketSize = maxPacketSize;
            _inner = EnumeratePackets(pcapBytes);
        }

        /// <inheritdoc />
        public byte[] Current => _current ?? throw new InvalidOperationException("Enumeration has not started or has ended.");

        object System.Collections.IEnumerator.Current => Current;

        /// <summary>
        /// Advances the enumerator to the next packet that satisfies the size constraints.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the next packet;
        /// <see langword="false"/> if the enumerator has passed the end of the collection or an error occurred.
        /// </returns>
        public bool MoveNext()
        {
            try
            {
                while (_inner.MoveNext())
                {
                    var packet = _inner.Current;

                    if (_minPacketSize is > 0 && packet.Length < _minPacketSize)
                    {
                        SkippedPackets++;
                        SkippedBytes += packet.Length;
                        continue;
                    }

                    if (_maxPacketSize is > 0 && packet.Length > _maxPacketSize)
                    {
                        SkippedPackets++;
                        SkippedBytes += packet.Length;
                        continue;
                    }

                    _current = packet;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError("Failed to enumerate pcap packets: {Message}", ex.Message);
            }

            _current = null;
            return false;
        }

        /// <inheritdoc />
        public void Reset() => throw new NotSupportedException();

        /// <inheritdoc />
        public void Dispose()
        {
            _inner.Dispose();
        }
    }

    /// <summary>
    /// Reads and yields individual raw packet payloads from the given pcap bytes using a streaming reader.
    /// </summary>
    /// <param name="pcapBytes">The binary pcap or pcapng payload.</param>
    /// <returns>An enumerator yielding each packet's raw payload bytes.</returns>
    private static IEnumerator<byte[]> EnumeratePackets(byte[] pcapBytes)
    {
        using var stream = new MemoryStream(pcapBytes);
        using var reader = IReaderFactory.GetReader(stream);

        while (reader.MoreAvailable)
        {
            var packet = reader.ReadNextPacket();
            if (packet == null)
                yield break;

            yield return packet.Data.ToArray();
        }
    }
}
