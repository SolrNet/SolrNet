
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.SolrNetIntegration.Collections
{
    public class NullResolverOverrideExtractor : ResolverOverrideExtractor
    {
        public override ResolverOverride[] ExtractResolverOverrides(IBuilderContext context)
        {
            return new ResolverOverride[0];
        }
    }
}
