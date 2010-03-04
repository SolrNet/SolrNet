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
using System.Text.RegularExpressions;
using System.Xml;
using SolrNet.Schema;

namespace SolrNet.Mapping.Validation {
	/// <summary>
	/// Manages the validation of a mapping against a solr schema XML document.
	/// </summary>
	public class MappingValidationManager : IMappingValidationManager
	{
		private readonly IReadOnlyMappingManager mappingManager;
		private readonly ISolrSchemaParser schemaParser;

		private SolrSchema solrSchema;

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingValidationManager"/> class.
		/// </summary>
		/// <param name="mappingManager">The mapping manager that is used to map types to and from their Solr representation.</param>
		/// <param name="schemaParser">The schema parser to use to parse the Solr schema XML.</param>
		public MappingValidationManager(IReadOnlyMappingManager mappingManager, ISolrSchemaParser schemaParser)
		{
			this.mappingManager = mappingManager;
			this.schemaParser = schemaParser;
		}

		/// <summary>
		/// Validates the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="solrSchemaXml">The solr schema XML.</param>
		/// <returns>A <see cref="MappingValidationResultSet"/> containing all found warnings and errors. If any.</returns>
		public MappingValidationResultSet Validate(Type type, XmlDocument solrSchemaXml)
		{
			var result = new MappingValidationResultSet();

			solrSchema = this.schemaParser.Parse(solrSchemaXml);

			this.ValidateUniqueKey(type, result);
			this.ValidateMappedPropertiesAreInSolrSchema(type, result);
			this.ValidateRequiredFields(type, result);

			return result;
		}

		/// <summary>
		/// Validates that the unique key in the solr field is present in the mapping.
		/// </summary>
		/// <param name="type">The type for which to check the unique key mapping.</param>
		/// <param name="resultSet">The validation result set to add errors and warnings to</param>
		private void ValidateUniqueKey(Type type, MappingValidationResultSet resultSet)
		{
			if (solrSchema.UniqueKey != null) // Validate that the unique key in the schema is present in the type.
			{
				if (!mappingManager.GetUniqueKey(type).Value.Equals(solrSchema.UniqueKey))
					resultSet.Errors.Add(new MappingValidationError(
					                     	String.Format("Solr schema unique key '{0}' does not match document unique key '{1}'.",
					                     	              solrSchema.UniqueKey, mappingManager.GetUniqueKey(type))));
			}
		}

		/// <summary>
		/// Validates that all properties in the mapping are present in the Solr schema
		/// as either a SolrField or a DynamicField
		/// </summary>
		/// <param name="type">The type for which to check the mapped properties.</param>
		/// <param name="resultSet">The validation result set to add errors and warnings to</param>
		private void ValidateMappedPropertiesAreInSolrSchema(Type type, MappingValidationResultSet resultSet)
		{
			foreach (var mappedField in mappingManager.GetFields(type))
			{
				bool fieldFoundInSolrSchema = false;
				foreach (SolrField solrField in solrSchema.SolrFields)
				{
					if (solrField.Name.Equals(mappedField.Value))
					{
						fieldFoundInSolrSchema = true;
						break;
					}
				}

				if (!fieldFoundInSolrSchema)
				{
					foreach (SolrDynamicField dynamicField in solrSchema.SolrDynamicFields)
					{
						if (IsGlobMatch(dynamicField.Name, mappedField.Value))
						{
							fieldFoundInSolrSchema = true;
							break;
						}
					}
				}

				if (!fieldFoundInSolrSchema) // If field couldn't be matched to any of the solrfield, dynamicfields throw an exception.
					resultSet.Errors.Add(new MappingValidationError(
					                     	String.Format("No matching SolrField or DynamicField found in the Solr schema for document property '{0}' in type '{1}'.",
					                     	              mappedField.Key.Name, type.FullName)));
			}
		}

		/// <summary>
		/// Validates that all SolrFields in the SolrSchema which are required are
		/// either present in the mapping or as a CopyField.
		/// </summary>
		/// <param name="type">The type for which to check the mappings</param>
		/// <param name="resultSet">The validation result set to add errors and warnings to</param>
		private void ValidateRequiredFields(Type type, MappingValidationResultSet resultSet)
		{
			foreach (SolrField solrField in solrSchema.SolrFields)
			{
				if (solrField.IsRequired)
				{
					bool fieldFoundInMappingOrCopyFields = false;
					foreach (var mappedField in mappingManager.GetFields(type))
					{
						if (mappedField.Value.Equals(solrField.Name))
						{
							fieldFoundInMappingOrCopyFields = true;
							break;
						}
					}

					if (!fieldFoundInMappingOrCopyFields)
					{
						foreach (SolrCopyField copyField in solrSchema.SolrCopyFields)
						{
							if (copyField.Destination.Equals(solrField.Name))
							{
								fieldFoundInMappingOrCopyFields = true;
								break;
							}
						}
					}

					if (!fieldFoundInMappingOrCopyFields)
						resultSet.Errors.Add(new MappingValidationError(
						                     	String.Format("Required field '{0}' in the Solr schema is not mapped in type '{1}'.", 
						                     	              solrField.Name, type.FullName)));
				}
			}
		}

		private bool IsGlobMatch(string pattern, string input)
		{
			pattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
			var regex = new Regex(pattern);
			return regex.Match(input).Success;
		}

	}
}