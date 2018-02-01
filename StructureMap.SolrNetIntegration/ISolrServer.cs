#if NET46
#endif

namespace StructureMap.SolrNetIntegration
{
    public interface ISolrServer
    {
        string Id { get; set; }
        string Url { get; set; }
        string DocumentType { get; set; }
    }
}
