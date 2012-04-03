#region usings

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using MbUnit.Framework;
using Microsoft.Practices.Unity;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using Unity.SolrNetIntegration.Config;

#endregion

namespace Unity.SolrNetIntegration.Tests {
  [TestFixture]
  public class RegistryTests {
    private static IUnityContainer container;
    private SolrServers testServers = new SolrServers {
        new SolrServerElement {
          Id = "entity",
          DocumentType = typeof (Entity).AssemblyQualifiedName,
          Url = "http://localhost:8983/solr/entity",
        },
        new SolrServerElement {
          Id = "entity2Dict",
          DocumentType = typeof (Dictionary<string, object>).AssemblyQualifiedName,
          Url = "http://localhost:8983/solr/entity2",
        },
        new SolrServerElement {
          Id = "entity2",
          DocumentType = typeof (Entity2).AssemblyQualifiedName,
          Url = "http://localhost:8983/solr/entity2",
        },
      };

    [Test]
    public void ResolveSolrOperations() {
      SetupContainer();
      var m = container.Resolve<ISolrOperations<Entity>>("entity");
      Assert.IsNotNull(m);
    }

    [Test]
    public void RegistersSolrConnectionWithAppConfigServerUrl() {
      SetupContainer();
      var instanceKey = "entity" + typeof (SolrConnection);

      var solrConnection = (SolrConnection) container.Resolve<ISolrConnection>(instanceKey);

      Assert.AreEqual("http://localhost:8983/solr/entity", solrConnection.ServerURL);
    }

    [Test, Category("Integration")]
    public void Ping_And_Query() {
      SetupContainer();
      var solr = container.Resolve<ISolrOperations<Entity>>("entity");
      solr.Ping();
      Console.WriteLine(solr.Query(SolrQuery.All).Count);
    }

    [Test, ExpectedException(typeof (InvalidURLException))]
    public void Should_throw_exception_for_invalid_protocol_on_url() {
      var solrServers = new SolrServers {
        new SolrServerElement {
          Id = "test",
          Url = "htp://localhost:8893",
          DocumentType = typeof (Entity2).AssemblyQualifiedName,
        }
      };
      container = new UnityContainer();
      new SolrNetContainerConfiguration().ConfigureContainer(solrServers, container);
      container.Resolve<SolrConnection>();
    }

    [Test, ExpectedException(typeof (InvalidURLException))]
    public void Should_throw_exception_for_invalid_url() {
      var solrServers = new SolrServers {
        new SolrServerElement {
          Id = "test",
          Url = "http:/localhost:8893",
          DocumentType = typeof (Entity2).AssemblyQualifiedName,
        }
      };
      container = new UnityContainer();
      new SolrNetContainerConfiguration().ConfigureContainer(solrServers, container);
      container.Resolve<SolrConnection>();
    }

    [Test]
    public void Container_has_ISolrFieldParser() {
      SetupContainer();
      var parser = container.Resolve<ISolrFieldParser>();
      Assert.IsNotNull(parser);
    }

    [Test]
    public void Container_has_ISolrFieldSerializer() {
      SetupContainer();
      container.Resolve<ISolrFieldSerializer>();
    }

    [Test]
    public void Container_has_ISolrDocumentPropertyVisitor() {
      SetupContainer();
      container.Resolve<ISolrDocumentPropertyVisitor>();
    }

    [Test]
    public void ResponseParsers() {
      SetupContainer();

      var parser = container.Resolve<ISolrQueryResultParser<Entity>>() as SolrQueryResultParser<Entity>;

      var field = parser.GetType().GetField("parsers", BindingFlags.NonPublic | BindingFlags.Instance);
      var parsers = (ISolrAbstractResponseParser<Entity>[]) field.GetValue(parser);
      Assert.AreEqual(13, parsers.Length);
      foreach (var t in parsers)
        Console.WriteLine(t);
    }

    [Test]
    public void DictionaryDocument_and_multi_core() {
      container = new UnityContainer();
      new SolrNetContainerConfiguration().ConfigureContainer(testServers, container);
      container.Resolve<ISolrOperations<Entity>>("entity");
      container.Resolve<ISolrOperations<Entity2>>("entity2");
      container.Resolve<ISolrOperations<Dictionary<string, object>>>("entity2Dict");
    }

    

    [Test, Category("Integration")]
    public void DictionaryDocument() {
      container = new UnityContainer();
      new SolrNetContainerConfiguration().ConfigureContainer(testServers, container);
      var solr = container.Resolve<ISolrOperations<Entity2>>("entity2");
      var results = solr.Query(SolrQuery.All);
      Assert.GreaterThan(results.Count, 0);
    }

    [Test, Category("Integration")]
    public void DictionaryDocument_add() {
      container = new UnityContainer();
      new SolrNetContainerConfiguration().ConfigureContainer(testServers, container);

      var solr = container.Resolve<ISolrOperations<Dictionary<string, object>>>("entity2Dict");

      solr.Add(new Dictionary<string, object> {
        {"id", "5"},
        {"manu", "who knows"},
        {"popularity", 55},
        {"timestamp", DateTime.UtcNow},
      });
    }

    [Test]
    public void DictionaryDocument_ResponseParser() {
      SetupContainer();

      var parser = container.Resolve<ISolrDocumentResponseParser<Dictionary<string, object>>>();
      Assert.IsInstanceOfType<SolrDictionaryDocumentResponseParser>(parser);
    }

    [Test]
    public void DictionaryDocument_Serializer() {
      SetupContainer();
      var serializer = container.Resolve<ISolrDocumentSerializer<Dictionary<string, object>>>();
      Assert.IsInstanceOfType<SolrDictionarySerializer>(serializer);
    }

    private static void SetupContainer() {
      var solrConfig = (SolrConfigurationSection) ConfigurationManager.GetSection("solr");
      container = new UnityContainer();
      new SolrNetContainerConfiguration().ConfigureContainer(solrConfig.SolrServers, container);
    }
  }
}