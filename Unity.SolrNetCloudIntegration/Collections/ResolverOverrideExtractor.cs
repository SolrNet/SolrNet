using Unity.Builder;
using Unity.Resolution;

namespace Unity.SolrNetCloudIntegration.Collections
{
    public abstract class ResolverOverrideExtractor
    {
        public abstract ResolverOverride[] ExtractResolverOverrides(IBuilderContext context);
    }
}