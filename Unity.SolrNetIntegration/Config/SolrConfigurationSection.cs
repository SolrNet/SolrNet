using System.Configuration;

namespace Unity.SolrNetIntegration.Config {
  public class SolrConfigurationSection : ConfigurationSection {
    [ConfigurationProperty("", IsDefaultCollection = true)]
    public SolrServers SolrServers {
      get { return (SolrServers) base[""]; }
    }
  }
}