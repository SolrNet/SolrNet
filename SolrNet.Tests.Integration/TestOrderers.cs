using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SolrNet.Tests.Integration
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MethodDefTestCaseOrderer: ITestCaseOrderer
    {
        public const string Type = "SolrNet.Tests.Integration." + nameof(MethodDefTestCaseOrderer);
        public const string Assembly = "SolrNet.Tests.Integration";
            
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            // orders tests by method definition order
            
            var xunitTests = (IEnumerable<IXunitTestCase>) testCases; // :shrug:
            
            var tests = xunitTests
                .OrderBy(t => t.TestMethod.TestClass.Class.Name)
                .ThenBy(t => t.TestMethod.TestClass.Class.GetMethods(false).ToList().IndexOf(t.Method))
                .Select(x => (TTestCase) x); // :shrug:

            return tests;
        }
    }
    
    public class AlphaTestCollectionOrderer: ITestCollectionOrderer
    {
        public const string Type = "SolrNet.Tests.Integration." + nameof(AlphaTestCollectionOrderer);
        public const string Assembly = "SolrNet.Tests.Integration";
        
        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections) => 
            testCollections.OrderBy(x => x.DisplayName);
    }
}
