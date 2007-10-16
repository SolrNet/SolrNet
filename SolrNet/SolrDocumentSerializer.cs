using System.Reflection;
using System.Text;
using System.Xml;

namespace SolrNet {
	public class SolrDocumentSerializer<T> : ISolrDocumentSerializer<T> where T : ISolrDocument {

		public string Serialize(T doc) {
			XmlDocument xml = new XmlDocument();
			XmlElement docNode = xml.CreateElement("doc");
			foreach (PropertyInfo p in typeof(T).GetProperties()) {
				object[] atts = p.GetCustomAttributes(typeof(SolrField), true);
				if (atts.Length > 0) {
					SolrField att = (SolrField)atts[0];
					XmlElement fieldNode = xml.CreateElement("field");
					XmlAttribute nameAtt = xml.CreateAttribute("name");
					nameAtt.InnerText = att.FieldName ?? p.Name;
					fieldNode.Attributes.Append(nameAtt);
					fieldNode.InnerText = p.GetValue(doc, null).ToString();
					docNode.AppendChild(fieldNode);
				}
			}
			xml.AppendChild(docNode);
			return xml.OuterXml;
		}
	}
}