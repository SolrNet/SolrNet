using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Unity.SolrNetCloudIntegration.Collections
{
    public abstract class ResolverOverrideExtractor
    {
        public abstract ResolverOverride[] ExtractResolverOverrides(IBuilderContext context);
    }
}