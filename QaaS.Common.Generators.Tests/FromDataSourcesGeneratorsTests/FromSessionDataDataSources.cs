using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using QaaS.Common.Generators.ConfigurationObjects.FromDataSourcesConfigurations.SessionDataConfigurations;
using QaaS.Common.Generators.Tests.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session;
using QaaS.Framework.SDK.Session.CommunicationDataObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.Serialization;

namespace QaaS.Common.Generators.Tests.FromDataSourcesGeneratorsTests;

public class FromSessionDataDataSources
{

    private static IEnumerable<TestCaseData> TestGenerateWithValidSessionDataAndConfigurationCaseSource()
    {
        var mockData = new DetailedData<object>
        {
            MetaData = null,
            Timestamp = new DateTime(0),
            Body = new byte[] { 1, 2, 3 }
        };
        var differentMockData = new DetailedData<object>
        {
            Body = new byte[] { 1, 2, 3, 4, 5, 6 },
            MetaData = null,
            Timestamp = new DateTime(0)
        };
        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>(),
            Array.Empty<FromSessionDataDataSourcesConfiguration>(),
            new List<Data<object>>()).SetName("0DataSourcesWith0SessionDataItems0Configured");

        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>> { new() },
            Array.Empty<FromSessionDataDataSourcesConfiguration>(),
            new List<Data<object>>()).SetName("1DataSourcesWith0SessionDataItems0Configured");

        yield return new TestCaseData(
            new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>
                { new() { new Framework.SDK.Session.SessionDataObjects.SessionData { Name = "test" } } },
            Array.Empty<FromSessionDataDataSourcesConfiguration>(),
            new List<Data<object>>()).SetName("1DataSourcesWith1SessionDataItems0Configured");

        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>
                {
                    new()
                    {
                        new Framework.SDK.Session.SessionDataObjects.SessionData
                        {
                            Name = "test",
                            Outputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "output",
                                    Data = new List<DetailedData<object>> { mockData, mockData, mockData }
                                }
                            }
                        }
                    }
                }, new FromSessionDataDataSourcesConfiguration[]
                {
                    new()
                    {
                        SessionName = "test",
                        CommunicationDataList = new CommunicationDataName[]
                        {
                            new()
                            {
                                Type = CommunicationDataType.Output,
                                Name = "output"
                            }
                        }
                    }
                },
                new List<Data<object>> { mockData, mockData, mockData })
            .SetName("1DataSourcesWith1SessionDataItems1ConfiguredWith1OutputContaining3ItemsConfigured");

        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>
                {
                    new()
                    {
                        new Framework.SDK.Session.SessionDataObjects.SessionData
                        {
                            Name = "test",
                            Inputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "input",
                                    Data = new List<DetailedData<object>> { mockData, mockData, mockData }
                                }
                            }
                        }
                    }
                }, new FromSessionDataDataSourcesConfiguration[]
                {
                    new()
                    {
                        SessionName = "test",
                        CommunicationDataList = new CommunicationDataName[]
                        {
                            new()
                            {
                                Type = CommunicationDataType.Input,
                                Name = "input"
                            }
                        }
                    }
                },
                new List<Data<object>> { mockData, mockData, mockData })
            .SetName("1DataSourceSourcesWith1SessionDataItems1ConfiguredWith1InputContaining3ItemsConfigured");

        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>
                {
                    new()
                    {
                        new Framework.SDK.Session.SessionDataObjects.SessionData
                        {
                            Name = "test",
                            Inputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "input",
                                    Data = new List<DetailedData<object>> { mockData, mockData, mockData }
                                }
                            },
                            Outputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "output",
                                    Data = new List<DetailedData<object>>
                                        { differentMockData, differentMockData, differentMockData }
                                }
                            }
                        }
                    }
                }, new FromSessionDataDataSourcesConfiguration[]
                {
                    new()
                    {
                        SessionName = "test",
                        CommunicationDataList = new CommunicationDataName[]
                        {
                            new()
                            {
                                Type = CommunicationDataType.Input,
                                Name = "input"
                            },
                            new()
                            {
                                Type = CommunicationDataType.Output,
                                Name = "output"
                            }
                        }
                    }
                },
                new List<Data<object>>
                    { mockData, mockData, mockData, differentMockData, differentMockData, differentMockData })
            .SetName(
                "1DataSourcesWith1SessionDataItems1ConfiguredWith1InputContaining3ItemsAnd1OutputContaining3ItemsConfigured");

        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>
                {
                    new()
                    {
                        new Framework.SDK.Session.SessionDataObjects.SessionData
                        {
                            Name = "test",
                            Inputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "input",
                                    Data = new List<DetailedData<object>> { mockData, mockData }
                                }
                            }
                        },
                        new Framework.SDK.Session.SessionDataObjects.SessionData
                        {
                            Name = "test2",
                            Outputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "output",
                                    Data = new List<DetailedData<object>> { differentMockData, differentMockData }
                                }
                            }
                        }
                    }
                }, new FromSessionDataDataSourcesConfiguration[]
                {
                    new()
                    {
                        SessionName = "test",
                        CommunicationDataList = new CommunicationDataName[]
                        {
                            new()
                            {
                                Type = CommunicationDataType.Input,
                                Name = "input"
                            }
                        }
                    },
                    new()
                    {
                        SessionName = "test2",
                        CommunicationDataList = new CommunicationDataName[]
                        {
                            new()
                            {
                                Type = CommunicationDataType.Output,
                                Name = "output"
                            }
                        }
                    }
                },
                new List<Data<object>> { mockData, mockData, differentMockData, differentMockData })
            .SetName(
                "1DataSourcesWith2SessionDataItems1ConfiguredWith1InputInOneAndOneOutputInTheOtherContaining2ItemsEachConfigured");

        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>
                {
                    new()
                    {
                        new Framework.SDK.Session.SessionDataObjects.SessionData
                        {
                            Name = "test",
                            Inputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "input",
                                    Data = new List<DetailedData<object>> { mockData, mockData }
                                }
                            }
                        },
                        new Framework.SDK.Session.SessionDataObjects.SessionData
                        {
                            Name = "test2",
                            Outputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "output",
                                    Data = new List<DetailedData<object>> { differentMockData, differentMockData }
                                }
                            }
                        }
                    },
                    new()
                    {
                        new Framework.SDK.Session.SessionDataObjects.SessionData
                        {
                            Name = "test3",
                            Inputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "input",
                                    Data = new List<DetailedData<object>> { mockData, mockData }
                                }
                            }
                        },
                        new Framework.SDK.Session.SessionDataObjects.SessionData
                        {
                            Name = "test4",
                            Outputs = new List<CommunicationData<object>>
                            {
                                new()
                                {
                                    Name = "output",
                                    Data = new List<DetailedData<object>> { differentMockData, differentMockData }
                                }
                            }
                        }
                    }
                }, new FromSessionDataDataSourcesConfiguration[]
                {
                    new()
                    {
                        SessionName = "test",
                        CommunicationDataList = new CommunicationDataName[]
                        {
                            new()
                            {
                                Type = CommunicationDataType.Input,
                                Name = "input"
                            }
                        }
                    },
                    new()
                    {
                        SessionName = "test2",
                        CommunicationDataList = new CommunicationDataName[]
                        {
                            new()
                            {
                                Type = CommunicationDataType.Output,
                                Name = "output"
                            }
                        }
                    },
                    new()
                    {
                        SessionName = "test3",
                        CommunicationDataList = new CommunicationDataName[]
                        {
                            new()
                            {
                                Type = CommunicationDataType.Input,
                                Name = "input"
                            }
                        }
                    },
                    new()
                    {
                        SessionName = "test4",
                        CommunicationDataList = new CommunicationDataName[]
                        {
                            new()
                            {
                                Type = CommunicationDataType.Output,
                                Name = "output"
                            }
                        }
                    }
                },
                new List<Data<object>>
                {
                    mockData, mockData, differentMockData, differentMockData, mockData, mockData, differentMockData,
                    differentMockData
                })
            .SetName(
                "2DataSourcesWith2SessionDataItems1ConfiguredWith1InputInOneAndOneOutputInTheOtherContaining2ItemsEachConfigured");
    }

    [Test, TestCaseSource(nameof(TestGenerateWithValidSessionDataAndConfigurationCaseSource))]
    public void TestGenerateWithValidSessionDataAndConfiguration_CallFunction_ShouldReturnExpectedGeneratedData(
        IEnumerable<List<Framework.SDK.Session.SessionDataObjects.SessionData>> dataSourceListSessionDataItems,
        FromSessionDataDataSourcesConfiguration[] configuration, IList<Data<object>> expectedGeneratedData)
    {
        // Arrange
        var generation = new FromDataSourcesGenerators.FromSessionDataDataSources
        {
            Context = Globals.Context,
            Configuration = configuration.ToList()
        };

        // Act
        var dataSourceCounter = 0;
        var generatedData = generation.Generate(new ImmutableArray<Framework.SDK.Session.SessionDataObjects.SessionData>(),
            dataSourceListSessionDataItems.Select(sessionDataItems
                =>
            {
                var dataSource = new DataSource()
                {
                    Name = dataSourceCounter++.ToString(),
                    
                };
                var generatedData = sessionDataItems.Select(data => new Data<object>()
                {
                    Body =  SessionDataSerialization.SerializeSessionData(data)
                });
                return dataSource.SetGeneratedData(generatedData.ToImmutableList());
            }).ToImmutableList()).ToList();

        // Assert
        //CollectionAssert.AreEqual(expectedGeneratedData, generatedData);
        for (var expectedGeneratedDataIndex = 0;
             expectedGeneratedDataIndex < expectedGeneratedData.Count;
             expectedGeneratedDataIndex++)
        {
            var expectedOutput =
                expectedGeneratedData.ElementAtOrDefault(expectedGeneratedDataIndex);
            var output = generatedData.ElementAtOrDefault(expectedGeneratedDataIndex);
            Globals.Logger.LogInformation("Checking generated data at index {GeneratedDataIndex}",
                expectedGeneratedDataIndex);
            Assert.That(output?.MetaData, Is.EqualTo(expectedOutput?.MetaData));
            Assert.That(output?.Body, Is.EqualTo(expectedOutput?.Body));
        }
    }

    private static IEnumerable<TestCaseData> TestGenerateWithInvalidSessionDataAndConfigurationCaseSource()
    {
        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>(new[]
            {
                new List<Framework.SDK.Session.SessionDataObjects.SessionData>()
                {
                    new()
                    {
                        Name = "test"
                    },
                    new()
                    {
                        Name = "test"
                    }
                }
            }), new FromSessionDataDataSourcesConfiguration[] { new() { SessionName = "test" } }
        ).SetName("MoreThan1SessionWithSameConfiguredNameInSameDataSource");

        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>(
                new List<Framework.SDK.Session.SessionDataObjects.SessionData>[]
                {
                    new()
                    {
                        new()
                        {
                            Name = "test"
                        }
                    },
                    new()
                    {
                        new()
                        {
                            Name = "test"
                        }
                    }
                }), new FromSessionDataDataSourcesConfiguration[] { new() { SessionName = "test" } }
        ).SetName("MoreThan1SessionWithSameConfiguredIdInDifferentDataSource");

        yield return new TestCaseData(new List<List<Framework.SDK.Session.SessionDataObjects.SessionData>>(),
                new FromSessionDataDataSourcesConfiguration[] { new() { SessionName = "test" } })
            .SetName("NoSessionWithConfiguredId");
    }

    [Test, TestCaseSource(nameof(TestGenerateWithInvalidSessionDataAndConfigurationCaseSource))]
    public void TestGenerateWithInvalidSessionDataAndConfiguration_CallFunction_ShouldThrowArgumentException(
        IEnumerable<List<Framework.SDK.Session.SessionDataObjects.SessionData>> dataSourceListSessionDataItems,
        FromSessionDataDataSourcesConfiguration[] configuration)
    {
        // Arrange
        var generation = new FromDataSourcesGenerators.FromSessionDataDataSources
        {
            Context = Globals.Context,
            Configuration = configuration.ToList()
        };

        // Act + Assert
        var dataSourceCounter = 0;
        Assert.Throws<ArgumentException>(() =>
            generation.Generate(new ImmutableArray<Framework.SDK.Session.SessionDataObjects.SessionData>(),
                dataSourceListSessionDataItems.Select(sessionDataItems =>
                {   var dataSource = new DataSource()
                    {
                        Name = dataSourceCounter++.ToString()
                       
                    };
                    var generatedData = sessionDataItems.Select(data => new Data<object>()
                    {
                        Body =  SessionDataSerialization.SerializeSessionData(data)
                    });
                    return dataSource.SetGeneratedData(generatedData.ToImmutableList());
                }).ToImmutableList()).ToList());
    }
}