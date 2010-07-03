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
using System.Xml;
using SolrNet.Commands.Parameters;
using SolrNet.Utils;
using System.IO;

namespace SolrNet.Commands {
    /// <summary>
    /// Deletes document(s), either by id or by query
    /// </summary>
	public class AddBinaryCommand : ISolrCommand {
		private readonly Stream content;
        private readonly IEnumerable<KeyValuePair<string, string>> parameters;

        public AddBinaryCommand(Stream content, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            this.content = content;
            this.parameters = parameters;
		}

        public Stream Content {
			get { return content; }
		}

        private static KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }

		public string Execute(ISolrConnection connection) {
            return connection.PostBinary("/update/extract", content, parameters);
		}
	}
}