using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl
{
    public class SolrMoreLikeThisHandlerResults<T> : AbstractSolrQueryResults<T>, ISolrMoreLikeThisQueryResults<T>
    {
        public SolrMoreLikeThisHandlerResults()
        {
            this.InterestingTerms = new List<KeyValuePair<string, float>>();
        }

        public T Match { get; set; }

        public IList<KeyValuePair<string, float>> InterestingTerms { get; set; }
    }
}
