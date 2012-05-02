using System.Collections.Generic;
using System.Configuration;

namespace Unity.SolrNetIntegration.Config {
    public class SolrServers : ConfigurationElementCollection, IEnumerable<SolrServerElement> {
        public override ConfigurationElementCollectionType CollectionType {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override string ElementName {
            get { return "server"; }
        }

        public void Add(SolrServerElement configurationElement) {
            base.BaseAdd(configurationElement);
        }

        protected override ConfigurationElement CreateNewElement() {
            return new SolrServerElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            var solrServerElement = (SolrServerElement) element;
            return solrServerElement.Url + solrServerElement.DocumentType;
        }

        IEnumerator<SolrServerElement> IEnumerable<SolrServerElement>.GetEnumerator() {
            var c = (ConfigurationElementCollection) this;
            foreach (SolrServerElement e in c) {
                yield return e;
            }
        }
    }
}