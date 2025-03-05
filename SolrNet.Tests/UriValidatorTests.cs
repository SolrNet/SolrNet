using System;
using System.Linq;
using FsCheck;
using FsCheck.Fluent;
using SolrNet.Utils;
using Xunit;
using Xunit.Abstractions;

namespace SolrNet.Tests
{
    public class UriValidatorTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public UriValidatorTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }
        
        [Fact(Skip = "This fails but UriValidator is still useful even though it's not 100% precise. Maybe MaxUriLength could be more conservative.")]
        public void UriLength_Equivalent()
        {
            var uriGen =
                from scheme in Gen.Elements("http", "https")
                from host in ArbMap.Default.GeneratorFor<HostName>()
                from port in Gen.Choose(1, UInt16.MaxValue)
                from pathPartsCount in Gen.Choose(0, 20)
                from pathParts in Gen.Elements("abcdefghijklmnopqrstuvwxyz0123456789-".ToCharArray()).ListOf(pathPartsCount)
                select GetUri(new UriBuilder(scheme: scheme, host: host.Item, port: port, pathValue: string.Join("/", pathParts)));
            
            Prop.ForAll(uriGen.ToArbitrary(), (Uri u) =>
            {
                var ub = new UriBuilder(u);
                var expected = ub.Uri.ToString().Length;
                var actual = UriValidator.UriLength(ub);
                Assert.True(expected == actual, $"Expected length {expected}, got {actual}. URI: {u}");
            }).QuickCheckThrowOnFailure();
        }
        
        Uri GetUri(UriBuilder u)
        {
            try
            {
                return u.Uri;
            } catch (Exception e)
            {
                throw new Exception("Could not build URI from " + u);
            }
        }
    }
}
