namespace SolrNet {
	public class RandomSortOrder : SortOrder {
		public RandomSortOrder(string fieldName) : base(fieldName) {}
		public RandomSortOrder(string fieldName, Order order) : base(fieldName, order) {}

		public static RandomSortOrder Instance {
			get { return new RandomSortOrder(""); }
		}
	}
}