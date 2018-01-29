using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace StructureMap.SolrNetIntegration.Config 
{
    public class SolrServers
#if NETCOREAPP2_0 || NETSTANDARD2_0
     : IEnumerable
    {
        public List<SolrServerElement> SolrServerElements { get; set; }

        public void Add(SolrServerElement solrSevServerElement)
        {
            SolrServerElements.Add(solrSevServerElement);
        }

        public SolrServers()
        {
            SolrServerElements = new List<SolrServerElement>();
        }

        public IEnumerator GetEnumerator()
        {
            return SolrServerElements.GetEnumerator();
        }
    }
#else
     : ConfigurationElementCollection 
    {
        public void Add(SolrServerElement configurationElement)
        {
            base.BaseAdd(configurationElement);
        }

        protected override ConfigurationElement CreateNewElement() 
        {
            return new SolrServerElement();
        }

        protected override object GetElementKey(ConfigurationElement element) 
        {
            var solrServerElement = (SolrServerElement)element;
            return solrServerElement.Url + solrServerElement.DocumentType;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName 
        {
            get { return "server"; }
        }
}
#endif
}
