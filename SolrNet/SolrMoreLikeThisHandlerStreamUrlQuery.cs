using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet
{
    public class SolrMoreLikeThisHandlerStreamUrlQuery : ISolrMoreLikeThisHandlerQuery
    {
        private string _query;

        public SolrMoreLikeThisHandlerStreamUrlQuery(string url)
        {
            this._query = url;
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
