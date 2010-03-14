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
using System.Text;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet {
    /// <summary>
    /// Provides a way to "localize" information about a specific argument that is being sent to Solr. 
    /// In other words, it provides a way to add meta-data to certain argument types such as query strings.
    /// </summary>
    public class LocalParams : Dictionary<string, string> {
        public LocalParams() {}

        public LocalParams(IDictionary<string, string> dictionary) : base(dictionary) {}

        public override string ToString() {
            if (Count == 0)
                return string.Empty;
            var sb = new StringBuilder();
            sb.Append("{!");
            sb.Append(string.Join(" ", Func.ToArray(Func.Select(this, kv => string.Format("{0}={1}", kv.Key, Quote(kv.Value))))));
            sb.Append("}");
            return sb.ToString();
        }

        private string Quote(string v) {
            if (v == null)
                throw new SolrNetException("Null LocalParam value");
            if (!v.Contains(" "))
                return v;
            return string.Format("'{0}'", v.Replace("'", "\\'"));
        }

        public static AbstractSolrQuery operator + (LocalParams p, ISolrQuery q) {
            return new SolrQuery(p + q.Query);
        }
    }
}