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
using Microsoft.Practices.ServiceLocation;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet.DSL.Impl {
    public class DeleteBy {
        private readonly ISolrConnection connection;

        public DeleteBy(ISolrConnection connection) {
            this.connection = connection;
        }

        private ISolrQuerySerializer GetQuerySerializer() {
            return ServiceLocator.Current.GetInstance<ISolrQuerySerializer>();
        }

        public void ById(string id) {
            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] { id }, null, GetQuerySerializer()), null);
            cmd.Execute(connection);
        }

        public void ByIds(IEnumerable<string> ids) {
            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, null, GetQuerySerializer()), null);
            cmd.Execute(connection);
        }
       
        public void ByQuery(ISolrQuery q) {
            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(null, q, GetQuerySerializer()), null);
            cmd.Execute(connection);
        }

        public void ByQuery(string q) {
            ByQuery(new SolrQuery(q));
        }
    }
}