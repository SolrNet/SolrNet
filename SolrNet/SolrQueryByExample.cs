using System.Collections.Generic;
using System.Reflection;

namespace SolrNet {
	public class SolrQueryByExample<T> : ISolrQuery<T> where T : ISolrDocument {
		public string q;

		public SolrQueryByExample(T document) {
			IList<ISolrQuery<T>> fields = new List<ISolrQuery<T>>();
			foreach (PropertyInfo property in typeof (T).GetProperties()) {
				object[] atts = property.GetCustomAttributes(typeof (SolrField), true);
				if (atts.Length > 0) {
					SolrField f = (SolrField) atts[0];
					string fieldName = f.FieldName ?? property.Name;
					object v = property.GetValue(document, null);
					if (v != null)
						fields.Add(new SolrQueryByField<T>(fieldName, v.ToString()));
				}
			}
			q = new SolrMultipleCriteriaQuery<T>(fields).Query;
		}

		/// <summary>
		/// query string
		/// </summary>
		public string Query {
			get { return q; }
		}
	}
}