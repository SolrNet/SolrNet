using System;

namespace SimpleInjector.Integration.SolrNet.Tests
{
    class EntityConnectionBuilder : IConnectionBuilder
    {
        private string host = "http://localhost:8920/solr/";

        public string Build(Type entityType)
        {
            return host + entityType.Name.ToLower() + "s";
        }

        public string Build<TEntity>()
        {
            return Build(typeof(TEntity));
        }
    }
}
