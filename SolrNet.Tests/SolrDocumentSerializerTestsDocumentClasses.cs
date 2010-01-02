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
using SolrNet.Attributes;

namespace SolrNet.Tests {
    public partial class SolrDocumentSerializerTests {
        public class SampleDoc : ISolrDocument {
            [SolrField]
            public string Id { get; set; }

            [SolrField("Flower")]
            public decimal Dd { get; set; }
        }

        public class TestDocWithCollections : ISolrDocument {
            [SolrField]
            public ICollection<string> coll {
                get { return new[] {"one", "two"}; }
            }
        }

        public class TestDocWithDate : ISolrDocument {
            [SolrField]
            public DateTime Date { get; set; }
        }

        public class TestDocWithBool : ISolrDocument {
            [SolrField]
            public bool B { get; set; }
        }

        public class TestDocWithGuid {
            [SolrField]
            public Guid Key { get; set; }
        }

        public class TestDocWithGenDict {
            [SolrUniqueKey]
            public int Id { get; set; }

            [SolrField]
            public IDictionary<string, string> Dict { get; set; }
        }

        public class TestDocWithGenDict2 {
            [SolrUniqueKey]
            public int Id { get; set; }

            [SolrField]
            public IDictionary<string, int> Dict { get; set; }
        }

        public class TestDocWithGenDict3 {
            [SolrUniqueKey]
            public int Id { get; set; }

            [SolrField("*")]
            public IDictionary<string, object> Dict { get; set; }
        }

        public class TestDocWithNullableDate {
            [SolrField]
            public DateTime? Date { get; set; }
        }

        public class TestDocWithString {
            [SolrField]
            public string Desc { get; set; }
        }
    }
}