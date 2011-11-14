using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet
{
    /// <summary>
    /// MoreLikeThisHandler stream.url query
    /// </summary>
    public class SolrMoreLikeThisHandlerStreamUrlQuery : ISolrMoreLikeThisHandlerQuery
    {
        private readonly string _query;

        public SolrMoreLikeThisHandlerStreamUrlQuery(string url)
        {
            this._query = url;
        }

        public SolrMoreLikeThisHandlerStreamUrlQuery(Uri url)
        {
            this._query = url.AbsoluteUri;
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
