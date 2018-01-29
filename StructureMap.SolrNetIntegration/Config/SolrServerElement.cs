using System.Configuration;

namespace StructureMap.SolrNetIntegration.Config 
{
#if NETCOREAPP2_0 || NETSTANDARD2_0
    public class SolrServerElement
    {

        public string Id
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public string DocumentType
        {
            get;
            set;
        }

        public override string ToString() 
        {
            return $"Id: {Id} Url: {Url} DocType: {DocumentType}";
        }
    }
#else
    public class SolrServerElement : ConfigurationElement 
    {
        private const string ID = "id";
        private const string URL = "url";
        private const string DOCUMENT_TYPE = "documentType";

        [ConfigurationProperty(ID, IsKey = true, IsRequired = true)]
        public string Id
        {
            get { return base[ID].ToString(); }
            set { base[ID] = value; }
        }

        [ConfigurationProperty(URL, IsKey = true, IsRequired = true)]
        public string Url 
        {
            get { return base[URL].ToString(); }
            set { base[URL] = value; }
        }

        [ConfigurationProperty(DOCUMENT_TYPE, IsKey = true, IsRequired = true)]
        public string DocumentType
        {
            get { return base[DOCUMENT_TYPE].ToString(); }
            set { base[DOCUMENT_TYPE] = value; }
        }

        public override string ToString() 
        {
            return string.Format("Id: {0} Url: {1} DocType: {2}", Id, Url, DocumentType);
        }
}
#endif
}
