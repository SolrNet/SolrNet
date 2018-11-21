namespace Ninject.Integration.SolrNet
{
    public interface ISolrServer
    {
        string Id { get; set; }
        string Url { get; set; }
        string DocumentType { get; set; }
    }
}
