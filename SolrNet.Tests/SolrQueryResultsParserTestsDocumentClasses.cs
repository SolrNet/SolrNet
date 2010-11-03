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
using System.Collections;
using System.Collections.Generic;
using SolrNet.Attributes;

namespace SolrNet.Tests {
    public partial class SolrQueryResultsParserTests {
        public class Product  {
            [SolrUniqueKey("id")]
            public string Id { get; set; }

            [SolrField("sku")]
            public string SKU { get; set; }

            [SolrField("name")]
            public string Name { get; set; }

            [SolrField("manu")]
            public string Manufacturer { get; set; }

            [SolrField("cat")]
            public ICollection<string> Categories { get; set; }

            [SolrField("features")]
            public ICollection<string> Features { get; set; }

            [SolrField("includes")]
            public string Includes { get; set; }

            [SolrField("weight")]
            public float Weight { get; set; }

            [SolrField("price")]
            public decimal Price { get; set; }

            [SolrField("popularity")]
            public int Popularity { get; set; }

            [SolrField("inStock")]
            public bool InStock { get; set; }

            [SolrField("word")]
            public string Word { get; set; }

            [SolrField("timestamp")]
            public DateTime Timestamp { get; set; }
        }

        public class TestDocument  {
            [SolrField("advancedview")]
            public string AdvancedView { get; set; }

            [SolrField("basicview")]
            public string BasicView { get; set; }

            [SolrField("id")]
            public int Id { get; set; }
        }

        public class TestDocumentWithoutAttributes {
            public int Id { get; set; }
        }

        public class TestDocumentWithNullableDouble  {
            [SolrField("price")]
            public double? Price { get; set; }
        }

        public class TestDocumentWithArrays  {
            [SolrField("cat")]
            public ICollection<string> Cat { get; set; }

            [SolrField("features")]
            public ICollection<string> Features { get; set; }

            [SolrField("id")]
            public string Id { get; set; }

            [SolrField("inStock")]
            public bool InStock { get; set; }

            [SolrField("manu")]
            public string Manu { get; set; }

            [SolrField("name")]
            public string Name { get; set; }

            [SolrField("popularity")]
            public int Popularity { get; set; }

            [SolrField("price")]
            public double Price { get; set; }

            [SolrField("sku")]
            public string Sku { get; set; }

            [SolrField("numbers")]
            public ICollection<int> Numbers { get; set; }
        }

        public class TestDocumentWithArrays2  {
            [SolrField("numbers")]
            public int[] Numbers { get; set; }
        }

        public class TestDocumentWithArrays3  {
            [SolrField("numbers")]
            public ICollection Numbers { get; set; }
        }

        public class TestDocumentWithArrays4  {
            [SolrField("features")]
            public IEnumerable<string> Features { get; set; }
        }

        public class TestDocumentWithDate  {
            [SolrField]
            public DateTime Fecha { get; set; }
        }

        public class TestDocumentWithNullableDate  {
            [SolrField]
            public DateTime? Fecha { get; set; }
        }

        public class TestDocWithGuid {
            [SolrField]
            public Guid Key { get; set; }
        }

        public class TestDocWithGenDict {
            [SolrField]
            public IDictionary<string, string> Dict { get; set; }
        }

        public class TestDocWithGenDict2 {
            [SolrField]
            public IDictionary<string, int> Dict { get; set; }
        }

        public class TestDocWithGenDict3 {
            [SolrField]
            public IDictionary<string, float> Dict { get; set; }
        }

        public class TestDocWithGenDict4 {
            [SolrField]
            public IDictionary<string, decimal> Dict { get; set; }
        }

        public class TestDocWithGenDict5 {
            [SolrField]
            public string DictOne { get; set; }

            [SolrField("*")]
            public IDictionary<string, object> Dict { get; set; }
        }

        public class TestDocWithoutSetter {
            private readonly int id;

            [SolrField]
            public int Id {
                get { return id; }
            }
        }

    }
}