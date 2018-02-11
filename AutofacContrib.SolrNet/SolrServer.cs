namespace AutofacContrib.SolrNet
{
    public class SolrServer : ISolrServer
    {
        public SolrServer() { }

        public SolrServer(string id, string url, string documentType)
        {
            this.Id = id;
            this.Url = url;
            this.DocumentType = documentType;
        }

        public string Id { get; set; }

        public string Url { get; set; }

        public string DocumentType { get; set; }

        public override string ToString() => $"Id: {Id} Url: {Url} DocType: {DocumentType}";
    }
}
