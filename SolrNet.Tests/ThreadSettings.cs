using System;
using System.Globalization;
using System.Threading;

namespace SolrNet.Tests {
    public static class ThreadSettings {
        private class DisposableCultureInfo : IDisposable {
            private readonly CultureInfo original;

            public DisposableCultureInfo(CultureInfo culture) {
                original = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = culture;
            }

            public void Dispose() {
                Thread.CurrentThread.CurrentCulture = original;
            }
        }

        public static IDisposable Culture(CultureInfo culture) {
            return new DisposableCultureInfo(culture);
        }

        public static IDisposable Culture(string culture) {
            return Culture(CultureInfo.GetCultureInfo(culture));
        }
    }
}