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

using SolrNet.Commands.Parameters;

namespace SolrNet.DSL.Impl {
    public interface IDSLRun<T> {
        SolrQueryResults<T> Run();
        SolrQueryResults<T> Run(int start, int rows);
        IDSLRun<T> OrderBy(string fieldName);
        IDSLRun<T> OrderBy(string fieldName, Order o);
        IDSLFacetFieldOptions<T> WithFacetField(string fieldName);
        IDSLRun<T> WithFacetQuery(string query);
        IDSLRun<T> WithFacetQuery(ISolrQuery query);
        IDSLRun<T> WithHighlighting(HighlightingParameters parameters);
        IDSLRun<T> WithHighlightingFields(params string[] fields);
    }
}