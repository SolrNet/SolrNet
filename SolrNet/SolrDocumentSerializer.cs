using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using SolrNet.Attributes;
using SolrNet.Exceptions;

namespace SolrNet {
	/// <summary>
	/// Serializes a Solr document to xml
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class SolrDocumentSerializer<T> : ISolrDocumentSerializer<T> {
		/// <summary>
		/// Serializes a Solr document to xml
		/// </summary>
		/// <param name="doc">document to serialize</param>
		/// <returns>serialized document</returns>
		public XmlDocument Serialize(T doc) {
			var xml = new XmlDocument();
			var docNode = xml.CreateElement("doc");
			foreach (PropertyInfo p in typeof (T).GetProperties()) {
				var atts = p.GetCustomAttributes(typeof (SolrFieldAttribute), true);
				if (atts.Length > 0) {
					if (p.GetValue(doc, null) == null)
						continue;
					var att = (SolrFieldAttribute) atts[0];
					var fieldNode = xml.CreateElement("field");
					var nameAtt = xml.CreateAttribute("name");
					nameAtt.InnerText = att.FieldName ?? p.Name;
					fieldNode.Attributes.Append(nameAtt);
					// TODO the following should be pluggable via SolrFieldSerializer or something similar
					if (p.PropertyType != typeof (string) && typeof (IEnumerable).IsAssignableFrom(p.PropertyType)) {
						foreach (object o in p.GetValue(doc, null) as IEnumerable) {
							fieldNode.InnerText = o.ToString();
							docNode.AppendChild(fieldNode.CloneNode(true));
						}
						fieldNode = null;
						//fieldNode.InnerXml = (GetPropertyValue(doc, p) ?? "").ToString();
					} else if (p.PropertyType == typeof (DateTime) || p.PropertyType == typeof (DateTime?)) {
						fieldNode.InnerText = ((DateTime) p.GetValue(doc, null)).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
					} else if (p.PropertyType == typeof (bool)) {
						fieldNode.InnerText = p.GetValue(doc, null).ToString().ToLower();
					} else if (typeof(IFormattable).IsAssignableFrom(p.PropertyType)) {
                        var v = (IFormattable)p.GetValue(doc, null);
					    fieldNode.InnerText = v.ToString(null, CultureInfo.InvariantCulture);
					} else {
					    fieldNode.InnerText = p.GetValue(doc, null).ToString();
					}
					if (fieldNode != null)
						docNode.AppendChild(fieldNode);
				}
			}
			xml.AppendChild(docNode);
			return xml;
		}

		private static readonly IDictionary<Type, string> solrTypes;

		static SolrDocumentSerializer() {
			solrTypes = new Dictionary<Type, string>();
			solrTypes[typeof (int)] = "int";
			solrTypes[typeof (string)] = "str";
			solrTypes[typeof (bool)] = "bool";
			solrTypes[typeof (DateTime)] = "date";
		}

		private static object GetPropertyValue(T doc, PropertyInfo p) {
			try {
				var xml = new XmlDocument();
				var root = xml.CreateElement("arr");
				xml.AppendChild(root);
				var e = (IEnumerable) p.GetValue(doc, null);
				foreach (object o in e) {
					var fn = xml.CreateElement(solrTypes[o.GetType()]);
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