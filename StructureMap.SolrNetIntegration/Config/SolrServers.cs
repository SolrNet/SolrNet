using System.Configuration;

namespace StructureMap.SolrNetIntegration.Config 
{
    public class SolrServers : ConfigurationElementCollection 
    {
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