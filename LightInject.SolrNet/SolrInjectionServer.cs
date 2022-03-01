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

using SolrNet;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;

namespace LightInject.SolrNet {
    /// <summary>
    /// Overridden Class to include the wrapper connection so that we can use the dependency injection system for two seperate cores.
    /// </summary>
    /// <typeparam name="TModel">The model being used for the specific core.</typeparam>
    public class SolrInjectionServer<TModel> : SolrServer<TModel> {
        public SolrInjectionServer(ISolrBasicOperations<TModel> basicServer, IReadOnlyMappingManager mappingManager, IMappingValidator _schemaMappingValidator)
            : base(basicServer, mappingManager, _schemaMappingValidator) { }
    }
}
