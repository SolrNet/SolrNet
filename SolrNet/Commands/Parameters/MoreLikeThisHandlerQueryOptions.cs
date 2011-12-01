using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
    public class MoreLikeThisHandlerQueryOptions: CommonQueryOptions {
        public MoreLikeThisHandlerQueryOptions(MoreLikeThisHandlerParameters parameters) {
            Parameters = parameters;
        }

        public MoreLikeThisHandlerParameters Parameters { get; set; }
    }
}