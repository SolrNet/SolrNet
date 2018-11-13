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
using Moroco;
using SolrNet.Attributes;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;
using SolrNet.Tests.Mocks;

namespace SolrNet.Tests {
    
    public class SolrServerTests {

        [Fact]
        public void Add_single_doc_calls_operations_with_null_add_parameters() {
            var basicServer = new MSolrBasicOperations<TestDocument>();
            basicServer.addWithBoost += (docs, param) => {
                Assert.Null(param);
                return null;
            };
            basicServer.addWithBoost &= x => x.Expect(1);

            var mapper = new MReadOnlyMappingManager();
            var validationManager = new MMappingValidator();

            var s = new SolrServer<TestDocument>(basicServer, mapper, validationManager);
            var t = new TestDocument();
            s.Add(t);
            basicServer.addWithBoost.Verify();
        }

        [Fact]
        public void Add_single_doc_with_add_parameters_calls_operations_with_same_add_parameters() {
            var parameters = new AddParameters { CommitWithin = 4343 };
            var basicServer = new MSolrBasicOperations<TestDocument>();
            basicServer.addWithBoost += (_, p) => {
                Assert.Equal(parameters, p);
                return null;
            };

            var mapper = new MReadOnlyMappingManager();
            var validationManager = new MMappingValidator();

            var s = new SolrServer<TestDocument>(basicServer, mapper, validationManager);
            var t = new TestDocument();
            s.Add(t, parameters);

            Assert.Equal(1, basicServer.addWithBoost.Calls);
        }

        [Fact]
        public void AddWithBoost_single_doc_calls_operations_with_null_add_parameters() {
            var basicServer = new MSolrBasicOperations<TestDocument>();
            basicServer.addWithBoost += (a,b) => {
                Assert.Null(b);
                return null;
            };

            var mapper = new MReadOnlyMappingManager();
            var validationManager = new MMappingValidator();

            var s = new SolrServer<TestDocument>(basicServer, mapper, validationManager);
            var t = new TestDocument();
            s.AddWithBoost(t, 2.1);

            Assert.Equal(1, basicServer.addWithBoost.Calls);
        }

        [Fact]
        public void AddWithBoost_single_doc_with_add_parameters_calls_operations_with_same_add_parameters() {
            var basicServer = new MSolrBasicOperations<TestDocument>();
            var parameters = new AddParameters { CommitWithin = 4343 };
            var t = new TestDocument();
            basicServer.addWithBoost += (docs, p) => {
                Assert.Same(parameters, p);
                var ldocs = docs.ToList();
                Assert.Single(ldocs);
                var doc = ldocs[0];
                Assert.Equal(2.1, doc.Value);
                Assert.Same(t, doc.Key);
                return null;
            };

            var s = new SolrServer<TestDocument>(basicServer, null, null);
            s.AddWithBoost(t, 2.1, parameters);
            Assert.Equal(1, basicServer.addWithBoost.Calls);
        }

        [Fact]
        public void Add_enumerable_calls_operations_with_null_add_parameters() {
            var basicServer = new MSolrBasicOperations<TestDocument>();
            basicServer.addWithBoost += (docs, p) => {
                Assert.Null(p);
                return null;
            };
            var s = new SolrServer<TestDocument>(basicServer, null, null);
            var t = new[] { new TestDocument(), new TestDocument() };
            s.AddRange(t);
            basicServer.addWithBoost.Verify();
        }

        [Fact]
        public void Add_enumerable_with_add_parameters_calls_operations_with_same_add_parameters() {
            var parameters = new AddParameters { CommitWithin = 4343 };
            var basicServer = new MSolrBasicOperations<TestDocument>();
            basicServer.addWithBoost += (docs, p) => {
                Assert.Same(parameters, p);
                return null;
            };
            var s = new SolrServer<TestDocument>(basicServer, null, null);
            var t = new[] { new TestDocument(), new TestDocument() };
            s.AddRange(t, parameters);
        }

        [Fact]
        public void Ping() {
            var basicServer = new MSolrBasicOperations<TestDocument> {
                ping = () => new ResponseHeader {QTime = 2},
            };
            var s = new SolrServer<TestDocument>(basicServer, null, null);
            var r = s.Ping();
            Assert.Equal(2, r.QTime);
        }

        [Fact]
        public void Commit() {
            var basicServer = new MSolrBasicOperations<TestDocument> {
                commit = _ => new ResponseHeader { QTime = 2}
            };
            var s = new SolrServer<TestDocument>(basicServer, null, null);
            var r = s.Commit();
            Assert.Equal(2, r.QTime);
        }

        [Fact]
        public void Rollback() {
            var basicServer = new MSolrBasicOperations<TestDocument>();
            basicServer.rollback += () => new ResponseHeader {QTime = 2};
            var s = new SolrServer<TestDocument>(basicServer, null, null);
            var r = s.Rollback();
            Assert.Equal(2, r.QTime);
            Assert.Equal(1, basicServer.rollback.Calls);
        }

        [Fact]
        public void GetSchema() {
            var basicServer = new MSolrBasicOperations<TestDocument>();
            basicServer.getSchema += () => new SolrSchema {UniqueKey = "bla"};            
            var s = new SolrServer<TestDocument>(basicServer, null, null);
            var r = s.GetSchema();
            Assert.Equal("bla", r.UniqueKey);
        }

        [Fact]
        public void Validate() {
            var basicServer = new MSolrBasicOperations<TestDocument>();
            basicServer.getSchema += () => new SolrSchema();
            var validator = new MMappingValidator();
            validator.enumerate &= x => x.Return(new List<ValidationResult>());
            var s = new SolrServer<TestDocument>(basicServer, null, validator);
            s.EnumerateValidationResults().ToList();
            Assert.Equal(1, basicServer.getSchema.Calls);
            Assert.Equal(1, validator.enumerate.Calls);
        }

        public class TestDocument {
            [SolrUniqueKey]
            public int id {
                get { return 0; }
            }
        }
    }
}