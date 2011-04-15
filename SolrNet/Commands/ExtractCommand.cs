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
using System.Collections.Generic;
using System.Linq;

namespace SolrNet.Commands {
    /// <summary>
    /// Sends documents to solr for extraction
    /// </summary>
    public class ExtractCommand : ISolrCommand {
        private readonly ExtractParameters parameters;

        public ExtractCommand(ExtractParameters parameters) {
            this.parameters = parameters;
        }

        public string Execute(ISolrConnection connection) {
            var queryParameters = ConvertToQueryParameters();
            return connection.PostStream("/update/extract", parameters.StreamType, parameters.Content, queryParameters);
        }

        private IEnumerable<KeyValuePair<string, string>> ConvertToQueryParameters() {
            var parms = new List<KeyValuePair<string, string>> {
                KV("literal.id", parameters.Id),
                KV("resource.name", parameters.ResourceName)
            };

            if (parameters.Fields != null)
            {
                foreach (var f in parameters.Fields)
                {
                    if (f.FieldName == "id")
                    {
                        throw new ArgumentException("ExtractField named 'id' is not permitted in ExtractParameters.Fields - use ExtractParameters.Id instead");
                    }

                    parms.Add(KV("literal." + f.FieldName, f.Value));
                }
            }

            if (parameters.StreamType != null)
            {
                parms.Add(new KeyValuePair<string, string>("stream.type", parameters.StreamType));
            }

            if (parameters.AutoCommit)
                parms.Add(KV("commit", "true"));

            if (!string.IsNullOrEmpty(parameters.Prefix))
                parms.Add(KV("uprefix", parameters.Prefix));

            if (!string.IsNullOrEmpty(parameters.DefaultField))
                parms.Add(KV("defaultField", parameters.DefaultField));

            if (parameters.ExtractOnly)
            {
                parms.Add(KV("extractOnly", "true"));

                if (parameters.ExtractFormat == ExtractFormat.Text)
                    parms.Add(KV("extractFormat", "text"));
            }

            if (!string.IsNullOrEmpty(parameters.Capture))
                parms.Add(KV("capture", parameters.Capture));

            if (parameters.CaptureAttributes)
                parms.Add(KV("captureAttr", "true"));

            if (!string.IsNullOrEmpty(parameters.XPath))
                parms.Add(KV("xpath", parameters.XPath));

            if (parameters.LowerNames)
                parms.Add(KV("lowernames", "true"));

            return parms;
        }

        private static KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}