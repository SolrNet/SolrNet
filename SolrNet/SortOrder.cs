namespace SolrNet {
	public class SortOrder {
		private string fieldName;
		private Order? order;

		public SortOrder(string fieldName) {
			this.fieldName = fieldName;
		}

		public SortOrder(string fieldName, Order? order) {
			this.fieldName = fieldName;
			this.order = order;
		}

		public override string ToString() {
			return string.Format("{0}{1}", fieldName, order.HasValue ? " " + order : "");
		}
	}
}