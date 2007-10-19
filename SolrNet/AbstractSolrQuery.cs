using System;
using System.Collections.Generic;
using System.Text;

namespace SolrNet {
	public abstract class AbstractSolrQuery<T> : ISolrQuery<T> where T : ISolrDocument {
		private int? start;
		private int? rows;

		/// <summary>
		/// query string
		/// </summary>
		public abstract string Query { get; }

		/// <summary>
		/// Start row
		/// </summary>
		public virtual int? Start {
			get { return start; }
			set { start = value; }
		}

		/// <summary>
		/// Row count to get
		/// </summary>
		public virtual int? Rows {
			get { return rows; }
			set { rows = value; }
		}
	}
}
