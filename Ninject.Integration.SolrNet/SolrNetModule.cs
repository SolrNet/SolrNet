#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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

using Ninject.Core;
using SolrNet;
using SolrNet.Utils;

namespace Ninject.Integration.SolrNet {
    public class SolrNetModule : StandardModule {
        private readonly string serverURL;

        public SolrNetModule(string serverURL) {
            this.serverURL = serverURL;
        }

        public override void Load() {
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            Bind<IReadOnlyMappingManager>().ToConstant(mapper);
            Bind<IRNG>().To<RNG>();
            Bind<IListRandomizer>().To<ListRandomizer>();
            Bind<ISolrConnection>().ToConstant(new SolrConnection(serverURL));
            Bind(typeof (ISolrQueryResultParser<>)).To(typeof (SolrQueryResultParser<>));
            Bind(typeof(ISolrQueryExecuter<>)).To(typeof(SolrQueryExecuter<>));
            Bind(typeof(ISolrDocumentSerializer<>)).To(typeof(SolrDocumentSerializer<>));
            Bind(typeof(ISolrBasicOperations<>)).To(typeof(SolrBasicServer<>));
            Bind(typeof(ISolrBasicReadOnlyOperations<>)).To(typeof(SolrBasicServer<>));
            Bind(typeof(ISolrOperations<>)).To(typeof(SolrServer<>));
            Bind(typeof(ISolrReadOnlyOperations<>)).To(typeof(SolrServer<>));
        }
    }
}