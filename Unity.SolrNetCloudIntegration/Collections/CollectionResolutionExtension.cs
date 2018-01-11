
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Extension;

namespace Unity.SolrNetCloudIntegration.Collections
{
    public class CollectionResolutionExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<ResolverOverrideExtractor, CoupledToImplementationDetailsOverrideExtractor>();

            // PreCreation is the wrong stage, should be Creation. But the DynamicMethodConstructorStrategy
            // will receive the request for resolve prior to the CollectionResolutionStrategy and raise
            // an exception for trying to construct an interface. There is no way to insert a strategy at the
            // beginning of a stage as the StagedStrategyChain is append only. Thats the reason why the ArrayResolutionStrategy
            // is added as first strategy in the Creation stage.
            var strategy = new CollectionResolutionStrategy();
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);


        }
    }
}
