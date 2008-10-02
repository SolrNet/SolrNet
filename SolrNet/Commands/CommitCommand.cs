using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Commands {
	public class CommitCommand : ISolrCommand {

		/// <summary>
		/// Block until index changes are flushed to disk
		/// Default is true
		/// </summary>
		public bool? WaitFlush { get; set; }

		/// <summary>
		/// Block until a new searcher is opened and registered as the main query searcher, making the changes visible. 
		/// Default is true
		/// </summary>
		public bool? WaitSearcher { get; set; }

		public string Execute(ISolrConnection connection) {
			var xml = new XmlDocument();
			var node = xml.CreateElement("commit");
			foreach (var p in new[] {new KeyValuePair<bool?, string>(WaitSearcher, "waitSearcher"), new KeyValuePair<bool?, string>(WaitFlush, "waitFlush")}) {
				if (p.Key.HasValue) {
					var att = xml.CreateAttribute(p.Value);
					att.InnerText = p.Key.Value.ToString().ToLower();
					node.Attributes.Append(att);
				}
			}
			return connection.Post("/update", node.OuterXml);
		}
	}
}