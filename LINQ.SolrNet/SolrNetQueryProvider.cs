using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolrNet;
using SolrNet.LINQ.Helpers;
using System.Linq.Expressions;
using SolrNet.Commands.Parameters;

namespace SolrNet.LINQ
{
    public class SolrNetQueryProvider<TData> : IQueryProvider
    {
        private ISolrBasicReadOnlyOperations<TData> _solrOperation;
        private readonly IReadOnlyMappingManager mapper;
        private QueryOptions _queryOptions = new QueryOptions();

        public SolrNetQueryProvider(ISolrBasicReadOnlyOperations<TData> solrOperation, IReadOnlyMappingManager mapper)
        {
            _solrOperation = solrOperation;
            this.mapper = mapper;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(QueryableSolrNet<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == typeof(TData))
            {
                SolrNetQueryProvider<TElement> solrNetQueryProvider = this as SolrNetQueryProvider<TElement>;
                return new QueryableSolrNet<TElement>(solrNetQueryProvider, expression);
            }
            throw new NotSupportedException();
        }



        public object Execute(Expression expression)
        {
            QueryOptions queryOptions;
            SolrQuery query = GetSolrQuery(expression, out queryOptions);
            return _solrOperation.Query(query, queryOptions);

        }


        public TResult Execute<TResult>(Expression expression)
        {
            bool IsEnumerable = (typeof(TResult).Name == "IEnumerable`1");

            return (TResult)Execute(expression);
        }



        internal SolrQuery GetSolrQuery(Expression expression, out QueryOptions queryOptions)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            SolrQueryTranslator qt = new SolrQueryTranslator(elementType, mapper);
            expression = Evaluator.PartialEval(expression);
            var queryStr = qt.Translate(expression);
            if (qt.SortItems.Count > 0)
            {
                _queryOptions.OrderBy = qt.SortItems;
            }

            queryOptions = _queryOptions;

            return new SolrQuery(queryStr);
        }

        internal void SetPaging(int start, int rows)
        {
            _queryOptions.Start = start;
            _queryOptions.Rows = rows;

        }

        internal void SetAdditinalParameters(Dictionary<string, string> extraParams)
        {
            _queryOptions.ExtraParams = extraParams;
        }
    }
}
