using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Commands {
	public class CommitCommand : ISolrCommand {
		private bool? waitFlush;
		private bool? waitSearcher;

		public bool? WaitFlush {
			get { return waitFlush; }
			set { waitFlush = value; }
		}

		public bool? WaitSearcher {
			get { return waitSearcher; }
			set { waitSearcher = value; }
		}

		public string Execute(ISolrConnection connection) {
			XmlDocument xml = new XmlDocument();
			XmlNode node = xml.CreateElement("commit");
			foreach (KeyValuePair<bool?, string> p in new KeyValuePair<bool?, string>[] {new KeyValuePair<bool?, string>(waitSearcher, "waitSearcher"), new KeyValuePair<bool?, string>(waitFlush, "waitFlush")}) {
				if (p.Key.HasValue) {
					XmlAttribute att = xml.CreateAttribute(p.Value);
					att.InnerText = p.Key.Value.ToString().ToLower();
					node.Attributes.Append(att);
				}
			}
			return connection.Post("/update", node.OuterXml);
		}
	}
}