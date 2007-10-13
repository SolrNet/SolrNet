using System;

namespace SolrNet {
	public interface ISolrCommand {
		void Execute(ISolrConnection connection);
	}
}