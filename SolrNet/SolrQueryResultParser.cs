using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
			var results = new SolrQueryResults<T>();
			var xml = new XmlDocument();
			xml.LoadXml(r);
			var resultNode = xml.SelectSingleNode("response/result");
			results.NumFound = Convert.ToInt32(resultNode.Attributes["numFound"].InnerText);
			var maxScore = resultNode.Attributes["maxScore"];
			if (maxScore != null) {
				results.MaxScore = double.Parse(maxScore.InnerText, CultureInfo.InvariantCulture.NumberFormat);
			}
			foreach (XmlNode docNode in xml.SelectNodes("response/result/doc")) {
				results.Add(ParseDocument(docNode));
			}
			var mainFacetNode = xml.SelectSingleNode("response/lst[@name='facet_counts']");
			if (mainFacetNode != null) {
				results.FacetQueries = ParseFacetQueries(mainFacetNode);
				results.FacetFields = ParseFacetFields(mainFacetNode);
			}

			var responseHeaderNode = xml.SelectSingleNode("response/lst[@name='responseHeader']");
			if (responseHeaderNode != null) {
				results.Header = ParseHeader(responseHeaderNode);
			}

			return results;
		}

		public IDictionary<string, ICollection<KeyValuePair<string, int>>> ParseFacetFields(XmlNode node) {
			var d = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
			foreach (XmlNode fieldNode in node.SelectSingleNode("lst[@name='facet_fields']").ChildNodes) {
				var field = fieldNode.Attributes["name"].Value;
				var c = new List<KeyValuePair<string, int>>();
				foreach (XmlNode facetNode in fieldNode.ChildNodes) {
					var key = facetNode.Attributes["name"].Value;
					var value = Convert.ToInt32(facetNode.InnerText);
					c.Add(new KeyValuePair<string, int>(key, value));
				}
				d[field] = c;
			}
			return d;
		}

		public IDictionary<string, int> ParseFacetQueries(XmlNode node) {
			var d = new Dictionary<string, int>();
			foreach (XmlNode fieldNode in node.SelectSingleNode("lst[@name='facet_queries']").ChildNodes) {
				var key = fieldNode.Attributes["name"].Value;
				var value = Convert.ToInt32(fieldNode.InnerText);
				d[key] = value;
			}
			return d;
		}

		private delegate bool BoolFunc(PropertyInfo[] p);

		public DateTime ParseDate(string s) {
			return DateTime.ParseExact(s, "yyyy-MM-dd'T'HH:mm:ss.FFF'Z'", CultureInfo.InvariantCulture);
		}

		public void SetProperty(T doc, PropertyInfo prop, XmlNode field) {
			// HACK too messy
			if (field.Name == "arr") {
				prop.SetValue(doc, GetCollectionProperty(field, prop), null);
			} else if (prop.PropertyType == typeof (double?)) {
				if (!string.IsNullOrEmpty(field.InnerText))
					prop.SetValue(doc, double.Parse(field.InnerText, CultureInfo.InvariantCulture), null);
			} else if (prop.PropertyType == typeof (DateTime)) {
				prop.SetValue(doc, ParseDate(field.InnerText), null);
			} else if (prop.PropertyType == typeof (DateTime?)) {
				if (!string.IsNullOrEmpty(field.InnerText))
					prop.SetValue(doc, ParseDate(field.InnerText), null);
			} else {
				var converter = TypeDescriptor.GetConverter(prop.PropertyType);
				if (converter.CanConvertFrom(typeof (string)))
					prop.SetValue(doc, converter.ConvertFromInvariantString(field.InnerText), null);
				else
					prop.SetValue(doc, Convert.ChangeType(field.InnerText, prop.PropertyType), null);
			}
		}

		private static object GetCollectionProperty(XmlNode field, PropertyInfo prop) {
			try {
				var genericTypes = prop.PropertyType.GetGenericArguments();
				if (genericTypes.Length == 1) {
					// ICollection<int>, etc
					return GetGenericCollectionProperty(field, genericTypes);
				}
				if (prop.PropertyType.IsArray) {
					// int[], string[], etc
					return GetArrayProperty(field, prop);
				}
				if (prop.PropertyType.IsInterface) {
					// ICollection
					return GetNonGenericCollectionProperty(field);
				}
			} catch (Exception e) {
				throw new CollectionTypeNotSupportedException(e, prop.PropertyType);
			}
			throw new CollectionTypeNotSupportedException(prop.PropertyType);
		}

		private static IList GetNonGenericCollectionProperty(XmlNode field) {
			var l = new ArrayList();
			foreach (XmlNode arrayValueNode in field.ChildNodes) {
				l.Add(Convert.ChangeType(arrayValueNode.InnerText, solrTypes[arrayValueNode.Name]));
			}
			return l;
		}

		private static Array GetArrayProperty(XmlNode field, PropertyInfo prop) {
			// int[], string[], etc
			var arr = (Array) Activator.CreateInstance(prop.PropertyType, new object[] {field.ChildNodes.Count});
			var arrType = Type.GetType(prop.PropertyType.ToString().Replace("[]", ""));
			int i = 0;
			foreach (XmlNode arrayValueNode in field.ChildNodes) {
				arr.SetValue(Convert.ChangeType(arrayValueNode.InnerText, arrType), i);
				i++;
			}
			return arr;
		}

		private static IList GetGenericCollectionProperty(XmlNode field, Type[] genericTypes) {
			// ICollection<int>, etc
			var gt = genericTypes[0];
			var l = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(gt));
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
			var doc = new T();
			var properties = typeof (T).GetProperties();
			// TODO this is a mess, clean it up
			foreach (XmlNode field in node.ChildNodes) {
				string fieldName = field.Attributes["name"].InnerText;
				// first look up attribute SolrFieldAttribute with this FieldName
				bool found = new BoolFunc(delegate {
					foreach (var property in properties) {
						var atts = property.GetCustomAttributes(typeof (SolrFieldAttribute), true);
						if (atts.Length > 0) {
							var att = (SolrFieldAttribute) atts[0];
							var fName = att.FieldName ?? property.Name;
							if (fName == fieldName) {
								SetProperty(doc, property, field);
								return true;
							}
						}
					}
					return false;
				})(properties);
				// if not found, look up by property name
				if (!found) {
					foreach (var property in properties) {
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

		public ResponseHeader ParseHeader(XmlNode node) {
			var r = new ResponseHeader();
			r.Status = int.Parse(node.SelectSingleNode("int[@name='status']").InnerText);
			r.QTime = int.Parse(node.SelectSingleNode("int[@name='QTime']").InnerText);
			r.Params = new Dictionary<string, string>();
			var paramNodes = node.SelectNodes("lst[@name='params']/str");
			if (paramNodes != null) {
				foreach (XmlNode n in paramNodes) {
					r.Params[n.Attributes["name"].InnerText] = n.InnerText;
				}				
			}
			return r;
		}
	}
}