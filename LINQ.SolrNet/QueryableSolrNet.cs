using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using SolrNet.Commands.Parameters;
using SolrNet;

namespace LINQ.SolrNet
{
    public class QueryableSolrNet<TData> : IQueryableSolrNet<TData>
    {
        #region Constructors
        /// <summary>
        /// This constructor is called by the client to create the data source.
        /// </summary>
        public QueryableSolrNet(ISolrBasicReadOnlyOperations<TData> solrOperation)
        {
            Provider = new SolrNetQueryProvider<TData>(solrOperation);
            Expression = Expression.Constant(this);
        }

        /// <summary>
        /// This constructor is called by Provider.CreateQuery().
        /// </summary>
        /// <param name="expression"></param>
        public QueryableSolrNet(SolrNetQueryProvider<TData> provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<TData>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            Provider = provider;
            Expression = expression;
        }
        #endregion

        #region Properties

        public IQueryProvider Provider { get; private set; }
        public Expression Expression { get; private set; }

        public Type ElementType
        {
            get { return typeof(TData); }
        }

        

        #endregion

        #region Enumerators
        public IEnumerator<TData> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<TData>>(Expression)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<System.Collections.IEnumerable>(Expression)).GetEnumerator();
        }
        #endregion

        #region Methods

        public SolrQuery GetSolrQuery(out QueryOptions queryOptions)
        {

            return ((SolrNetQueryProvider<TData>)Provider).GetSolrQuery(Expression, out queryOptions);
        }


        internal void SetAdditinalParameters(Dictionary<string, string> extraParams)
        {
            ((SolrNetQueryProvider<TData>)Provider).SetAdditinalParameters(extraParams);
        }

      
        internal void SetPaging(int start, int rows)
        {

            ((SolrNetQueryProvider<TData>)Provider).SetPaging(start, rows);
        }

        #endregion
    }
}
