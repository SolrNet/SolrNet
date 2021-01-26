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
using System.Linq;
using System.Xml.Linq;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    /// <summary>
    /// Pass-through document visitor
    /// </summary>
    public class RegularDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly ISolrFieldParser parser;
        private readonly IReadOnlyMappingManager mapper;

        /// <summary>
        /// Pass-through document visitor
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="mapper"></param>
        public RegularDocumentVisitor(ISolrFieldParser parser, IReadOnlyMappingManager mapper) {
            this.parser = parser;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public void Visit(object doc, string fieldName, XElement field) {
            var allFields = mapper.GetFields(doc.GetType());
            SolrFieldModel thisField;
            if (!allFields.TryGetValue(fieldName, out thisField))
                return;
            if (!thisField.Property.CanWrite)
                return;
            if (parser.CanHandleSolrType(field.Name.LocalName) &&
                parser.CanHandleType(thisField.Property.PropertyType)) {
                var v = parser.Parse(field, thisField.Property.PropertyType);
                try {
                    thisField.Property.SetValue(doc, v, null);                    
                } catch (ArgumentException e) {
                    throw new ArgumentException(string.Format("Could not convert value '{0}' to property '{1}' of document type {2}", v, thisField.Property.Name, thisField.Property.DeclaringType), e);
                }
            }
        }
    }
}
