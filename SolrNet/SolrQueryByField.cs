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
		private readonly string fieldName;
        private readonly string fieldValue;

        /// <summary>
        /// Queries a field for a value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="fieldValue">Field value</param>
        public SolrQueryByField(string fieldName, string fieldValue) {
            this.fieldName = fieldName;
            this.fieldValue = fieldValue;
            Quoted = true;
        }

        /// <summary>
        /// If true (default), special characters (e.g. '?', '*') in the value are quoted.
        /// </summary>
        public bool Quoted { get; set; }

        public string FieldName {
            get { return fieldName; }
        }

        public string FieldValue {
            get { return fieldValue; }
        }
	}
}