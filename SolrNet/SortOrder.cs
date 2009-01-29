#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using SolrNet.Exceptions;

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
            if (string.IsNullOrEmpty(s))
                throw new InvalidSortOrderException();
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