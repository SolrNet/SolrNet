using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrNet;

namespace SimpleInjector.Integration.SolrNet.Tests
{
    [TestClass]
    public class OperationFixture
    {
        private readonly Container _Container;
        private readonly SolrNetConfiguration _Config;

        public OperationFixture()
        {
            _Container = new Container();

            _Config = new SolrNetConfiguration(_Container);
            _Config.ConfigureContainer();
        }

        [TestMethod]
        public void RegisterCore()
        {
            _Config.RegisterCore<Entity>("http://google.com");
        }

        [TestMethod]
        public void RegisterMultiCore()
        {
            _Config.RegisterCore<Entity>("http://google.com");
            _Config.RegisterCore<Entity2>("http://google.com");
        }

        [TestMethod]
        public void ISolrQueryExecuter()
        {
            _Config.RegisterCore<Entity>("http://google.com");
            _Config.RegisterCore<Entity2>("http://google.com");

            var queryExecuter = _Container.GetInstance<ISolrQueryExecuter<Entity>>();
        }

        [TestMethod]
        public void ISolrBasicOperations()
        {
            _Config.RegisterCore<Entity>("http://google.com");
            _Config.RegisterCore<Entity2>("http://google.com");

            var instance = _Container.GetInstance<ISolrBasicOperations<Entity>>();
        }

        [TestMethod]
        public void ISolrBasicReadOnlyOperations()
        {
            _Config.RegisterCore<Entity>("http://google.com");
            _Config.RegisterCore<Entity2>("http://google.com");

            var instance = _Container.GetInstance<ISolrBasicReadOnlyOperations<Entity>>();
        }

        [TestMethod]
        public void ISolrOperation()
        {
            _Config.RegisterCore<Entity>("http://google.com");
            _Config.RegisterCore<Entity2>("http://google.com");

            var instance = _Container.GetInstance<ISolrOperations<Entity>>();
        }

        [TestMethod]
        public void ConnectionBuilder()
        {
            _Config.RegisterCores<EntityConnectionBuilder>();

            var instance1 = _Container.GetInstance<ISolrOperations<Entity>>();
            var instance2 = _Container.GetInstance<ISolrOperations<Entity2>>();
        }
    }
}
