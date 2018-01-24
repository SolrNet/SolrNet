using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using CommonServiceLocator;

namespace CommonServiceLocator.SolrNet.Tests
{
    /// <summary>
    /// Copied from CommonServiceLocator.WindsorAdapter.Tests
    /// Written by Oren Eini (Ayende)
    /// </summary>
    public abstract class ServiceLocatorTestCase {
        private IServiceLocator locator;

        public ServiceLocatorTestCase()
        {
            locator = CreateServiceLocator();
        }

        public interface ILogger {
            void Log(string msg);
        }

        public class AdvancedLogger : ILogger {
            public void Log(string msg) {
                Console.WriteLine("Log: {0}", msg);
            }
        }

        public class SimpleLogger : ILogger {
            public void Log(string msg) {
                Console.WriteLine(msg);
            }
        }


        protected abstract IServiceLocator CreateServiceLocator();

        [Fact]
        public void GetInstance() {
            var instance = locator.GetInstance<ILogger>();
            Assert.NotNull(instance);
        }

        [Fact]
        
        public void AskingForInvalidComponentShouldRaiseActivationException() {
          Assert.Throws<ActivationException> ( () => locator.GetInstance<IDictionary>());
        }

        [Fact]
        public void GetNamedInstance() {
            var instance = locator.GetInstance<ILogger>(typeof (AdvancedLogger).FullName);
            Assert.IsType<AdvancedLogger>(instance);
        }

        [Fact]
        public void GetNamedInstance2() {
            var instance = locator.GetInstance<ILogger>(typeof (SimpleLogger).FullName);
            Assert.IsType<SimpleLogger>(instance);
        }

        [Fact]
    
        public void GetNamedInstance_WithZeroLenName() {
            Assert.Throws<ActivationException>(() => locator.GetInstance<ILogger>(""));
        }

        [Fact]
        
        public void GetUnknownInstance2() {
            Assert.Throws<ActivationException>(() => locator.GetInstance<ILogger>("test"));
        }

        [Fact]
        public void GetAllInstances() {
            IEnumerable<ILogger> instances = locator.GetAllInstances<ILogger>();
            IList<ILogger> list = new List<ILogger>(instances);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public void GetlAllInstance_ForUnknownType_ReturnEmptyEnumerable() {
            IEnumerable<IDictionary> instances = locator.GetAllInstances<IDictionary>();
            IList<IDictionary> list = new List<IDictionary>(instances);
            Assert.Equal(0, list.Count);
        }

        [Fact]
        public void GenericOverload_GetInstance() {
            Assert.Equal(
                locator.GetInstance<ILogger>().GetType(),
                locator.GetInstance(typeof (ILogger), null).GetType()
                );
        }

        [Fact]
        public void GenericOverload_GetInstance_WithName() {
            Assert.Equal(
                locator.GetInstance<ILogger>(typeof (AdvancedLogger).FullName).GetType(),
                locator.GetInstance(typeof (ILogger), typeof (AdvancedLogger).FullName).GetType()
                );
        }

        [Fact]
        public void Overload_GetInstance_NoName_And_NullName() {
            Assert.Equal(
                locator.GetInstance<ILogger>().GetType(),
                locator.GetInstance<ILogger>(null).GetType()
                );
        }

        [Fact]
        public void GenericOverload_GetAllInstances() {
            var genericLoggers = new List<ILogger>(locator.GetAllInstances<ILogger>());
            var plainLoggers = new List<object>(locator.GetAllInstances(typeof (ILogger)));
            Assert.Equal(genericLoggers.Count, plainLoggers.Count);
            for (int i = 0; i < genericLoggers.Count; i++) {
                Assert.Equal(
                    genericLoggers[i].GetType(),
                    plainLoggers[i].GetType());
            }
        }
    }
}