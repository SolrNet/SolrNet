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
using System.Linq;
using System.Reflection;

namespace SolrNet.Mapping
{
    /// <summary>
    /// Manual mapping manager
    /// </summary>
    public class MappingManager : IMappingManager
    {
        private readonly bool _useReflectedTypeOnly;
        private readonly IDictionary<Type, Dictionary<string, SolrFieldModel>> mappings = new Dictionary<Type, Dictionary<string, SolrFieldModel>>();
        private readonly IDictionary<Type, SolrFieldModel> uniqueKeys = new Dictionary<Type, SolrFieldModel>();

        public MappingManager() : this(false) { }

        public MappingManager(bool useReflectedTypeOnly)
        {
            _useReflectedTypeOnly = useReflectedTypeOnly;
        }

        /// <inheritdoc />
        public void Add(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            Add(property, property.Name);
        }

        /// <inheritdoc />
        public void Add(PropertyInfo property, string fieldName)
        {
            Add(property, fieldName, null, _useReflectedTypeOnly);
        }

        public void Add(PropertyInfo property, string fieldName, bool useReflectedTypeOnly)
        {
            Add(property, fieldName, null, useReflectedTypeOnly);
        }

        /// <inheritdoc />
        public void Add(PropertyInfo property, string fieldName, float? boost)
        {
            Add(property, fieldName, boost, _useReflectedTypeOnly);
        }

        public void Add(PropertyInfo property, string fieldName, float? boost, bool useReflectedTypeOnly)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (fieldName == null)
                throw new ArgumentNullException("fieldName");

            var declaringType = useReflectedTypeOnly
                ? property.ReflectedType
                : property.DeclaringType ?? property.ReflectedType;

            // create or find the SolrFieldModel dictionary...
            Dictionary<string, SolrFieldModel> solrFieldDict;
            if (!mappings.ContainsKey(declaringType))
            {
                solrFieldDict = new Dictionary<string, SolrFieldModel>();
                mappings[declaringType] = solrFieldDict;
            }
            else
            {
                solrFieldDict = mappings[declaringType];
            }

            // see if the property is already there...
            var m = solrFieldDict.FirstOrDefault(k => k.Value.Property == property);
            if (m.Key != null)
            {
                // it is, so remove it
                solrFieldDict.Remove(m.Key);
            }

            // and add the SolrFieldModel to the dictionary by fieldName
            var fld = new SolrFieldModel(property, fieldName, boost);
            solrFieldDict[fieldName] = fld;
        }

        /// <summary>
        /// Gets all the SolrFieldModels mapped for this type
        /// </summary>
        /// <param name="type">Document type</param>
        /// <returns>Null if <paramref name="type"/> is not mapped</returns>
        public IDictionary<string, SolrFieldModel> GetFields(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return mappings
                .Where(m => m.Key.IsAssignableFrom(type))
                .SelectMany(kvp => kvp.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <inheritdoc />
        public void SetUniqueKey(PropertyInfo property)
        {
            SetUniqueKey(property, _useReflectedTypeOnly);
        }

        public void SetUniqueKey(PropertyInfo property, bool useReflectedTypeOnly)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            var declaringType = useReflectedTypeOnly
                ? property.ReflectedType
                : property.DeclaringType ?? property.ReflectedType;

            if (!mappings.ContainsKey(declaringType))
                throw new ArgumentException(string.Format("Property '{0}.{1}' not mapped. Please use Add() to map it first", declaringType, property.Name));

            var solrFieldDict = mappings[declaringType];

            var theSolrFieldModel = solrFieldDict
                .Where(kvp => kvp.Value.Property == property)
                .Select(kvp => kvp.Value)
                .FirstOrDefault();

            if (theSolrFieldModel == null)
                throw new ArgumentException(string.Format("Property '{0}.{1}' not mapped. Please use Add() to map it first", declaringType, property.Name));

            uniqueKeys[declaringType] = theSolrFieldModel;
        }

        /// <inheritdoc />
        public SolrFieldModel GetUniqueKey(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var prop = uniqueKeys
                .Where(k => k.Key.IsAssignableFrom(type))
                .Select(x => x.Value)
                .FirstOrDefault();

            return prop;
        }

        /// <inheritdoc />
        public ICollection<Type> GetRegisteredTypes()
        {
            return mappings.Select(k => k.Key).ToList();
        }
    }
}
