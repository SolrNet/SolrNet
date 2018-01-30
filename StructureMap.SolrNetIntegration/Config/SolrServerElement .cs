#if NET46
using System.Configuration;
#endif

namespace StructureMap.SolrNetIntegration.Config
{
    public class SolrServerElement
#if NET46
        : ConfigurationElement
#endif
    {
#if NET46
        private const string ID = "id";
        private const string URL = "url";
        private const string DOCUMENT_TYPE = "documentType";

        [ConfigurationProperty(ID, IsKey = true, IsRequired = true)]
#endif
        public string Id
        {
            get
#if NET46
            { return base[ID].ToString(); }
#else
;
#endif
            set
#if NET46
            { base[ID] = value; }
#else
;
#endif
        }

#if NET46
        [ConfigurationProperty(URL, IsKey = true, IsRequired = true)]
#endif
        public string Url
        {
            get
#if NET46
            {return base[URL].ToString();}
#else
;
#endif
            set
#if NET46 
            { base[URL] = value; }
#else
;
#endif
        }

#if NET46
        [ConfigurationProperty(DOCUMENT_TYPE, IsKey = true, IsRequired = true)]
#endif
        public string DocumentType
        {
            get
#if NET46
            { return base[DOCUMENT_TYPE].ToString(); }
#else
;
#endif
            set
#if NET46
            { base[DOCUMENT_TYPE] = value; }
#else
;
#endif
        }

        public override string ToString()
        {
            return string.Format("Id: {0} Url: {1} DocType: {2}", Id, Url, DocumentType);
        }
    }
}
