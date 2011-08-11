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
using Unity.SolrNetIntegration.Config;

#endregion

namespace Unity.SolrNetIntegration.Tests {
  [TestFixture]
  public class RegistryTests {
    private static IUnityContainer container;

    [Test]
    public void ResolveSolrOperations() {
      SetupContainer();
      var m = container.Resolve<ISolrOperations<Entity>>();
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
      var solr = container.Resolve<ISolrOperations<Entity>>();
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
      var parsers = (ISolrResponseParser<Entity>[]) field.GetValue(parser);
      Assert.AreEqual(11, parsers.Length);
      foreach (var t in parsers)
        Console.WriteLine(t);
    }

    [Test]
    public void DictionaryDocument_and_multi_core() {
      var cores = new SolrServers {
        new SolrServerElement {
          Id = "default",
          DocumentType = typeof (Entity).AssemblyQualifiedName,
          Url = "http://localhost:8983/solr/entity1",
        },
        new SolrServerElement {
          Id = "entity1dict",
          DocumentType = typeof (Dictionary<string, object>).AssemblyQualifiedName,
          Url = "http://localhost:8983/solr/entity1",
        },
        new SolrServerElement {
          Id = "another",
          DocumentType = typeof (Entity2).AssemblyQualifiedName,
          Url = "http://localhost:8983/solr/entity2",
        },
      };

      container = new UnityContainer();
      new SolrNetContainerConfiguration().ConfigureContainer(cores, container);
      container.Resolve<ISolrOperations<Entity>>();
      container.Resolve<ISolrOperations<Entity2>>();
      container.Resolve<ISolrOperations<Dictionary<string, object>>>();
    }

    [Test, Category("Integration")]
    public void DictionaryDocument() {
      SetupContainer();

      var solr = container.Resolve<ISolrOperations<Dictionary<string, object>>>();
      var results = solr.Query(SolrQuery.All);
      Assert.GreaterThan(results.Count, 0);
      foreach (var d in results) {
        Assert.GreaterThan(d.Count, 0);
        foreach (var kv in d)
          Console.WriteLine("{0}: {1}", kv.Key, kv.Value);
      }
    }

    [Test, Category("Integration")]
    public void DictionaryDocument_add() {
      SetupContainer();

      var solr = container.Resolve<ISolrOperations<Dictionary<string, object>>>();

      solr.Add(new Dictionary<string, object> {
        {"id", "ababa"},
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