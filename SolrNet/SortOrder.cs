using System;
using System.Collections.Generic;
using SolrNet.Tests;

namespace SolrNet {
	public class SortOrder {
		private readonly string fieldName;
		private readonly Order order = Order.ASC;
		public static ICollection<SortOrder> Random = new[] {RandomSortOrder.Instance};

		public SortOrder(string fieldName) {
			if (fieldName.IndexOf(' ') >= 0)
				throw new InvalidSortOrderException();
			this.fieldName = fieldName;
		}

		public SortOrder(string fieldName, Order order) : this(fieldName) {
			this.order = order;
		}

		public override string ToString() {
			return string.Format("{0} {1}", fieldName, order.ToString().ToLower());
		}

		private delegate T F<TP, T>(TP p);

		public static SortOrder Parse(string s) {
			try {
				var tokens = s.Split(' ');
				string field = tokens[0];
				if (tokens.Length > 1) {
					Order o = (Order) Enum.Parse(typeof (Order), tokens[1].ToUpper());
					return new SortOrder(field, o);
				}
				return new SortOrder(field);
			} catch (Exception e) {
				throw new InvalidSortOrderException(e);
			}
		}
	}
}