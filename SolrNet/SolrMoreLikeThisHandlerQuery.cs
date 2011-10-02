using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet
{
    public class SolrMoreLikeThisHandlerQuery : ISolrMoreLikeThisHandlerQuery
    {
        private readonly string _query;

        public SolrMoreLikeThisHandlerQuery(string query)
        {
            this._query = query;
        }

        public string Query
        {
            get
            {
                return this._query;
            }
        }
    }
}
