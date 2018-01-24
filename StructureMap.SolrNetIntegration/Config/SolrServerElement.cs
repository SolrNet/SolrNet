
namespace StructureMap.SolrNetIntegration.Config 
{
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
}
