using System.Collections.Generic;
using System.Xml;

namespace SolrNet {
	public class DeleteCommand : ISolrCommand {
		private bool? fromPending;
		private bool? fromCommitted;
		private ISolrDeleteParam deleteParam;

		public DeleteCommand(ISolrDeleteParam deleteParam) {
			this.deleteParam = deleteParam;
		}

		public bool? FromPending {
			get { return fromPending; }
			set { fromPending = value; }
		}

		public bool? FromCommitted {
			get { return fromCommitted; }
			set { fromCommitted = value; }
		}

		public ISolrDeleteParam DeleteParam {
			get { return deleteParam; }
		}

		public string Execute(ISolrConnection connection) {
			XmlDocument xml = new XmlDocument();
			XmlNode deleteNode = xml.CreateElement("delete");
			foreach (KeyValuePair<bool?, string> p in new KeyValuePair<bool?, string>[] {new KeyValuePair<bool?, string>(fromPending, "fromPending"), new KeyValuePair<bool?, string>(fromCommitted, "fromCommitted")}) {
				if (p.Key.HasValue) {
					XmlAttribute att = xml.CreateAttribute(p.Value);
					att.InnerText = p.Key.Value.ToString().ToLower();
					deleteNode.Attributes.Append(att);
				}
			}
			deleteNode.InnerXml = deleteParam.ToXmlNode().OuterXml;
			return connection.Post("/update", deleteNode.OuterXml);
		}
	}
}