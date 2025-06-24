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
        /// Sets or replaces the value(s) of a particular field
        /// </summary>
        Set,
        /// <summary>
        /// Adds additional value(s) to a multi-valued field
        /// </summary>
        Add,
        /// <summary>
        /// Adds the specified value(s) to a multi-valued field, but only if they are not already present
        /// </summary>
        AddDistinct,
        /// <summary>
        /// Increments a numeric value by a specific amount
        /// </summary>
        Inc,
        /// <summary>
        /// Removes specified value(s) from a multi-valued field
        /// </summary>
        Remove,
        /// <summary>
        /// Removes value(s) from a multi-valued field according to the regular expression(s) specified
        /// </summary>
        RemoveRegex
    }

    /// <summary>
    /// Contains all the information to make a single atomic update to an indexed document.
    /// </summary>
    /// <remarks>
    /// See https://wiki.apache.org/solr/Atomic_Updates
    /// </remarks>
    public class AtomicUpdateSpec
    {
        /// <summary>
        /// Name of the field to apply the update to
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// Type of update to apply
        /// </summary>
        public AtomicUpdateType Type { get; private set; }

        /// <summary>
        /// The update value
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Constructor for single string updates
        /// </summary>
        /// <param name="field">Name of the field to apply the update to</param>
        /// <param name="type">Type of update to apply</param>
        /// <param name="value">String containing the update</param>
        public AtomicUpdateSpec(string field, AtomicUpdateType type, string value)
        {
            Field = field;
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Constructor for multiple string updates
        /// </summary>
        /// <param name="field">Name of the field to apply the update to</param>
        /// <param name="type">Type of update to apply</param>
        /// <param name="value">String array containing the updates</param>
        public AtomicUpdateSpec(string field, AtomicUpdateType type, string[] value)
        {
            Field = field;
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Constructor for int updates
        /// </summary>
        /// <param name="field">Name of the field to apply the update to</param>
        /// <param name="type">Type of update to apply</param>
        /// <param name="value">Int containing the update</param>
        public AtomicUpdateSpec(string field, AtomicUpdateType type, int value)
        {
            Field = field;
            Type = type;
            Value = value;
        }
    }
}