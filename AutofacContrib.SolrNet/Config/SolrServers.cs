using System.Configuration;

namespace AutofacContrib.SolrNet.Config
{
    public class SolrServers : ConfigurationElementCollection
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
}