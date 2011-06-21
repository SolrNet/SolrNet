using System.Configuration;

namespace AutofacContrib.SolrNet.Config
{
    public class SolrConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public SolrServers SolrServers
        {
            get { return (SolrServers)base[""]; }
        }
    }
}