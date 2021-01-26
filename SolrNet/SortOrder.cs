#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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
using System.Text.RegularExpressions;
using SolrNet.Exceptions;

namespace SolrNet {
    /// <summary>
    /// Sorting order
    /// </summary>
	public class SortOrder {
		private readonly string fieldName;
		private readonly Order order = Order.ASC;
        private static readonly Regex parseRx = new Regex("\\s+", RegexOptions.Compiled);

		///<summary>
		/// Ctor. Default sort order is ascending.
		///</summary>
		///<param name="fieldName">The name of the field to sort by.</param>
		///<exception cref="InvalidSortOrderException">Thrown if field name contains spaces.</exception>
		public SortOrder(string fieldName) {
			this.fieldName = fieldName;
		}

		///<summary>
		/// Ctor.
		///</summary>
        ///<param name="fieldName">The name of the field to sort by.</param>
        ///<param name="order">The <see cref="Order">order</see> to sort in (asc/desc).</param>
		public SortOrder(string fieldName, Order order) : this(fieldName) {
			this.order = order;
		}

        /// <summary>
        /// Sort field
        /// </summary>
        public string FieldName {
            get { return fieldName; }
        }

        /// <summary>
        /// Sort order
        /// </summary>
        public Order Order {
            get { return order; }
        }

        /// <inheritdoc />
        public override string ToString() {
			return string.Format("{0} {1}", FieldName, Order.ToString().ToLower());
		}

        /// <summary>
        /// Parses a sort order in format "field (ASC | DESC)".
        /// E.g. "name desc"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
		public static SortOrder Parse(string s) {
            if (string.IsNullOrEmpty(s))
                return null;
			try {
			    var tokens = parseRx.Split(s.Trim());
				string field = tokens[0];
				if (tokens.Length > 1) {
					var o = (Order) Enum.Parse(typeof (Order), tokens[1].ToUpper());
					return new SortOrder(field, o);
				}
				return new SortOrder(field);
			} catch (Exception e) {
				throw new InvalidSortOrderException(e);
			}
		}
        
        public bool Equals(SortOrder other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return Equals(other.fieldName, fieldName) && Equals(other.order, order);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != typeof (SortOrder)) {
                return false;
            }
            return Equals((SortOrder) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            unchecked {
                return ((fieldName != null ? fieldName.GetHashCode() : 0)*397) ^ order.GetHashCode();
            }
        }
	}
}
