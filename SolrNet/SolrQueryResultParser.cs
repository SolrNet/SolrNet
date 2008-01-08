using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using SolrNet.Exceptions;

namespace SolrNet {
	/// <summary>
	/// Default query results parser.
	/// Parses xml query results
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class SolrQueryResultParser<T> : ISolrQueryResultParser<T> where T : ISolrDocument, new() {
		private static readonly IDictionary<string, Type> solrTypes;

		static SolrQueryResultParser() {
			solrTypes = new Dictionary<string, Type>();
			solrTypes["int"] = typeof (int);
			solrTypes["str"] = typeof (string);
			solrTypes["bool"] = typeof (bool);
			solrTypes["date"] = typeof (DateTime);
		}

		/// <summary>
		/// Parses solr's xml response
		/// </summary>
		/// <param name="r">solr xml response</param>
		/// <returns>query results</returns>
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

		public void SetProperty(T doc, PropertyInfo prop, XmlNode field) {
			if (field.Name == "arr") {
				prop.SetValue(doc, GetCollectionProperty(field, prop), null);
			} else if (prop.PropertyType == typeof (DateTime)) {
				prop.SetValue(doc, DateTime.ParseExact(field.InnerText, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture), null);
			} else {
				prop.SetValue(doc, Convert.ChangeType(field.InnerText, prop.PropertyType), null);
			}
		}

		private static object GetCollectionProperty(XmlNode field, PropertyInfo prop) {
			try {
				Type[] genericTypes = prop.PropertyType.GetGenericArguments();
				if (genericTypes.Length == 1) {
					// ICollection<int>, etc
					return GetGenericCollectionProperty(field, genericTypes);
				} else if (prop.PropertyType.IsArray) {
					// int[], string[], etc
					return GetArrayProperty(field, prop);
				} else if (prop.PropertyType.IsInterface) {
					// ICollection
					return GetNonGenericCollectionProperty(field);
				}
			} catch (Exception e) {
				throw new CollectionTypeNotSupportedException(e, prop.PropertyType);
			}
			throw new CollectionTypeNotSupportedException(prop.PropertyType);
		}

		private static IList GetNonGenericCollectionProperty(XmlNode field) {
			IList l = new ArrayList();
			foreach (XmlNode arrayValueNode in field.ChildNodes) {
				l.Add(Convert.ChangeType(arrayValueNode.InnerText, solrTypes[arrayValueNode.Name]));
			}
			return l;
		}

		private static Array GetArrayProperty(XmlNode field, PropertyInfo prop) {
			// int[], string[], etc
			Array arr = (Array) Activator.CreateInstance(prop.PropertyType, new object[] {field.ChildNodes.Count});
			Type arrType = Type.GetType(prop.PropertyType.ToString().Replace("[]", ""));
			int i = 0;
			foreach (XmlNode arrayValueNode in field.ChildNodes) {
				arr.SetValue(Convert.ChangeType(arrayValueNode.InnerText, arrType), i);
				i++;
			}
			return arr;
		}

		private static IList GetGenericCollectionProperty(XmlNode field, Type[] genericTypes) {
			// ICollection<int>, etc
			Type gt = genericTypes[0];
			IList l = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(gt));			
			foreach (XmlNode arrayValueNode in field.ChildNodes) {
				l.Add(Convert.ChangeType(arrayValueNode.InnerText, gt));
			}
			return l;
		}

		/// <summary>
		/// Builds a document from the correponding response xml node
		/// </summary>
		/// <param name="node">response xml node</param>
		/// <returns>populated document</returns>
		public T ParseDocument(XmlNode node) {
			T doc = new T();
			PropertyInfo[] properties = typeof (T).GetProperties();
			// TODO this is a mess, clean it up
			foreach (XmlNode field in node.ChildNodes) {
				string fieldName = field.Attributes["name"].InnerText;
				// first look up attribute SolrFieldAttribute with this FieldName
				bool found = new BoolFunc(delegate {
				                          	foreach (PropertyInfo property in properties) {
				                          		object[] atts = property.GetCustomAttributes(typeof (SolrFieldAttribute), true);
				                          		if (atts.Length > 0) {
				                          			SolrFieldAttribute att = (SolrFieldAttribute) atts[0];
				                          			if (att.FieldName == fieldName) {
				                          				SetProperty(doc, property, field);
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
							SetProperty(doc, property, field);
							found = true;
							break;
						}
					}
				}
				// no property found with this name, wrong class map
				//if (!found) {
				//  FieldNotFoundException ex =
				//    new FieldNotFoundException(string.Format("Field '{0}' not found on class {1}", fieldName, typeof (T)));
				//  ex.FieldName = fieldName;
				//  throw ex;
				//}
			}
			return doc;
		}
	}
}