using System.Globalization;

namespace SolrNet {
    public class SolrQueryBoost: AbstractSolrQuery {
        private readonly ISolrQuery query;
        private readonly double factor;

        public SolrQueryBoost(ISolrQuery query, double factor) {
            this.query = query;
            this.factor = factor;
        }

        public double Factor {
            get { return factor; }
        }

        public override string Query {
            get {
                return string.Format("({0})^{1}", query.Query, factor.ToString(CultureInfo.InvariantCulture.NumberFormat));
            }
        }
    }
}