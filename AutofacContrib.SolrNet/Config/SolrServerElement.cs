using System.Configuration;

namespace AutofacContrib.SolrNet.Config {
    /// <summary>
    /// Solr instance or core config
    /// </summary>
    public class SolrServerElement : ConfigurationElement {
        private const string ID = "id";
        private const string URL = "url";
        private const string DOCUMENT_TYPE = "documentType";

        /// <summary>
        /// Core / instance ID
        /// </summary>
        [ConfigurationProperty(ID, IsKey = true, IsRequired = true)]
        public string Id {
            get { return base[ID].ToString(); }
            set { base[ID] = value; }
        }

        /// <summary>
        /// Core / instance URL
        /// </summary>
        [ConfigurationProperty(URL, IsKey = true, IsRequired = true)]
        public string Url {
            get { return base[URL].ToString(); }
            set { base[URL] = value; }
        }

        /// <summary>
        /// Core / instance document type
        /// </summary>
        [ConfigurationProperty(DOCUMENT_TYPE, IsKey = true, IsRequired = true)]
        public string DocumentType {
            get { return base[DOCUMENT_TYPE].ToString(); }
            set { base[DOCUMENT_TYPE] = value; }
        }

        public override string ToString() {
            return string.Format("Id: {0} Url: {1} DocType: {2}", Id, Url, DocumentType);
        }
    }
}