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

namespace SolrNet
{
    /// <summary>
    /// Possible types of atomic update
    /// </summary>
    public enum AtomicUpdateType
    {
        /// <summary>
        /// Sets or replaces a particular value, or remove the value if null is specified as the new value
        /// </summary>
        Set,
        /// <summary>
        /// Adds an additional value to a multi-valued field
        /// </summary>
        Add,
        /// <summary>
        /// Increments a numeric value by a specific amount
        /// </summary>
        Inc
    }

    /// <summary>
    /// Contains all the information to make a single atomic update to a indexed document.
    /// </summary>
    /// <remarks>
    /// See https://wiki.apache.org/solr/Atomic_Updates
    /// </remarks>
    public class AtomicUpdateSpec
    {
        /// <summary>
        /// Name of the field to apply the update to
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Type of update to apply
        /// </summary>
        public AtomicUpdateType Type { get; set; }

        /// <summary>
        /// The update value
        /// </summary>
        public string Value { get; set; }

        public AtomicUpdateSpec(string field, AtomicUpdateType type, string value)
        {
            Field = field;
            Type = type;
            Value = value;
        }
    }
}