using System.Collections.Generic;
using SolrNet.Attributes;

namespace SolrNet {
	public class SolrQueryByExample<T> : ISolrQuery where T : ISolrDocument {
		public string q;

		public SolrQueryByExample(T document) {
			var fields = new List<ISolrQuery>();
			foreach (var property in typeof (T).GetProperties()) {
				var atts = property.GetCustomAttributes(typeof (SolrFieldAttribute), true);
				if (atts.Length > 0) {
					var f = (SolrFieldAttribute) atts[0];
					var fieldName = f.FieldName ?? property.Name;
					var v = property.GetValue(document, null);
					if (v != null)
						fields.Add(new SolrQueryByField(fieldName, v.ToString()));
				}
			}
			q = new SolrMultipleCriteriaQuery(fields).Query;
		}

		/// <summary>
		/// query string
		/// </summary>
		public string Query {
			get { return q; }
		}
	}
}