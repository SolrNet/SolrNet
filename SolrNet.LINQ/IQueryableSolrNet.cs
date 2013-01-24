using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace SolrNet.LINQ
{
    public interface IQueryableSolrNet<TData> : IOrderedQueryable<TData>
    {
        SolrQuery GetSolrQuery(out QueryOptions queryOptions);
    }
}
