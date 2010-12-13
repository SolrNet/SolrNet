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
using System.Xml.Linq;

namespace SolrNet.Impl {
    /// <summary>
    /// Parses a single Solr XML result node
    /// </summary>
    public interface ISolrFieldParser {
        /// <summary>
        /// True if this parser can handle the solrType (int, bool, str, ...)
        /// </summary>
        /// <param name="solrType"></param>
        /// <returns></returns>
        bool CanHandleSolrType(string solrType);

        /// <summary>
        /// True if this parser can handle a type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool CanHandleType(Type t);

        /// <summary>
        /// Parses a single Solr XML result node
        /// </summary>
        /// <param name="field">Solr XML result node</param>
        /// <param name="t">Type the node value should be converted to</param>
        /// <returns>Parsed value</returns>
        object Parse(XElement field, Type t);
    }
}