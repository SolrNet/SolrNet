using System;
using Ninject.Core;
using NUnit.Framework;
using SolrNet;
using SolrNet.Exceptions;

namespace Ninject.Integration.SolrNet.Tests {
    [TestFixture]
    public class Tests {
        [Test]
        [ExpectedException(typeof(SolrConnectionException))]
        public void Ping() {
            var c = new StandardKernel();
            c.Load(new SolrNetModule("http://localhost:8983/solr"));
            var solr = c.Get<ISolrOperations<Entity>>();
            solr.Ping();
        }

        public class Entity {}
    }
}