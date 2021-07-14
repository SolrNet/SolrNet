using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector.Lifestyles;
using SolrNet;
using Xunit;

namespace SimpleInjector.SolrNet.Tests
{
    public class SimpleInjectorLifestyleTests
    {
        [Fact]
        public void ResolveDefaultTransient()
        {
            var container = new Container();
            container.Options.DefaultLifestyle = Lifestyle.Transient;

            container.AddSolrNet("http://localhost:8983/solr/techproducts");

            container.Verify();

            container.GetInstance<ISolrOperations<Entity>>();
        }

        [Fact]
        public void ResolveDefaultAsyncScoped()
        {
            using (var container = new Container())
            {
                container.Options.DefaultLifestyle = Lifestyle.Scoped;
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

                container.AddSolrNet("http://localhost:8983/solr/techproducts");

                container.Verify();

                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    var uow1 = container.GetInstance<ISolrOperations<Entity>>();
                }
            }
        }

        [Fact]
        public void ResolveDefaultThreadScoped()
        {
            using (var container = new Container())
            {
                container.Options.DefaultLifestyle = Lifestyle.Scoped;
                container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

                container.AddSolrNet("http://localhost:8983/solr/techproducts");

                container.Verify();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    var uow1 = container.GetInstance<ISolrOperations<Entity>>();
                }
            }
        }

        [Fact]
        public void ResolveDefaultSingleton()
        {
            using (var container = new Container())
            {
                container.Options.DefaultLifestyle = Lifestyle.Singleton;

                container.AddSolrNet("http://localhost:8983/solr/techproducts");

                container.Verify();

                using (ThreadScopedLifestyle.BeginScope(container))
                {
                    var uow1 = container.GetInstance<ISolrOperations<Entity>>();
                }
            }
        }

        public class Entity { }
    }
}
