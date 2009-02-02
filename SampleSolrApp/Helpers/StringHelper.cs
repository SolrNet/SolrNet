namespace SampleSolrApp.Helpers {
    public static class StringHelper {
        public static int TryParse(string u, int defaultValue) {
            try {
                return int.Parse(u);
            } catch {
                return defaultValue;
            }
        }

        public static int TryParse(string u) {
            return TryParse(u, 0);
        }

        public static string IfNullOrEmpty(string a, string b) {
            return string.IsNullOrEmpty(a) ? b : a;
        }

        public static string EmptyToNull(string a) {
            return string.IsNullOrEmpty(a) ? null : a;
        }
    }
}