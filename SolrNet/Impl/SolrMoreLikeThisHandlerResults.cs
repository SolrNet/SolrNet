using System;
using System.Collections.Generic;

namespace SolrNet.Impl
{
    public class SolrMoreLikeThisHandlerResults<T> : AbstractSolrQueryResults<T>
    {
        public T Match { get; set; }

        public IList<KeyValuePair<string, float>> InterestingTerms { get; set; }
    }
}
