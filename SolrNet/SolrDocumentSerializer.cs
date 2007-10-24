using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using SolrNet.Exceptions;

namespace SolrNet {
	/// <summary>
	/// Serializes a Solr document to xml
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class SolrDocumentSerializer<T> : ISolrDocumentSerializer<T> where T : ISolrDocument {
		/// <summary>
		/// Serializes a Solr document to xml
		/// </summary>
		/// <param name="doc">document to serialize</param>
		/// <returns>serialized document</returns>
		public XmlDocument Serialize(T doc) {
			XmlDocument xml = new XmlDocument();
			XmlElement docNode = xml.CreateElement("doc");
			foreach (PropertyInfo p in typeof (T).GetProperties()) {
				object[] atts = p.GetCustomAttributes(typeof (SolrFieldAttribute), true);
				if (atts.Length > 0) {
					if (p.GetValue(doc, null) == null)
						continue;
					SolrFieldAttribute att = (SolrFieldAttribute) atts[0];
					XmlElement fieldNode = xml.CreateElement("field");
					XmlAttribute nameAtt = xml.CreateAttribute("name");
					nameAtt.InnerText = att.FieldName ?? p.Name;
					fieldNode.Attributes.Append(nameAtt);
					// TODO the following should be pluggable via SolrFieldSerializer or something similar
					if (p.PropertyType != typeof (string) && typeof (IEnumerable).IsAssignableFrom(p.PropertyType)) {
						fieldNode.InnerXml = (GetPropertyValue(doc, p) ?? "").ToString();
					} else if (p.PropertyType == typeof (DateTime) || p.PropertyType == typeof (Nullable<DateTime>)) {
						fieldNode.InnerText = ((DateTime) p.GetValue(doc, null)).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
					} else if (p.PropertyType == typeof (bool)) {
						fieldNode.InnerText = p.GetValue(doc, null).ToString().ToLower();
					} else {
						fieldNode.InnerText = (p.GetValue(doc, null) ?? "").ToString();
					}
					docNode.AppendChild(fieldNode);
				}
			}
			xml.AppendChild(docNode);
			return xml;
		}

		private static IDictionary<Type, string> solrTypes;

		static SolrDocumentSerializer() {
			solrTypes = new Dictionary<Type, string>();
			solrTypes[typeof (int)] = "int";
			solrTypes[typeof (string)] = "str";
			solrTypes[typeof (bool)] = "bool";
			solrTypes[typeof (DateTime)] = "date";
		}

		private static object GetPropertyValue(T doc, PropertyInfo p) {
			try {
				XmlDocument xml = new XmlDocument();
				XmlNode root = xml.CreateElement("arr");
				xml.AppendChild(root);
				IEnumerable e = (IEnumerable) p.GetValue(doc, null);
				foreach (object o in e) {
					XmlNode fn = xml.CreateElement(solrTypes[o.GetType()]);
					fn.InnerText = o.ToString();
					root.AppendChild(fn);
				}
				return root.OuterXml;
			} catch (Exception e) {
				throw new CollectionTypeNotSupportedException(e, p.PropertyType);
			}
		}
	}
}