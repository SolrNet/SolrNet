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

using System;

namespace AutofacContrib.SolrNet
{
    internal class SolrCore
    {
        public string Id { get; private set; }
        public Type DocumentType { get; private set; }
        public string Url { get; private set; }

        public SolrCore(string id, Type documentType, string url)
        {
            Id = id;
            DocumentType = documentType;
            Url = url;
        }
    }
}
