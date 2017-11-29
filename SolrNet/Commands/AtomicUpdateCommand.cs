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
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace SolrNet.Commands
{
    /// <summary>
    /// Updates a document according to the supplied update specification
    /// </summary>
    public class AtomicUpdateCommand : ISolrCommand
    {
        private string uniqueKey;
        private string id;
        private AtomicUpdateParameters parameters;
        private IEnumerable<AtomicUpdateSpec> updateSpecs;

        public AtomicUpdateCommand(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        {
            this.uniqueKey = uniqueKey ?? throw new ArgumentNullException("Null value supplied for uniqueKey parameter.");
            this.id = id ?? throw new ArgumentNullException("Null value supplied for id parameter.");
            this.updateSpecs = updateSpecs ?? throw new ArgumentNullException("Null value supplied for updateSpecs parameter.");
            this.parameters = parameters;
        }

        public string Execute(ISolrConnection connection)
        {
            string json = GetAtomicUpdateJson();
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var content = new MemoryStream(bytes))
                return connection.PostStream("/update", "text/json; charset=utf-8", content, GetParamsAsKvp());
        }

        public Task<string> ExecuteAsync(ISolrConnection connection)
        {
            string json = GetAtomicUpdateJson();
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var content = new MemoryStream(bytes))
                return connection.PostStreamAsync("/update", "text/json; charset=utf-8", content, GetParamsAsKvp());
        }

        private KeyValuePair<string, string>[] GetParamsAsKvp()
        {
            if(parameters != null && parameters.CommitWithin.HasValue)
            {
                KeyValuePair<string, string>[] kvps = new KeyValuePair<string, string>[1];
                kvps[0] = new KeyValuePair<string, string>("commitwithin", parameters.CommitWithin.ToString());
                return kvps;
            }
            else
            {
                return null;
            }
        }

        private string GetAtomicUpdateJson()
        {
            string json = "[{\"" + uniqueKey + "\":\"" + id + "\"";

            foreach (var updateSpec in updateSpecs)
            {
                json += ",\"" + updateSpec.Field + "\":{\"" + updateSpec.Type.ToString().ToLower() + "\":" + ParseValue(updateSpec.Value) + "}";
            }

            json += "}]";
            return json;
        }

        private string ParseValue(object value)
        {
            if (value is int)
                return value.ToString();
            if (value is string)
                return "\"" + value + "\"";
            if (value is string[])
            {
                string[] values = (string[])value;
                string str = "[";
                for(int i=0; i<values.Length; i++)
                {
                    str += "\"" + values[i] + "\"";
                    if(i != values.Length - 1)
                    {
                        str += ","; // Add comma (except on the last value)
                    }
                }
                str += "]";
                return str;
            }

            throw new ArgumentNullException("Value for atomic update must be int, string or string[].");
        }
    }
}