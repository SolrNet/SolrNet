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
using System.Xml.Linq;

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
            this.uniqueKey = uniqueKey;
            this.id = id;
            this.updateSpecs = updateSpecs;
            this.parameters = parameters;
        }

        public string Execute(ISolrConnection connection)
        {
            var addElement = new XElement("add");
            if (parameters != null)
            {
                if (parameters.CommitWithin.HasValue)
                {
                    var commit = new XAttribute("commitWithin", parameters.CommitWithin.Value.ToString(CultureInfo.InvariantCulture));
                    addElement.Add(commit);
                }
            }

            // Overwrite must always be true for the update to take affect
            var overwrite = new XAttribute("overwrite", "true");
            addElement.Add(overwrite);

            var doc = new XElement("doc");
            addElement.Add(doc);

            // Check we have a value for the ID
            if(id == null)
            {
                throw new ArgumentNullException("Null value supplied for id parameter.");
            }

            var idField = new XElement("field", id);
            var uniqueName = new XAttribute("name", uniqueKey);
            idField.Add(uniqueName);
            doc.Add(idField);

            foreach (var updateSpec in updateSpecs)
            {
                var field = new XElement("field", updateSpec.Value);
                var name = new XAttribute("name", updateSpec.Field);
                field.Add(name);

                if (updateSpec.Value == null)
                {
                    var nullAttrib = new XAttribute("null", "true");
                    field.Add(nullAttrib);
                }
                else
                {
                    var updateType = new XAttribute("update", updateSpec.Type.ToString().ToLower());
                    field.Add(updateType);
                }

                doc.Add(field);
            }

            return connection.Post("/update", addElement.ToString(SaveOptions.DisableFormatting));
        }
    }
}