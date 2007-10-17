using System;
using System.Reflection;
using System.Xml;
using SolrNet.Exceptions;
using SolrNet.Tests;

namespace SolrNet {
	public class SolrQueryResultParser<T> : ISolrQueryResultParser<T> where T : ISolrDocument, new() {
		public ISolrQueryResults<T> Parse(string r) {
			SolrQueryResults<T> results = new SolrQueryResults<T>();
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(r);
			XmlNode resultNode = xml.SelectSingleNode("response/result");
			results.NumFound = Convert.ToInt32(resultNode.Attributes["numFound"].InnerText);
			foreach (XmlNode docNode in xml.SelectNodes("response/result/doc")) {
				results.Add(ParseDocument(docNode));
			}
			return results;
		}

		private delegate bool BoolFunc(PropertyInfo[] p);

		public T ParseDocument(XmlNode node) {
			T doc = new T();
			PropertyInfo[] properties = typeof (T).GetProperties();
			// TODO this is a mess, clean it up
			foreach (XmlNode field in node.ChildNodes) {
				string fieldName = field.Attributes["name"].InnerText;
				// first look up attribute SolrField with this FieldName
				bool found = new BoolFunc(delegate {
				                          	foreach (PropertyInfo property in properties) {
				                          		object[] atts = property.GetCustomAttributes(typeof (SolrField), true);
				                          		if (atts.Length > 0) {
				                          			SolrField att = (SolrField) atts[0];
				                          			if (att.FieldName == fieldName) {
				                          				property.SetValue(doc, Convert.ChangeType(field.InnerText, property.PropertyType),
				                          				                  null);
				                          				return true;
				                          			}
				                          		}
				                          	}
				                          	return false;
				                          })(properties);
				// if not found, look up by property name
				if (!found) {
					foreach (PropertyInfo property in properties) {
						if (property.Name == fieldName) {
							property.SetValue(doc, field.InnerText, null);
							found = true;
							break;
						}
					}
				}
				// no property found with this name, wrong class map
				if (!found) {
					FieldNotFoundException ex = new FieldNotFoundException(string.Format("Field '{0}' not found on class {1}", fieldName, typeof(T)));
					ex.FieldName = fieldName;
					throw ex;
				}
			}
			return doc;
		}
	}
}