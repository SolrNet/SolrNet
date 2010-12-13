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

using System.Collections.Generic;

namespace SolrNet {
    /// <summary>
    /// Queries a field for a list of possible values
    /// </summary>
    public class SolrQueryInList : AbstractSolrQuery {
        private readonly string fieldName;
        private readonly IEnumerable<string> list;

        public SolrQueryInList(string fieldName, IEnumerable<string> list) {
            this.fieldName = fieldName;
            this.list = list;
        }

        public SolrQueryInList(string fieldName, params string[] values) : this(fieldName, (IEnumerable<string>) values) {}

        public string FieldName {
            get { return fieldName; }
        }

        public IEnumerable<string> List {
            get { return list; }
        }
    }
}