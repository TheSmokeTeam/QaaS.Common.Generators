using System.Collections.Immutable;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations;
using QaaS.Common.Generators.FromDataSourcesGenerators;
using QaaS.Common.Generators.Tests.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using SessionData = QaaS.Framework.SDK.Session.SessionDataObjects.SessionData;

namespace QaaS.Common.Generators.Tests.FromDataSourcesGeneratorsTests;

[TestFixture]
public class FromPcapDataSourcesTests
{
    private static byte[] CreateSamplePcap(params byte[][] packets)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        // Global Header (24 bytes)
        writer.Write(0xa1b2c3d4); // magic
        writer.Write((ushort)2);   // major
        writer.Write((ushort)4);   // minor
        writer.Write(0);          // thiszone
        writer.Write(0u);         // sigfigs
        writer.Write(65535u);     // snaplen
        writer.Write(1u);         // network (Ethernet)

        // Packets
        foreach (var packet in packets)
        {
            writer.Write(1000);             // ts_sec
            writer.Write(500);              // ts_usec
            writer.Write((uint)packet.Length); // incl_len
            writer.Write((uint)packet.Length); // orig_len
            writer.Write(packet);
        }

        return ms.ToArray();
    }

    [Test]
    public void Generate_ValidPcapWithMultiplePackets_YieldsAllPackets()
    {
        var packet1 = new byte[] { 1, 2, 3, 4 };
        var packet2 = new byte[] { 5, 6, 7, 8, 9 };
        var pcapBytes = CreateSamplePcap(packet1, packet2);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration(),
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Body, Is.EqualTo(packet1));
        Assert.That(results[1].Body, Is.EqualTo(packet2));
    }

    [Test]
    public void Generate_WithMaxPacketSize_SkipsOversizedPackets()
    {
        var smallPacket = new byte[] { 1, 2, 3 };
        var largePacket = new byte[] { 4, 5, 6, 7, 8, 9, 10 };
        var pcapBytes = CreateSamplePcap(smallPacket, largePacket);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration { MaxPacketSize = 5 },
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].Body, Is.EqualTo(smallPacket));
    }

    [Test]
    public void Generate_WithCountLimit_StopsAfterCount()
    {
        var packet1 = new byte[] { 1, 2 };
        var packet2 = new byte[] { 3, 4 };
        var packet3 = new byte[] { 5, 6 };
        var pcapBytes = CreateSamplePcap(packet1, packet2, packet3);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration { Count = 2 },
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Body, Is.EqualTo(packet1));
        Assert.That(results[1].Body, Is.EqualTo(packet2));
    }

    [Test]
    public void Generate_NonBinaryData_LogsErrorAndSkips()
    {
        var dataSource = new DataSource { Name = "InvalidSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = "not binary bytes" }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration(),
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Is.Empty);
    }

    [Test]
    public void Generate_EmptyPcap_YieldsNoPackets()
    {
        var pcapBytes = CreateSamplePcap();

        var dataSource = new DataSource { Name = "EmptyPcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration(),
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Is.Empty);
    }

    [Test]
    public void Generate_CorruptedPcapBytes_LogsErrorAndYieldsNoPackets()
    {
        var corruptedBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        var dataSource = new DataSource { Name = "CorruptedPcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = corruptedBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration(),
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Is.Empty);
    }

    [Test]
    public void Generate_MaxPacketSizeZero_EmitsAllPacketsWithoutLimit()
    {
        var smallPacket = new byte[] { 1, 2, 3 };
        var largePacket = new byte[] { 4, 5, 6, 7, 8, 9, 10 };
        var pcapBytes = CreateSamplePcap(smallPacket, largePacket);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration { MaxPacketSize = 0 },
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Body, Is.EqualTo(smallPacket));
        Assert.That(results[1].Body, Is.EqualTo(largePacket));
    }

    [Test]
    public void Generate_MultiplePackets_EachPacketHasUniqueMetadataKey()
    {
        var packet1 = new byte[] { 1, 2 };
        var packet2 = new byte[] { 3, 4 };
        var pcapBytes = CreateSamplePcap(packet1, packet2);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration(),
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].MetaData?.Storage?.Key, Is.Not.Null.And.Not.Empty);
        Assert.That(results[1].MetaData?.Storage?.Key, Is.Not.Null.And.Not.Empty);
        Assert.That(results[0].MetaData!.Storage!.Key, Is.Not.EqualTo(results[1].MetaData!.Storage!.Key));
    }

    [Test]
    public void Generate_MultipleDataSources_ProcessesPacketsFromAllSources()
    {
        var packet1 = new byte[] { 1, 2 };
        var packet2 = new byte[] { 3, 4 };

        var dataSource1 = new DataSource { Name = "Source1" };
        dataSource1.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = CreateSamplePcap(packet1) }
        });

        var dataSource2 = new DataSource { Name = "Source2" };
        dataSource2.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = CreateSamplePcap(packet2) }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration(),
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource1, dataSource2)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Body, Is.EqualTo(packet1));
        Assert.That(results[1].Body, Is.EqualTo(packet2));
    }

    [Test]
    public void Generate_WithMinPacketSize_SkipsUndersizedPackets()
    {
        var smallPacket = new byte[] { 1, 2 };
        var mediumPacket = new byte[] { 3, 4, 5, 6, 7 };
        var largePacket = new byte[] { 8, 9, 10, 11, 12, 13, 14, 15 };
        var pcapBytes = CreateSamplePcap(smallPacket, mediumPacket, largePacket);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration { MinPacketSize = 5 },
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Body, Is.EqualTo(mediumPacket));
        Assert.That(results[1].Body, Is.EqualTo(largePacket));
    }

    [Test]
    public void Generate_WithMinAndMaxPacketSize_EmitsOnlyPacketsWithinRange()
    {
        var tinyPacket = new byte[] { 1, 2 };
        var validPacket1 = new byte[] { 3, 4, 5, 6 };
        var validPacket2 = new byte[] { 7, 8, 9, 10, 11, 12 };
        var hugePacket = new byte[] { 13, 14, 15, 16, 17, 18, 19, 20, 21, 22 };
        var pcapBytes = CreateSamplePcap(tinyPacket, validPacket1, validPacket2, hugePacket);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration
            {
                MinPacketSize = 4,
                MaxPacketSize = 8
            },
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Body, Is.EqualTo(validPacket1));
        Assert.That(results[1].Body, Is.EqualTo(validPacket2));
    }

    [Test]
    public void Generate_WithMinPacketSizeGreaterThanMaxPacketSize_ThrowsArgumentException()
    {
        var packet = new byte[] { 1, 2, 3 };
        var pcapBytes = CreateSamplePcap(packet);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration
            {
                MinPacketSize = 100,
                MaxPacketSize = 50
            },
            Context = Globals.Context
        };

        Assert.Throws<ArgumentException>(() =>
        {
            generator.Generate(
                ImmutableList<SessionData>.Empty,
                ImmutableList.Create(dataSource)).ToList();
        });
    }

    [Test]
    public void Generate_WithSkip_SkipsFirstNPackets()
    {
        var packet1 = new byte[] { 1 };
        var packet2 = new byte[] { 2 };
        var packet3 = new byte[] { 3 };
        var packet4 = new byte[] { 4 };
        var pcapBytes = CreateSamplePcap(packet1, packet2, packet3, packet4);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration { Skip = 2 },
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Body, Is.EqualTo(packet3));
        Assert.That(results[1].Body, Is.EqualTo(packet4));
    }

    [Test]
    public void Generate_WithSkipAndCount_ReturnsWindowOfPackets()
    {
        var packet1 = new byte[] { 1 };
        var packet2 = new byte[] { 2 };
        var packet3 = new byte[] { 3 };
        var packet4 = new byte[] { 4 };
        var packet5 = new byte[] { 5 };
        var pcapBytes = CreateSamplePcap(packet1, packet2, packet3, packet4, packet5);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration
            {
                Skip = 1,
                Count = 2
            },
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Body, Is.EqualTo(packet2));
        Assert.That(results[1].Body, Is.EqualTo(packet3));
    }

    [Test]
    public void Generate_WithSkipGreaterThanTotalPackets_YieldsNoPackets()
    {
        var packet1 = new byte[] { 1 };
        var packet2 = new byte[] { 2 };
        var pcapBytes = CreateSamplePcap(packet1, packet2);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration { Skip = 10 },
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Is.Empty);
    }

    [Test]
    public void Generate_WithSkipAndSizeFilter_SkipsAfterFiltering()
    {
        var tinyPacket = new byte[] { 1 };
        var validPacket1 = new byte[] { 10, 11, 12, 13, 14 };
        var validPacket2 = new byte[] { 20, 21, 22, 23, 24 };
        var validPacket3 = new byte[] { 30, 31, 32, 33, 34 };
        var pcapBytes = CreateSamplePcap(tinyPacket, validPacket1, validPacket2, validPacket3);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration
            {
                MinPacketSize = 5,
                Skip = 1
            },
            Context = Globals.Context
        };

        var results = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results[0].Body, Is.EqualTo(validPacket2));
        Assert.That(results[1].Body, Is.EqualTo(validPacket3));
    }

    [Test]
    public void Generate_MultipleGenerationsOnSameInstance_ResetsSkip()
    {
        var packet1 = new byte[] { 1 };
        var packet2 = new byte[] { 2 };
        var packet3 = new byte[] { 3 };
        var pcapBytes = CreateSamplePcap(packet1, packet2, packet3);

        var dataSource = new DataSource { Name = "PcapSource" };
        dataSource.SetGeneratedData(new List<Data<object>>
        {
            new() { Body = pcapBytes }
        });

        var generator = new FromPcapDataSources
        {
            Configuration = new FromPcapDataSourcesConfiguration { Skip = 1 },
            Context = Globals.Context
        };

        var results1 = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        var results2 = generator.Generate(
            ImmutableList<SessionData>.Empty,
            ImmutableList.Create(dataSource)).ToList();

        Assert.That(results1, Has.Count.EqualTo(2));
        Assert.That(results1[0].Body, Is.EqualTo(packet2));
        Assert.That(results1[1].Body, Is.EqualTo(packet3));

        Assert.That(results2, Has.Count.EqualTo(2));
        Assert.That(results2[0].Body, Is.EqualTo(packet2));
        Assert.That(results2[1].Body, Is.EqualTo(packet3));
    }
}
