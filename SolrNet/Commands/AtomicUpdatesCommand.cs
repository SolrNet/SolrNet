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
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SolrNet.Commands
{
    /// <summary>
    /// Updates a document according to the supplied update specification
    /// </summary>
    public class AtomicUpdatesCommand : ISolrCommand
    {
        private string uniqueKey;
        private IDictionary<string, IEnumerable<AtomicUpdateSpec>> atomicUpdates;
        private AtomicUpdateParameters parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicUpdatesCommand"/> class with the specified unique key, document id, update specifications, and parameters.
        /// </summary>
        /// <param name="uniqueKey">The unique key field name for the Solr document.</param>
        /// <param name="id">The id of the document to update.</param>
        /// <param name="updateSpecs">The atomic update specifications for the document.</param>
        /// <param name="parameters">Additional parameters for the atomic update operation.</param>
        public AtomicUpdatesCommand(string uniqueKey, string id, IEnumerable<AtomicUpdateSpec> updateSpecs, AtomicUpdateParameters parameters)
        : this(uniqueKey, new Dictionary<string, IEnumerable<AtomicUpdateSpec>> { { id, updateSpecs } }, parameters)
        {
            if (id == null)
                throw new ArgumentNullException("Null value supplied for id parameter.");
            if (updateSpecs == null)
                throw new ArgumentNullException("Null value supplied for updateSpecs parameter.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicUpdatesCommand"/> class with the specified unique key, atomic updates, and parameters.
        /// </summary>
        /// <param name="uniqueKey">The unique key field name for the Solr document.</param>
        /// <param name="atomicUpdates">A dictionary mapping document IDs to their respective atomic update specifications.</param>
        /// <param name="parameters">Additional parameters for the atomic update operation.</param>
        public AtomicUpdatesCommand(string uniqueKey, IDictionary<string, IEnumerable<AtomicUpdateSpec>> atomicUpdates, AtomicUpdateParameters parameters)
        {
            this.uniqueKey = uniqueKey ?? throw new ArgumentNullException("Null value supplied for uniqueKey parameter.");
            this.atomicUpdates = atomicUpdates ?? throw new ArgumentNullException("Null value supplied for atomicUpdates parameter.");
            this.parameters = parameters;
        }

        /// <inheritdoc/>
        public string Execute(ISolrConnection connection)
        {
            string json = GetAtomicUpdateJson();
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var content = new MemoryStream(bytes))
                return connection.PostStream("/update", "text/json; charset=utf-8", content, GetParamsAsKvp());
        }

        /// <inheritdoc/>
        public async Task<string> ExecuteAsync(ISolrConnection connection)
        {
            string json = GetAtomicUpdateJson();
            var bytes = Encoding.UTF8.GetBytes(json);
            using (var content = new MemoryStream(bytes))
                return await connection.PostStreamAsync("/update", "text/json; charset=utf-8", content, GetParamsAsKvp());
        }

        private KeyValuePair<string, string>[] GetParamsAsKvp()
        {
            if(parameters != null && parameters.CommitWithin.HasValue)
            {
                KeyValuePair<string, string>[] kvps = new KeyValuePair<string, string>[1];
                kvps[0] = new KeyValuePair<string, string>("commitWithin", parameters.CommitWithin.ToString());
                return kvps;
            }
            else
            {
                return null;
            }
        }

        private string GetAtomicUpdateJson()
        {
            JArray updatesArray = new JArray();
            foreach (KeyValuePair<string, IEnumerable<AtomicUpdateSpec>> kv in atomicUpdates)
            { 
                updatesArray.Add(GetAtomicUpdateJson(kv.Key, kv.Value));
            }
            return updatesArray.ToString(Formatting.None);
        }

        private JObject GetAtomicUpdateJson(string id, IEnumerable<AtomicUpdateSpec> updateSpecs)
        {
            JObject updateObject = new JObject();
            updateObject[uniqueKey] = id;

            foreach (var updateSpec in updateSpecs)
            {
                string fieldName = updateSpec.Field;
                string updateType = GetUpdateType(updateSpec.Type);
                JToken valueToken = updateSpec.Value == null ? null : JToken.FromObject(updateSpec.Value);

                // Create the atomic update object for this field
                JObject fieldUpdate = new JObject();
                fieldUpdate[updateType] = valueToken;

                // Add the field update to the main object
                updateObject[fieldName] = fieldUpdate;
            }

            return updateObject;
        }

        private string GetUpdateType(AtomicUpdateType type)
        {
            switch (type)
            {
                case AtomicUpdateType.AddDistinct:
                    return "add-distinct";
                default:
                    return type.ToString().ToLowerInvariant();
            }
        }
    }
}
