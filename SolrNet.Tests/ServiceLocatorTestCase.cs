using System;
using System.Collections;
using System.Collections.Generic;
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;

namespace SolrNet.Tests {
    /// <summary>
    /// Copied from CommonServiceLocator.WindsorAdapter.Tests
    /// Written by Oren Eini (Ayende)
    /// </summary>
    public abstract class ServiceLocatorTestCase {
        private IServiceLocator locator;

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


        [SetUp]
        public void SetUp() {
            locator = CreateServiceLocator();
        }

        protected abstract IServiceLocator CreateServiceLocator();

        [Test]
        public void GetInstance() {
            var instance = locator.GetInstance<ILogger>();
            Assert.IsNotNull(instance, "instance should not be null");
        }

        [Test]
        [ExpectedException(typeof (ActivationException))]
        public void AskingForInvalidComponentShouldRaiseActivationException() {
            locator.GetInstance<IDictionary>();
        }

        [Test]
        public void GetNamedInstance() {
            var instance = locator.GetInstance<ILogger>(typeof (AdvancedLogger).FullName);
            Assert.IsInstanceOfType(typeof (AdvancedLogger), instance, "Should be an advanced logger");
        }

        [Test]
        public void GetNamedInstance2() {
            var instance = locator.GetInstance<ILogger>(typeof (SimpleLogger).FullName);
            Assert.IsInstanceOfType(typeof (SimpleLogger), instance, "Should be a simple logger");
        }

        [Test]
        [ExpectedException(typeof (ActivationException))]
        public void GetNamedInstance_WithZeroLenName() {
            locator.GetInstance<ILogger>("");
        }

        [Test]
        [ExpectedException(typeof (ActivationException))]
        public void GetUnknownInstance2() {
            locator.GetInstance<ILogger>("test");
        }

        [Test]
        public void GetAllInstances() {
            IEnumerable<ILogger> instances = locator.GetAllInstances<ILogger>();
            IList<ILogger> list = new List<ILogger>(instances);
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void GetlAllInstance_ForUnknownType_ReturnEmptyEnumerable() {
            IEnumerable<IDictionary> instances = locator.GetAllInstances<IDictionary>();
            IList<IDictionary> list = new List<IDictionary>(instances);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void GenericOverload_GetInstance() {
            Assert.AreEqual(
                locator.GetInstance<ILogger>().GetType(),
                locator.GetInstance(typeof (ILogger), null).GetType(),
                "should get the same type"
                );
        }

        [Test]
        public void GenericOverload_GetInstance_WithName() {
            Assert.AreEqual(
                locator.GetInstance<ILogger>(typeof (AdvancedLogger).FullName).GetType(),
                locator.GetInstance(typeof (ILogger), typeof (AdvancedLogger).FullName).GetType(),
                "should get the same type"
                );
        }

        [Test]
        public void Overload_GetInstance_NoName_And_NullName() {
            Assert.AreEqual(
                locator.GetInstance<ILogger>().GetType(),
                locator.GetInstance<ILogger>(null).GetType(),
                "should get the same type"
                );
        }

        [Test]
        public void GenericOverload_GetAllInstances() {
            var genericLoggers = new List<ILogger>(locator.GetAllInstances<ILogger>());
            var plainLoggers = new List<object>(locator.GetAllInstances(typeof (ILogger)));
            Assert.AreEqual(genericLoggers.Count, plainLoggers.Count);
            for (int i = 0; i < genericLoggers.Count; i++) {
                Assert.AreEqual(
                    genericLoggers[i].GetType(),
                    plainLoggers[i].GetType(),
                    "instances (" + i + ") should give the same type");
            }
        }
    }
}