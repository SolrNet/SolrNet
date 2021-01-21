
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;


namespace Unity.SolrNetCloudIntegration.Collections
{
    public class CollectionResolutionStrategy : BuilderStrategy
    {
        private static readonly MethodInfo genericResolveCollectionMethod = typeof(CollectionResolutionStrategy)
                .GetMethod("ResolveCollection", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        private delegate object CollectionResolver(IBuilderContext context);

        public override void PreBuildUp(IBuilderContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            Type typeToBuild = context.BuildKey.Type;

            if (typeToBuild.IsGenericType)
            {
                Type openGeneric = typeToBuild.GetGenericTypeDefinition();

                if (openGeneric == typeof(IEnumerable<>) ||
                    openGeneric == typeof(ICollection<>) ||
                    openGeneric == typeof(IList<>))
                {
                    Type elementType = typeToBuild.GetGenericArguments()[0];

                    MethodInfo resolverMethod = genericResolveCollectionMethod.MakeGenericMethod(elementType);

                    CollectionResolver resolver = (CollectionResolver)Delegate.CreateDelegate(typeof(CollectionResolver), resolverMethod);

                    context.Existing = resolver(context);
                    context.BuildComplete = true;
                }
            }            
        }

        private static object ResolveCollection<T>(IBuilderContext context)
        {
            IUnityContainer container = context.NewBuildUp<IUnityContainer>();

            var extractor = context.NewBuildUp<ResolverOverrideExtractor>();

            var resolverOverrides = extractor.ExtractResolverOverrides(context);

            List<T> results = new List<T>(container.ResolveAll<T>(resolverOverrides));

            return results;
        }
    }
}
