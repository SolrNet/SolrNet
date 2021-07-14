using SolrNet;
using SolrNet.Impl;
using System;
using Xunit;

namespace SimpleInjector.SolrNet.Tests
{
    public class SimpleInjectorFixture
    {
        private readonly Container Container;

        public SimpleInjectorFixture()
        {
            Container = new Container();
            Container.AddSolrNet("http://localhost:8983/solr/techproducts");
        }

        [Fact]
        public void ResolveIReadOnlyMappingManager()
        {
            var mapper = Container.GetInstance<IReadOnlyMappingManager>();
            Assert.NotNull(mapper);
        }


        [Fact]
        public void ResolveSolrDocumentActivator()
        {
            var solr = Container.GetInstance<ISolrDocumentActivator<Entity>>();
            Assert.NotNull(solr);
        }

        [Fact]
        public void ResolveSolrAbstractResponseParser()
        {
            var solr = Container.GetInstance<ISolrAbstractResponseParser<Entity>>();
            Assert.NotNull(solr);
        }


        [Fact]
        public void ResolveSolrAbstractResponseParsersViaEnumerable()
        {
            var result = Container.GetAllInstances<ISolrAbstractResponseParser<Entity>>();
            Assert.NotNull(result);
            Assert.Single(result);

        }

        [Fact]
        public void ResolveSolrMoreLikeThisHandlerQueryResultsParser()
        {
            var solr = Container.GetInstance<ISolrMoreLikeThisHandlerQueryResultsParser<Entity>>();
            Assert.NotNull(solr);
        }


        [Fact]
        public void ResolveSolrOperations()
        {
            var solr = Container.GetInstance<ISolrOperations<Entity>>();
            Assert.NotNull(solr);
        }



        [Fact]
        public void SetBasicAuthenticationHeader()
        {
            var c = new Container();

            //my credentials
            var credentials = System.Text.Encoding.ASCII.GetBytes("myUsername:myPassword");
            //in base64
            var credentialsBase64 = Convert.ToBase64String(credentials);
            //use the options to set the Authorization header.
            c.AddSolrNet("http://localhost:8983/solr/techproducts", options => { options.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentialsBase64); });

            //test
            var connection = c.GetInstance<ISolrConnection>() as AutoSolrConnection;

            Assert.NotNull(connection.HttpClient.DefaultRequestHeaders.Authorization);
            Assert.Equal(credentialsBase64, connection.HttpClient.DefaultRequestHeaders.Authorization.Parameter);
        }

        public class Entity { }

    }
}
