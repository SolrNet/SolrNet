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

using System.Text.RegularExpressions;

namespace SolrNet {
    /// <summary>
    /// Queries a field for a value
    /// </summary>
	public class SolrQueryByField : AbstractSolrQuery {
		private readonly string q;

		public SolrQueryByField(string fieldName, string fieldValue) {
			if (fieldName == null || fieldValue == null)
				q = null;
			else
				q = string.Format("{0}:{1}", fieldName, quote(fieldValue));
		}

		public string quote(string value) {
            string r = Regex.Replace(value, "(\\+|\\-|\\&\\&|\\|\\||\\!|\\{|\\}|\\[|\\]|\\^|\\(|\\)|\\\"|\\~|\\:|\\;|\\\\)", "\\$1");
			if (r.IndexOf(' ') != -1)
				r = string.Format("\"{0}\"", r);
			return r;
		}

		/// <summary>
		/// query string
		/// </summary>
		public override string Query {
			get { return q; }
		}
	}
}