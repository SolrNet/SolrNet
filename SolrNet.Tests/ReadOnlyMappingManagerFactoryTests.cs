using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class ReadOnlyMappingManagerFactoryTests {
        [Test]
        public void IsSingleton() {
            var m1 = ReadOnlyMappingManagerFactory.Create();
            var m2 = ReadOnlyMappingManagerFactory.Create();
            Assert.AreSame(m1, m2);
        }

        [Test]
        public void SwitchCreateImplementation() {
            var stub = MockRepository.GenerateStub<IReadOnlyMappingManager>();
            ReadOnlyMappingManagerFactory.Create = () => stub;
            var m1 = ReadOnlyMappingManagerFactory.Create();
            var m2 = ReadOnlyMappingManagerFactory.Create();
            Assert.AreSame(m1, m2);
        }

        [Test]
        public void SwitchCreateImplementation_different_instances() {
            ReadOnlyMappingManagerFactory.Create = () => MockRepository.GenerateStub<IReadOnlyMappingManager>();
            var m1 = ReadOnlyMappingManagerFactory.Create();
            var m2 = ReadOnlyMappingManagerFactory.Create();
            Assert.AreNotSame(m1, m2);
        }

        [Test]
        public void EveryoneHasTheSameImpl() {
            var mapper = MockRepository.GenerateStub<IReadOnlyMappingManager>();
            ReadOnlyMappingManagerFactory.Create = () => mapper;

            var connection = new SolrConnection("http://localhost");
            var executer = new SolrQueryExecuter<Document>(connection);
            Assert.AreSame(mapper, executer.MappingManager);

            var solr = new SolrBasicServer<Document>(connection);
            var solrExec = (SolrQueryExecuter<Document>)solr.QueryExecuter;
            Assert.AreSame(mapper, solrExec.MappingManager);

        }

        [TestFixtureTearDown]
        public void FixtureTeardown() {
            // restore default factory
            ReadOnlyMappingManagerFactory.Create = ReadOnlyMappingManagerFactory.DefaultCreate;
        }

        public class Document {}

    }
}