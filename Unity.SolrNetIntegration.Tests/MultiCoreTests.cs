using System.Configuration;
using MbUnit.Framework;
using Microsoft.Practices.Unity;
using SolrNet;
using Unity.SolrNetIntegration.Config;

namespace Unity.SolrNetIntegration.Tests {
  [TestFixture]
  public class MultiCoreTests {
    private IUnityContainer container;

    [SetUp]
    public void SetUp() {
      var solrConfig = (SolrConfigurationSection) ConfigurationManager.GetSection("solr");

      container = new UnityContainer();
      new SolrNetContainerConfiguration().ConfigureContainer(solrConfig.SolrServers, container);
    }

    [Test]
    public void Get_SolrOperations_for_Entity() {
      var solrOperations = container.Resolve<ISolrOperations<Entity>>();
      Assert.IsNotNull(solrOperations);
    }

    [Test]
    public void Get_SolrOperations_for_Entity2() {
      var solrOperations2 = container.Resolve<ISolrOperations<Entity2>>();
      Assert.IsNotNull(solrOperations2);
    }
  }
}