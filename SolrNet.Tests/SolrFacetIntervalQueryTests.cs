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
using System.Linq;
using Xunit;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Utils;

namespace SolrNet.Tests
{

    public class SolrFacetIntervalQueryTests
    {
        [Fact]
        public void SerializeWithDateOneItem()
        {
            var q = new SolrFacetIntervalQuery("timestamp");
            q.Sets.Add(new FacetIntervalSet(new FacetIntervalSetValue(new DateTime(2009, 1, 1), true), new FacetIntervalSetValue(new DateTime(2010, 1, 1), true)));
          
            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "timestamp"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.interval.set", "[2009-01-01T00:00:00Z,2010-01-01T00:00:00Z]"), r);
        }

        [Fact]
        public void SerializeWithIntOneItem()
        {
            var q = new SolrFacetIntervalQuery("version");
            q.Sets.Add(new FacetIntervalSet(new FacetIntervalSetValue(1, true), new FacetIntervalSetValue(5, true)));

            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "version"), r);
            Assert.Contains(KV.Create("f.version.facet.interval.set", "[1,5]"), r);
        }


        [Fact]
        public void SerializeWithStringOneItem()
        {
            var q = new SolrFacetIntervalQuery("state");
            q.Sets.Add(new FacetIntervalSet(new FacetIntervalSetValue("az", true), new FacetIntervalSetValue("pa", true)));

            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "state"), r);
            Assert.Contains(KV.Create("f.state.facet.interval.set", "[az,pa]"), r);
        }

        [Fact]
        public void SerializeWithStringSpecialCharacters()
        {
            var q = new SolrFacetIntervalQuery("state");
            q.Sets.Add(new FacetIntervalSet(new FacetIntervalSetValue("a,z", true), new FacetIntervalSetValue("p[]a", true)));

            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "state"), r);
            Assert.Contains(KV.Create("f.state.facet.interval.set", @"[a\,z,p\[\]a]"), r);
        }

        [Fact]
        public void SerializeWithEndUnbound()
        {
            var q = new SolrFacetIntervalQuery("state");
            q.Sets.Add(new FacetIntervalSet(new FacetIntervalSetValue("az", true),null));

            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "state"), r);
            Assert.Contains(KV.Create("f.state.facet.interval.set", "[az,*]"), r);
        }


        [Fact]
        public void SerializeWithStartUnbound()
        {
            var q = new SolrFacetIntervalQuery("state");
            q.Sets.Add(new FacetIntervalSet(null, new FacetIntervalSetValue("pa", true)));

            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "state"), r);
            Assert.Contains(KV.Create("f.state.facet.interval.set", "[*,pa]"), r);
        }

        [Fact]
        public void SerializeWithAllUnbound()
        {
            var q = new SolrFacetIntervalQuery("state");
            q.Sets.Add(new FacetIntervalSet(null, null));

            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "state"), r);
            Assert.Contains(KV.Create("f.state.facet.interval.set", "[*,*]"), r);
        }

        [Fact]
        public void SerializeWithExclusive()
        {
            var q = new SolrFacetIntervalQuery("state");
            q.Sets.Add(new FacetIntervalSet(new FacetIntervalSetValue("az", false), new FacetIntervalSetValue("pa", false)));

            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "state"), r);
            Assert.Contains(KV.Create("f.state.facet.interval.set", "(az,pa)"), r);
        }

        [Fact]
        public void SerializeWithLocalParams()
        {
            var q = new SolrFacetIntervalQuery("state");
            var lp = new LocalParams();
            lp.Add("key", "arizonatopen");
            q.Sets.Add(new FacetIntervalSet(new FacetIntervalSetValue("az", false), new FacetIntervalSetValue("pa", false), lp));             

            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "state"), r);
            Assert.Contains(KV.Create("f.state.facet.interval.set", "{!key=arizonatopen}(az,pa)"), r);
        }



        [Fact]
        public void SerializeWithStringMultipleItem()
        {
            var q = new SolrFacetIntervalQuery("letters");
            q.Sets.Add(new FacetIntervalSet(null, new FacetIntervalSetValue("d", true), "a"));
            q.Sets.Add(new FacetIntervalSet(new FacetIntervalSetValue("e", true), new FacetIntervalSetValue("g", true)));
            q.Sets.Add(new FacetIntervalSet(new FacetIntervalSetValue("k", true), null));

            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "letters"), r);
            Assert.Contains(KV.Create("f.letters.facet.interval.set", "{!key=a}[*,d]"), r);
            Assert.Contains(KV.Create("f.letters.facet.interval.set", "[e,g]"), r);
            Assert.Contains(KV.Create("f.letters.facet.interval.set", "[k,*]"), r);
        }



        [Fact]
        public void IgnoresLocalParams()
        {
            var q = new SolrFacetIntervalQuery(new LocalParams { { "ex", "cat" } } + "state");
            q.Sets.Add(new FacetIntervalSet(null, null));


            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.interval", "{!ex=cat}state"), r);
            Assert.Contains(KV.Create("f.state.facet.interval.set", "[*,*]"), r);
        }




        private static IList<KeyValuePair<string, string>> Serialize(object o)
        {
            var fieldSerializer = new DefaultFieldSerializer();
            var serializer = new DefaultFacetQuerySerializer(new DefaultQuerySerializer(fieldSerializer), fieldSerializer);
            return serializer.Serialize(o).ToList();
        }
    }
}
