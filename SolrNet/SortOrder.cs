using System;
using SolrNet.Tests;

namespace SolrNet {
	public class SortOrder {
		private string fieldName;
		private Order? order;

		public SortOrder(string fieldName) {
			if (fieldName.IndexOf(' ') >= 0)
				throw new InvalidSortOrderException();
			this.fieldName = fieldName;
		}

		public SortOrder(string fieldName, Order? order) : this(fieldName) {
			this.order = order;
		}

		public override string ToString() {
			return string.Format("{0}{1}", fieldName, order.HasValue ? " " + order.ToString().ToLower() : "");
		}

		private delegate T F<TP, T>(TP p);

		public static SortOrder Parse(string s) {
			try {
				string[] tokens = s.Split(' ');
				string field = tokens[0];
				Order? o = new F<string[], Order?>(delegate(string[] t) {
				                                   	if (t.Length > 1)
				                                   		return (Order?) Enum.Parse(typeof (Order), t[1].ToUpper());
				                                   	return null;
				                                   })(tokens);
				return new SortOrder(field, o);
			} catch (Exception e) {
				throw new InvalidSortOrderException(e);
			}
		}
	}
}