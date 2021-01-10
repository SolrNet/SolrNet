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
using SolrNet.Utils;
using System.Threading.Tasks;

namespace SolrNet.Commands {
    /// <summary>
    /// Sends documents to solr for extraction
    /// </summary>
    public class ExtractCommand : ISolrCommand {
        private readonly ExtractParameters parameters;

        public ExtractCommand(ExtractParameters parameters) {
            this.parameters = parameters;
        }

        /// <inheritdoc />
        public string Execute(ISolrConnection connection) {
            var queryParameters = ConvertToQueryParameters();
            return connection.PostStream("/update/extract", parameters.StreamType, parameters.Content, queryParameters);
        }

        /// <inheritdoc />
        public Task<string> ExecuteAsync(ISolrConnection connection)
        {
            var queryParameters = ConvertToQueryParameters();
            return connection.PostStreamAsync("/update/extract", parameters.StreamType, parameters.Content, queryParameters);
        }

        private IEnumerable<KeyValuePair<string, string>> ConvertToQueryParameters() {
            var param = new List<KeyValuePair<string, string>> {
                KV.Create("literal.id", parameters.Id),
                KV.Create("resource.name", parameters.ResourceName)
            };

            if (parameters.Fields != null) {
                foreach (var f in parameters.Fields) {
                    if (f.FieldName == "id") {
                        throw new ArgumentException("ExtractField named 'id' is not permitted in ExtractParameters.Fields - use ExtractParameters.Id instead");
                    }

                    param.Add(KV.Create("literal." + f.FieldName, f.Value));
                }
            }

            if (parameters.StreamType != null) {
                param.Add(KV.Create("stream.type", parameters.StreamType));
            }

            if (parameters.AutoCommit)
                param.Add(KV.Create("commit", "true"));

            if (!string.IsNullOrEmpty(parameters.Prefix))
                param.Add(KV.Create("uprefix", parameters.Prefix));

            if (!string.IsNullOrEmpty(parameters.DefaultField))
                param.Add(KV.Create("defaultField", parameters.DefaultField));

            if (parameters.ExtractOnly) {
                param.Add(KV.Create("extractOnly", "true"));

                if (parameters.ExtractFormat == ExtractFormat.Text)
                    param.Add(KV.Create("extractFormat", "text"));
            }

            if (!string.IsNullOrEmpty(parameters.Capture))
                param.Add(KV.Create("capture", parameters.Capture));

            if (parameters.CaptureAttributes)
                param.Add(KV.Create("captureAttr", "true"));

            if (!string.IsNullOrEmpty(parameters.XPath))
                param.Add(KV.Create("xpath", parameters.XPath));

            if (parameters.LowerNames)
                param.Add(KV.Create("lowernames", "true"));

            return param;
        }
    }
}
