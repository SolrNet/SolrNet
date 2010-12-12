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

using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Impl;

namespace SolrNet.Tests {
  [TestFixture]
  class MultiCoreTests {

    public class TestDocument { }

    [Test]
    public void InitialiseWithSingleCore() {
      Startup.Container.Clear();
      Startup.InitContainer();
      Startup.InitCore<TestDocument>("core1", "http://localhost");

      var solrOperations = ServiceLocator.Current.GetInstance<ISolrOperations<TestDocument>>("core1");
      Assert.IsNotNull(solrOperations);
    }

    [Test]
    public void InitialiseWithTwoCores()
    {
      Startup.Container.Clear();
      Startup.InitContainer();
      Startup.InitCore<TestDocument>("core1", "http://localhost");
      Startup.InitCore<TestDocument>("core2", "http://localhost");

      var solrOperations1 = ServiceLocator.Current.GetInstance<ISolrOperations<TestDocument>>("core1");
      var solrOperations2 = ServiceLocator.Current.GetInstance<ISolrOperations<TestDocument>>("core2");
      Assert.IsNotNull(solrOperations1);
      Assert.IsNotNull(solrOperations2);
      Assert.AreNotSame(solrOperations1, solrOperations2);
    }

    [Test]
    public void ConfirmCoreSharedComponentsForDocumentType()
    {
      Startup.Container.Clear();
      Startup.InitContainer();
      Startup.InitCore<TestDocument>("core1", "http://localhost");
      Startup.InitCore<TestDocument>("core2", "http://localhost");

      var solrDocActivator = ServiceLocator.Current.GetInstance<ISolrDocumentActivator<TestDocument>>();
      Assert.IsNotNull(solrDocActivator);
      Assert.AreEqual(typeof(SolrDocumentActivator<TestDocument>), solrDocActivator.GetType());
    }

    [Test]
    [ExpectedException(typeof(ActivationException))]
    public void ConfirmCoreSharedComponentsForDocumentTypeAreNotPerCore()
    {
      Startup.Container.Clear();
      Startup.InitContainer();
      Startup.InitCore<TestDocument>("core1", "http://localhost");
      Startup.InitCore<TestDocument>("core2", "http://localhost");

      var solrDocActivator = ServiceLocator.Current.GetInstance<ISolrDocumentActivator<TestDocument>>("core1");
      Assert.Fail("Should throw Microsoft.Practices.ServiceLocation.ActivationException.");
    }
  }
}
