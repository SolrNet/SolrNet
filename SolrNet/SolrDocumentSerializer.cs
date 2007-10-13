using System;
using System.Reflection;
using SolrNet.Tests;

namespace SolrNet {
	public class SolrDocumentSerializer<T> : ISolrDocumentSerializer<T> where T : ISolrDocument {
		public string Serialize(T doc) {
			string r = "<doc>"; // HACK replace this with xml
			foreach (PropertyInfo p in typeof (T).GetProperties()) {
				object[] atts = p.GetCustomAttributes(typeof (SolrField), true);
				if (atts.Length > 0) {
					SolrField att = (SolrField) atts[0];
					string name = att.FieldName ?? p.Name;
					r += Serialize(name, p.GetValue(doc, null));
				}
			}
			r += "</doc>";
			return r;
		}

		public string Serialize(string fieldName, object fieldValue) {
			return string.Format("<field name=\"{0}\">{1}</field>", fieldName, fieldValue); // TODO escape values
		}
	}
}