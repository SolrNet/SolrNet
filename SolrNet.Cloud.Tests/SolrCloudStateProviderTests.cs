using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolrNet.Cloud.ZooKeeperClient;
using Xunit;

namespace SolrNet.Cloud.Tests
{
    public class SolrCloudStateProviderTests
    {

        [Fact]
        public async Task ThrowExceptionOnBadConnection()
        {
            var provider = new SolrCloudStateProvider("desktop-lmqi80k:9984");
            await Assert.ThrowsAsync<Exceptions.SolrNetCloudConnectionException>(() => provider.InitAsync());
        }
    }
}
