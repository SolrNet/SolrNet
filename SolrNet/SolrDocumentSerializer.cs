using System.Reflection;
using System.Xml;

namespace SolrNet {
	public class SolrDocumentSerializer<T> : ISolrDocumentSerializer<T> where T : ISolrDocument {
		public XmlDocument Serialize(T doc) {
			XmlDocument xml = new XmlDocument();
			XmlElement docNode = xml.CreateElement("doc");
			foreach (PropertyInfo p in typeof (T).GetProperties()) {
				object[] atts = p.GetCustomAttributes(typeof (SolrField), true);
				if (atts.Length > 0) {
					SolrField att = (SolrField) atts[0];
					XmlElement fieldNode = xml.CreateElement("field");
					XmlAttribute nameAtt = xml.CreateAttribute("name");
					nameAtt.InnerText = att.FieldName ?? p.Name;
					fieldNode.Attributes.Append(nameAtt);
					object propertyValue = p.GetValue(doc, null);
					fieldNode.InnerText = (propertyValue ?? "").ToString();
					docNode.AppendChild(fieldNode);
				}
			}
			xml.AppendChild(docNode);
			return xml;
		}
	}
}