using System.Collections.Generic;
using System.Xml;
using SolrNet.Commands.Parameters;

namespace SolrNet.Commands {
	public class DeleteCommand : ISolrCommand {
		private readonly ISolrDeleteParam deleteParam;

		public DeleteCommand(ISolrDeleteParam deleteParam) {
			this.deleteParam = deleteParam;
		}

		public bool? FromPending { get; set; }

		public bool? FromCommitted { get; set; }

		public ISolrDeleteParam DeleteParam {
			get { return deleteParam; }
		}

		public string Execute(ISolrConnection connection) {
			var xml = new XmlDocument();
			var deleteNode = xml.CreateElement("delete");
			foreach (var p in new[] {new KeyValuePair<bool?, string>(FromPending, "fromPending"), new KeyValuePair<bool?, string>(FromCommitted, "fromCommitted")}) {
				if (p.Key.HasValue) {
					var att = xml.CreateAttribute(p.Value);
					att.InnerText = p.Key.Value.ToString().ToLower();
					deleteNode.Attributes.Append(att);
				}
			}
			deleteNode.InnerXml = deleteParam.ToXmlNode().OuterXml;
			return connection.Post("/update", deleteNode.OuterXml);
		}
	}
}