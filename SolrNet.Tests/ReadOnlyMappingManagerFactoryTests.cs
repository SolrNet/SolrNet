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

    }
}