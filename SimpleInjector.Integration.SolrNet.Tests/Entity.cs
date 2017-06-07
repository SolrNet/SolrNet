using SolrNet.Attributes;

namespace SimpleInjector.Integration.SolrNet.Tests
{
    class Entity
    {
        [SolrUniqueKey(fieldName: "id")]
        public string Id { get; set; }
    }
}
