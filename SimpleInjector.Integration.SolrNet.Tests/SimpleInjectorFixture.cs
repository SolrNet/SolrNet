using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrNet.Impl;

namespace SimpleInjector.Integration.SolrNet.Tests
{
    [TestClass]
    public class SimpleInjectorFixture
    {
        private readonly Container _Container;

        public SimpleInjectorFixture()
        {
            _Container = new Container();

            var config = new SolrNetConfiguration(_Container);
            config.ConfigureContainer();
        }

        [TestMethod]
        public void SolrAbstracctParser()
        {
            var instance = _Container.GetInstance<ISolrAbstractResponseParser<Entity>>();
        }

        [TestMethod]
        public void SolrQuerySerializer()
        {
            var instance = _Container.GetInstance<ISolrQuerySerializer>();
        }

        [TestMethod]
        public void SolrFacetQuerySerializer()
        {
            var instance = _Container.GetInstance<ISolrFacetQuerySerializer>();
        }

        [TestMethod]
        public void SolrMoreLikeThisHandlerQueryResultsParser()
        {
            var instance = _Container.GetInstance<ISolrMoreLikeThisHandlerQueryResultsParser<Entity>>();
        }
    }
}
