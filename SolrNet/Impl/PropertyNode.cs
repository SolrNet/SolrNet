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


namespace SolrNet.Impl {
    /// <summary>
    /// Models a XML node for update consumption
    /// </summary>
    public class PropertyNode {
        /// <summary>
        /// Serialized field value
        /// </summary>
        public string FieldValue { get; set; }

        /// <summary>
        /// Optional field name suffix
        /// </summary>
        public string FieldNameSuffix { get; set; }
    }
}