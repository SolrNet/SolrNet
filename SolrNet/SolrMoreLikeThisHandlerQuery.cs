using System;

namespace SolrNet {
    /// <summary>
    /// Standard MoreLikeThisHandlerQuery
    /// </summary>
    public class SolrMoreLikeThisHandlerQuery : SolrMLTQuery {
        private readonly ISolrQuery query;

        public SolrMoreLikeThisHandlerQuery(ISolrQuery query) {
            this.query = query;
        }

        public ISolrQuery Query {
            get { return query; }
        }

        public override T Switch<T>(Func<ISolrQuery, T> q, Func<string, T> streamBody, Func<Uri, T> streamUrl) {
            return q(query);
        }
    }
}