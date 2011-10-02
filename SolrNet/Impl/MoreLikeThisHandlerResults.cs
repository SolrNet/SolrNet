using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Impl
{
    public class MoreLikeThisHandlerResults<T>
    {
        public MoreLikeThisHandlerResults()
        {
            this.Results = new List<T>();
        }

        public int NumFound { get; set; }

        public double? MaxScore { get; set; }

        public T Match { get; set; }

        public IList<T> Results { get; private set; }
    }
}
