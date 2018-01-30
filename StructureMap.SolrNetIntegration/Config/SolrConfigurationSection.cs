#if NET46

using System.Configuration;

namespace StructureMap.SolrNetIntegration.Config
{
    public class SolrConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public SolrServerElements SolrServers
        {
            get { return (SolrServerElements)base[""]; }
        }
    }
}


#endif
