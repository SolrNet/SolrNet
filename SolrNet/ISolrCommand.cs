using System;

namespace SolrNet {
	public interface ISolrCommand {
		string Execute(ISolrConnection connection);
	}
}