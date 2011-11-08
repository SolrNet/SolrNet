using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet;

namespace LINQ.SolrNet.Helpers
{
    public static class SolrNetLinqExt
    {


        public static QueryableSolrNet<T> Pagination<T>(this IEnumerable<T> enumerable, int start, int rows)
        {
            QueryableSolrNet<T> queryableSolrNet = enumerable as QueryableSolrNet<T>;
            queryableSolrNet.SetPaging(start, rows);
            return queryableSolrNet;
        }


        public static QueryableSolrNet<T> AdditinalParameters<T>(this IEnumerable<T> enumerable, Dictionary<string, string> extraParams)
        {
            QueryableSolrNet<T> queryableSolrNet = enumerable as QueryableSolrNet<T>;
            queryableSolrNet.SetAdditinalParameters(extraParams);
            return queryableSolrNet;
        }


        public static QueryableSolrNet<T> AsQueryable<T>(this ISolrBasicReadOnlyOperations<T> solrOperation)
        {

            return new QueryableSolrNet<T>(solrOperation);

        }


        //Default
        public static bool DefaultField(this object obj, string value)
        {
            throw new NotSupportedException("Should Not Call this Method");
        }

        //Boost
        public static bool Boost(this bool predicate, int boostVal)
        {
            throw new NotSupportedException("Should Not Call this Method");
        }


        //Boost
        public static string DynamicField(this object obj, string value)
        {
            throw new NotSupportedException("Should Not Call this Method");
        }

        //any item in collection
        public static TSource AnyItem<TSource>(this IEnumerable<TSource> source)
        {
            throw new NotSupportedException("Should Not Call this Method");
        }


    }

}
