using System;

namespace SimpleInjector.SolrNet
{
    internal class SolrCore
    {
        public string Id { get; private set; }
        public string Url { get; private set; }
        public Type DocumentType { get; private set; }

        public SolrCore(string id, Type documentType, string url)
        {
            Id = id;
            DocumentType = documentType;
            Url = url;
        }
    }
}
