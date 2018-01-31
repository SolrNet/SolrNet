using System.Collections;
using System.Collections.Generic;
namespace StructureMap.SolrNetIntegration.Config 
{
    public class SolrServers: IEnumerable
    {
        public List<SolrServerElement> SolrServerElements { get; set; }

        public void Add(SolrServerElement solrSevServer)
        {
            SolrServerElements.Add(solrSevServer);
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


}
