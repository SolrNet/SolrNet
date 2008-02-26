using System.Collections.Generic;
using Rhino.Commons;

namespace SolrNet.DSL.v3._5 {
	/// <summary>
	/// Solr DSL Entry point
	/// </summary>
	public static class Solr {
		// TODO implement an inner non-static inheritable object to delegate methods. I know what I mean.
		public static DeleteBy Delete {
			get { return DSL.Solr.Delete; }
		}

		public static void Add<T>(T document) where T : ISolrDocument {
			DSL.Solr.Add(document);
		}

		public static void Commit() {
			DSL.Solr.Commit();
		}

		public static void Commit(bool waitFlush, bool waitSearcher) {
			DSL.Solr.Commit(waitFlush, waitSearcher);
		}

		public static void Optimize() {
			DSL.Solr.Optimize();
		}

		public static ISolrQueryResults<T> Query<T>(ISolrQuery q) where T : ISolrDocument, new() {
			return DSL.Solr.Query<T>(q);
		}

		public static ISolrQueryResults<T> Query<T>(string q) where T : ISolrDocument, new() {
			return DSL.Solr.Query<T>(q);
		}

		public static ISolrQueryResults<T> Query<T>(ISolrQuery q, int start, int rows) where T : ISolrDocument, new() {
			return DSL.Solr.Query<T>(q, start, rows);
		}

		public static ISolrQueryResults<T> Query<T>(string q, int start, int rows) where T : ISolrDocument, new() {
			return DSL.Solr.Query<T>(q, start, rows);			
		}


		public static void Optimize(bool waitFlush, bool waitSearcher) {
			DSL.Solr.Optimize(waitFlush, waitSearcher);
		}

		public static ISolrQueryResults<T> Query<T>(SolrQuery query, SortOrder order) where T : ISolrDocument, new() {
			return DSL.Solr.Query<T>(query, order);
		}

		public static ISolrQueryResults<T> Query<T>(SolrQuery query, ICollection<SortOrder> orders) where T : ISolrDocument, new() {
			return DSL.Solr.Query<T>(query, orders);
		}

		public static readonly string SolrConnectionKey = DSL.Solr.SolrConnectionKey;

		/// <summary>
		/// thread-local or webcontext-local connection
		/// </summary>
		/// <seealso cref="http://www.ayende.com/Blog/archive/7447.aspx"/>
		/// <seealso cref="http://rhino-tools.svn.sourceforge.net/svnroot/rhino-tools/trunk/rhino-commons/Rhino.Commons/LocalDataImpl/"/>
		public static ISolrConnection Connection {
			private get { return (ISolrConnection) Local.Data[SolrConnectionKey]; }
			set { Local.Data[SolrConnectionKey] = value; }
		}

		public static IDSLQuery35<T> Query<T>() where T : ISolrDocument, new() {
			return new DSLQuery35<T>(Connection);
		}
	}
}