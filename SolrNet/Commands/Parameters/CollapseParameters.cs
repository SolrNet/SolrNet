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

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Parameters to query collapse
    /// <see href="https://issues.apache.org/jira/browse/SOLR-236"/>
    /// </summary>
    public enum CollapseType {
        /// <summary>
        /// Collapse all documents having equal collapsing field
        /// </summary>
        Normal,
        /// <summary>
        /// Collapse only consecutive documents
        /// </summary>
        Adjacent
    }

    /// <summary>
    /// Controls if collapsing happens before or after faceting
    /// </summary>
    public enum CollapseFacetMode {
        /// <summary>
        /// Faceting happens before collapsing
        /// </summary>
        Before,
        /// <summary>
        /// Faceting happens after collapsing
        /// </summary>
        After
    }

    /// <summary>
    /// Parameters to query collapse.
    /// See https://issues.apache.org/jira/browse/SOLR-236
    /// </summary>
    [Obsolete("Use result grouping instead")]
    public class CollapseParameters {
        /// <summary>
        /// Field to group results by
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Number of continuous results allowed before collapsing
        /// </summary>
        public int? Threshold { get; set; }

        /// <summary>
        /// limits the number of documents that CollapseFilter will process to create the filter DocSet. 
        /// The intention of this is to be able to limit the time collapsing will take for very large result sets 
        /// (obviously at the expense of accurate collapsing in those cases).
        /// </summary>
        public int? MaxDocs { get; set; }

        /// <summary>
        /// Number of continuous results allowed before collapsing
        /// </summary>
        public CollapseFacetMode FacetMode { get; set; }

        /// <summary>
        /// Collapse type: Adjacent or Normal
        /// </summary>
        public CollapseType Type { get; set; }

        public CollapseParameters(string field) {
            Field = field;
            Type = CollapseType.Normal;
            FacetMode = CollapseFacetMode.Before;
        }
    }
}
