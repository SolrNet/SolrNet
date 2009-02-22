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

using System;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Mapping;
using SolrNet.Utils;

namespace Castle.Facilities.SolrNetIntegration {
    public class SolrNetFacility : AbstractFacility {
        private readonly string solrURL;
        public IReadOnlyMappingManager Mapper { get; set; }

        public SolrNetFacility() {}

        public SolrNetFacility(string solrURL) {
            ValidateUrl(solrURL);
            this.solrURL = solrURL;
        }

        private string GetSolrUrl() {
            if (solrURL != null)
                return solrURL;
            if (FacilityConfig == null)
                throw new FacilityException("Please add solrURL to the SolrNetFacility configuration");
            var configNode = FacilityConfig.Children["solrURL"];
            if (configNode == null)
                throw new FacilityException("Please add solrURL to the SolrNetFacility configuration");
            var url = configNode.Value;
            ValidateUrl(url);
            return url;
        }

        protected override void Init() {
            var mapper = Mapper ?? new MemoizingMappingManager(new AttributesMappingManager());
            Kernel.AddComponentInstance<IReadOnlyMappingManager>(mapper);
            Kernel.Register(Component.For<IRNG>().ImplementedBy<RNG>());
            Kernel.Register(Component.For<ISolrConnection>().ImplementedBy<SolrConnection>()
                                .Parameters(Parameter.ForKey("serverURL").Eq(GetSolrUrl())));
            Kernel.Register(Component.For(typeof (ISolrQueryResultParser<>)).ImplementedBy(typeof (SolrQueryResultParser<>)));
            Kernel.Register(Component.For(typeof (ISolrQueryExecuter<>)).ImplementedBy(typeof (SolrQueryExecuter<>)));
            Kernel.Register(Component.For(typeof(ISolrDocumentSerializer<>)).ImplementedBy(typeof(SolrDocumentSerializer<>)));
            Kernel.Register(Component.For(typeof(ISolrBasicOperations<>), typeof(ISolrBasicReadOnlyOperations<>))
                .ImplementedBy(typeof(SolrBasicServer<>)));
            Kernel.Register(Component.For(typeof(ISolrOperations<>), typeof(ISolrReadOnlyOperations<>))
                .ImplementedBy(typeof(SolrServer<>)));
        }

        private static void ValidateUrl(string url) {
            try {
                var u = new Uri(url);
                if (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)
                    throw new FacilityException("Only HTTP or HTTPS protocols are supported");
            } catch (ArgumentException e) {
                throw new FacilityException(string.Format("Invalid URL '{0}'", url), e);
            } catch (UriFormatException e) {
                throw new FacilityException(string.Format("Invalid URL '{0}'", url), e);
            }
        }
    }
}