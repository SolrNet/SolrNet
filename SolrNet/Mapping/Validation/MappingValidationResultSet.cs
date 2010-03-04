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

using System.Collections.Generic;

namespace SolrNet.Mapping.Validation {
	/// <summary>
	/// Represents the results of validating a mapping agains the Solr schema XML. If any.
	/// </summary>
	public class MappingValidationResultSet {
		private List<MappingValidationWarning> warnings = new List<MappingValidationWarning>();
		/// <summary>
		/// Gets collection of the warnings.
		/// </summary>
		/// <value>The warnings.</value>
		public List<MappingValidationWarning> Warnings { get { return this.warnings; } }

		private List<MappingValidationError> errors = new List<MappingValidationError>();
		/// <summary>
		/// Gets a collection of the errors.
		/// </summary>
		/// <value>The errors.</value>
		public List<MappingValidationError> Errors { get { return this.errors; } }
	}
}